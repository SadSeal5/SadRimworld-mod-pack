using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.QoL.Patches.Safety
{
    [HarmonyPatch(typeof(GenConstruct), nameof(GenConstruct.CanPlaceBlueprintOver))]
    public static class Patch_ReplaceStuff
    {
        public static bool isReplacingBuilding = false;

        public static void ResetState()
        {
            isReplacingBuilding = false;
        }

        public static bool CanReplaceDefs(BuildableDef newDef, BuildableDef oldDef)
        {
            if (newDef == null || oldDef == null) return false;
            if (!(newDef is ThingDef newThingDef) || !(oldDef is ThingDef oldThingDef)) return false;
            if (newThingDef.category != ThingCategory.Building || oldThingDef.category != ThingCategory.Building) return false;
            if (newThingDef.IsFrame || newThingDef.IsBlueprint || oldThingDef.IsFrame || oldThingDef.IsBlueprint) return false;
            if (newThingDef.mineable || oldThingDef.mineable) return false;
            if (typeof(Mineable).IsAssignableFrom(oldThingDef.thingClass) || typeof(Mineable).IsAssignableFrom(newThingDef.thingClass)) return false;

            // Filter out conduits
            if (newThingDef.building?.isPowerConduit == true || oldThingDef.building?.isPowerConduit == true) return false;

            // Filter out edifice mismatches
            if (newThingDef.IsEdifice() != oldThingDef.IsEdifice()) return false;

            // Filter out size mismatches
            if (newThingDef.size != oldThingDef.size) return false;

            // Wood Bed to Marble Bed, Wood Wall to Slate Wall, Wood Door to Steel Door
            if (newThingDef == oldThingDef)
            {
                return newThingDef.MadeFromStuff;
            }

            // Wall over Wall (different def)
            if (IsWall(newThingDef) && IsWall(oldThingDef))
            {
                return true;
            }

            // Door over Wall, Wall over Door, Door over Door
            if ((IsDoor(newThingDef) && IsWall(oldThingDef)) ||
                (IsWall(newThingDef) && IsDoor(oldThingDef)) ||
                (IsDoor(newThingDef) && IsDoor(oldThingDef)))
            {
                return true;
            }

            if (GenConstruct.HasMatchingReplacementTag(newThingDef, oldThingDef))
            {
                return true;
            }

            return false;
        }

        public static void Postfix(BuildableDef newDef, ThingDef oldDef, ref bool __result)
        {
            if (QoLModule.Instance?.IsEnabled != true) return;
            if (__result) return;
            if (QoLModule.ModSettings?.enableReplaceStuff != true) return;

            if (CanReplaceDefs(newDef, oldDef))
            {
                __result = true;
            }
        }

        public static bool IsWall(ThingDef def)
        {
            if (def == null || def.IsFrame || def.IsBlueprint || def.mineable || typeof(Mineable).IsAssignableFrom(def.thingClass)) return false;
            if (def.building != null && def.building.isPlaceOverableWall) return true;
            if (def.holdsRoof && def.fillPercent >= 1f && def.passability == Traversability.Impassable) return true;
            return def == ThingDefOf.Wall || (def.defName != null && def.defName.Contains("Wall"));
        }

        public static bool IsDoor(ThingDef def)
        {
            if (def == null || def.IsFrame || def.IsBlueprint) return false;
            if (typeof(Building_Door).IsAssignableFrom(def.thingClass)) return true;
            return def == ThingDefOf.Door || (def.defName != null && def.defName.Contains("Door"));
        }

        public static bool IsPlayerDeconstructible(Thing t)
        {
            return !(t is Building b) || b.DeconstructibleBy(Faction.OfPlayer).Accepted;
        }
    }

    [HarmonyPatch(typeof(GenConstruct), nameof(GenConstruct.BlocksConstruction))]
    public static class Patch_GenConstruct_BlocksConstruction
    {
        public static void Postfix(Thing constructible, Thing t, ref bool __result)
        {
            if (QoLModule.Instance?.IsEnabled != true) return;
            if (!__result) return;
            if (QoLModule.ModSettings?.enableReplaceStuff != true) return;
            if (constructible == null || t == null) return;

            BuildableDef newDef = constructible.def?.entityDefToBuild;
            if (newDef != null && Patch_ReplaceStuff.CanReplaceDefs(newDef, t.def) && Patch_ReplaceStuff.IsPlayerDeconstructible(t))
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(WorkGiver_DeconstructForBlueprint), nameof(WorkGiver_DeconstructForBlueprint.HasJobOnThing))]
    public static class Patch_WorkGiver_DeconstructForBlueprint
    {
        public static bool Prefix(Verse.Pawn pawn, Thing t, ref bool __result)
        {
            if (QoLModule.Instance?.IsEnabled != true) return true;
            if (QoLModule.ModSettings?.enableReplaceStuff != true) return true;
            if (pawn?.Map == null || t == null) return true;
            if (!Patch_ReplaceStuff.IsPlayerDeconstructible(t)) return true;

            List<Thing> thingList = t.Position.GetThingList(pawn.Map);
            for (int i = 0; i < thingList.Count; i++)
            {
                if (thingList[i] is Blueprint bp)
                {
                    BuildableDef newDef = bp.def?.entityDefToBuild;
                    if (newDef != null && Patch_ReplaceStuff.CanReplaceDefs(newDef, t.def))
                    {
                        __result = false;
                        return false;
                    }
                }
                if (thingList[i] is Frame frame)
                {
                    BuildableDef newDef = frame.def?.entityDefToBuild;
                    if (newDef != null && Patch_ReplaceStuff.CanReplaceDefs(newDef, t.def))
                    {
                        __result = false;
                        return false;
                    }
                }
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(GenSpawn), nameof(GenSpawn.SpawningWipes))]
    public static class Patch_GenSpawn_SpawningWipes
    {
        public static void Postfix(BuildableDef newEntDef, BuildableDef oldEntDef, ref bool __result)
        {
            if (QoLModule.Instance?.IsEnabled != true) return;
            if (!__result) return;
            if (QoLModule.ModSettings?.enableReplaceStuff != true) return;
            if (newEntDef == null || oldEntDef == null) return;

            ThingDef newThingDef = newEntDef as ThingDef;
            if (newThingDef == null || (!newThingDef.IsFrame && !newThingDef.IsBlueprint))
            {
                return;
            }

            BuildableDef builtNew = GenConstruct.BuiltDefOf(newEntDef as ThingDef) ?? newEntDef;
            BuildableDef builtOld = GenConstruct.BuiltDefOf(oldEntDef as ThingDef) ?? oldEntDef;

            if (Patch_ReplaceStuff.CanReplaceDefs(builtNew, builtOld))
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(GenConstruct), nameof(GenConstruct.CanPlaceBlueprintAt_NewTemp))]
    public static class Patch_GenConstruct_CanPlaceBlueprintAt
    {
        public static void Postfix(BuildableDef entDef, IntVec3 center, Rot4 rot, Map map, Thing thing, ThingDef stuffDef, ref AcceptanceReport __result)
        {
            if (QoLModule.Instance?.IsEnabled != true) return;
            if (!__result.Accepted) return;
            if (QoLModule.ModSettings?.enableReplaceStuff != true) return;
            if (entDef == null || map == null) return;

            ThingDef effectiveStuff = stuffDef ?? thing?.Stuff;

            // When newThingDef == oldDef, ensure replacement is only allowed if the stuff is different
            // E.g. Slate Wall over Wood Wall is allowed, but Slate Wall over Slate Wall is rejected.
            foreach (IntVec3 cell in GenAdj.OccupiedRect(center, rot, entDef.Size))
            {
                if (!cell.InBounds(map)) continue;

                List<Thing> things = map.thingGrid.ThingsListAt(cell);
                for (int i = 0; i < things.Count; i++)
                {
                    Thing th = things[i];
                    if (th is Building b && !(b is Frame) && !(th is Blueprint) && !b.Destroyed && b.def == entDef && b.Stuff == effectiveStuff)
                    {
                        __result = new AcceptanceReport("IdenticalThingExists".Translate());
                        return;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Frame), nameof(Frame.CompleteConstruction))]
    public static class Patch_Frame_CompleteConstruction
    {
        public static void Prefix(Frame __instance, Verse.Pawn worker)
        {
            if (QoLModule.Instance?.IsEnabled != true) return;
            if (QoLModule.ModSettings?.enableReplaceStuff != true) return;
            if (__instance == null || !__instance.Spawned) return;

            Map map = __instance.Map;
            if (map == null) return;

            BuildableDef targetDef = __instance.def?.entityDefToBuild;
            if (targetDef == null) return;

            bool targetHoldsRoof = (targetDef as ThingDef)?.holdsRoof == true;
            HashSet<Building> destroyed = new HashSet<Building>();
            foreach (IntVec3 pos in __instance.OccupiedRect().Cells)
            {
                List<Thing> things = map.thingGrid.ThingsListAt(pos);
                for (int i = things.Count - 1; i >= 0; i--)
                {
                    if (things[i] is Building b && b != __instance && !(b is Frame) && !b.Destroyed)
                    {
                        if (Patch_ReplaceStuff.CanReplaceDefs(targetDef, b.def) && b.DeconstructibleBy(Faction.OfPlayer).Accepted && destroyed.Add(b))
                        {
                            try
                            {
                                Patch_ReplaceStuff.isReplacingBuilding = targetHoldsRoof;
                                b.Destroy(DestroyMode.Deconstruct);
                            }
                            finally
                            {
                                Patch_ReplaceStuff.isReplacingBuilding = false;
                            }
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(RoofCollapseCellsFinder), nameof(RoofCollapseCellsFinder.Notify_RoofHolderDespawned))]
    public static class Patch_RoofCollapseCellsFinder_Notify_RoofHolderDespawned
    {
        public static bool Prefix(Thing t, Map map)
        {
            if (QoLModule.Instance?.IsEnabled != true) return true;
            if (QoLModule.ModSettings?.enableReplaceStuff != true) return true;
            if (Patch_ReplaceStuff.isReplacingBuilding)
            {
                return false; // Skip collapse check during replacement
            }

            if (t != null && map != null)
            {
                foreach (Thing item in map.thingGrid.ThingsListAt(t.Position))
                {
                    if (item is Frame f && f.def?.entityDefToBuild is ThingDef td1 && td1.holdsRoof)
                    {
                        return false; // Replacement frame is present propping up the roof
                    }
                }
            }

            return true;
        }
    }
}
