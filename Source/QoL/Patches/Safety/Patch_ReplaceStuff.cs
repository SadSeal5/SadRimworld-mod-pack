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

        public static bool CanReplaceDefs(BuildableDef newDef, BuildableDef oldDef)
        {
            if (newDef == null || oldDef == null) return false;
            if (!(newDef is ThingDef newThingDef) || !(oldDef is ThingDef oldThingDef)) return false;
            if (oldThingDef.category != ThingCategory.Building) return false;

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

            // Same building class and same size
            if (newThingDef.thingClass != null && oldThingDef.thingClass != null &&
                newThingDef.thingClass == oldThingDef.thingClass &&
                newThingDef.size == oldThingDef.size)
            {
                return true;
            }

            // Same designation category & size (e.g. upgraded workstations)
            if (newThingDef.designationCategory != null &&
                newThingDef.designationCategory == oldThingDef.designationCategory &&
                newThingDef.size == oldThingDef.size)
            {
                return true;
            }

            return false;
        }

        public static void Postfix(BuildableDef newDef, ThingDef oldDef, ref bool __result)
        {
            if (__result) return;
            if (QoLModule.ModSettings?.enableReplaceStuff != true) return;

            if (CanReplaceDefs(newDef, oldDef))
            {
                __result = true;
            }
        }

        public static bool IsWall(ThingDef def)
        {
            if (def == null) return false;
            if (def.building != null && def.building.isPlaceOverableWall) return true;
            if (def.holdsRoof && def.fillPercent >= 1f && def.passability == Traversability.Impassable) return true;
            return def == ThingDefOf.Wall || (def.defName != null && def.defName.Contains("Wall"));
        }

        public static bool IsDoor(ThingDef def)
        {
            if (def == null) return false;
            if (typeof(Building_Door).IsAssignableFrom(def.thingClass)) return true;
            return def == ThingDefOf.Door || (def.defName != null && def.defName.Contains("Door"));
        }
    }

    [HarmonyPatch(typeof(GenConstruct), nameof(GenConstruct.BlocksConstruction))]
    public static class Patch_GenConstruct_BlocksConstruction
    {
        public static void Postfix(Thing constructible, Thing t, ref bool __result)
        {
            if (!__result) return;
            if (QoLModule.ModSettings?.enableReplaceStuff != true) return;
            if (constructible == null || t == null) return;

            BuildableDef newDef = constructible.def?.entityDefToBuild;
            if (newDef != null && Patch_ReplaceStuff.CanReplaceDefs(newDef, t.def))
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
            if (QoLModule.ModSettings?.enableReplaceStuff != true) return true;
            if (pawn?.Map == null || t == null) return true;

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
            if (!__result) return;
            if (QoLModule.ModSettings?.enableReplaceStuff != true) return;
            if (newEntDef == null || oldEntDef == null) return;

            BuildableDef builtNew = GenConstruct.BuiltDefOf(newEntDef as ThingDef) ?? newEntDef;
            BuildableDef builtOld = GenConstruct.BuiltDefOf(oldEntDef as ThingDef) ?? oldEntDef;

            if (Patch_ReplaceStuff.CanReplaceDefs(builtNew, builtOld))
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(GenConstruct), nameof(GenConstruct.CanPlaceBlueprintAt))]
    public static class Patch_GenConstruct_CanPlaceBlueprintAt
    {
        public static void Postfix(BuildableDef entDef, IntVec3 center, Rot4 rot, Map map, ThingDef stuffDef, ref AcceptanceReport __result)
        {
            if (!__result.Accepted) return;
            if (QoLModule.ModSettings?.enableReplaceStuff != true) return;
            if (entDef == null || map == null) return;

            Building edifice = center.GetEdifice(map);
            if (edifice != null && edifice.def == entDef)
            {
                // When newThingDef == oldDef, ensure replacement is only allowed if the stuff is different
                // E.g. Slate Wall over Wood Wall is allowed, but Slate Wall over Slate Wall is rejected.
                if (edifice.Stuff == stuffDef)
                {
                    __result = new AcceptanceReport("IdenticalThingExists".Translate());
                }
            }
        }
    }

    [HarmonyPatch(typeof(Frame), nameof(Frame.CompleteConstruction))]
    public static class Patch_Frame_CompleteConstruction
    {
        public static void Prefix(Frame __instance, Verse.Pawn worker)
        {
            if (QoLModule.ModSettings?.enableReplaceStuff != true) return;
            if (__instance == null || !__instance.Spawned) return;

            Map map = __instance.Map;
            if (map == null) return;

            HashSet<Building> destroyed = new HashSet<Building>();
            foreach (IntVec3 pos in __instance.OccupiedRect().Cells)
            {
                List<Thing> things = map.thingGrid.ThingsListAt(pos);
                for (int i = things.Count - 1; i >= 0; i--)
                {
                    if (things[i] is Building b && b != __instance && !(b is Frame) && !b.Destroyed)
                    {
                        if (Patch_ReplaceStuff.CanReplaceDefs(__instance.def?.entityDefToBuild, b.def) && destroyed.Add(b))
                        {
                            try
                            {
                                Patch_ReplaceStuff.isReplacingBuilding = true;
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
                    if (item is Blueprint bp && bp.def?.entityDefToBuild is ThingDef td2 && td2.holdsRoof)
                    {
                        return false; // Replacement blueprint is present propping up the roof
                    }
                }
            }

            return true;
        }
    }
}
