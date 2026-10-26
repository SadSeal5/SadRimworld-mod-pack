using System.Collections.Generic;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Durability
{
    [StaticConstructorOnStartup]
    public static class DurabilityApplier
    {
        static DurabilityApplier()
        {
            ApplyDurabilitySettings();
        }

        public static void ApplyDurabilitySettings()
        {
            var settings = MegaMod.Settings?.Durability;
            if (settings == null) return;

            var allDefs = DefDatabase<ThingDef>.AllDefsListForReading;
            for (int i = 0; i < allDefs.Count; i++)
            {
                ThingDef def = allDefs[i];
                if (def == null) continue;

                if (def.IsApparel)
                {
                    def.useHitPoints = !settings.removeApparelDurability;
                }
                if (def.IsWeapon)
                {
                    def.useHitPoints = !settings.removeWeaponDurability;
                }
            }
        }

        public static void RestoreAllDamagedItemsInWorld()
        {
            if (Current.ProgramState != ProgramState.Playing)
            {
                return;
            }

            int restoredCount = 0;
            HashSet<Thing> processed = new HashSet<Thing>();

            void TryRestore(Thing item)
            {
                if (item == null || item.def == null || processed.Contains(item)) return;
                processed.Add(item);

                if (item.def.IsWeapon || item.def.IsApparel)
                {
                    if (item.HitPoints < item.MaxHitPoints)
                    {
                        item.HitPoints = item.MaxHitPoints;
                        restoredCount++;
                    }
                }
            }

            if (Find.Maps != null)
            {
                foreach (Map map in Find.Maps)
                {
                    if (map.listerThings?.AllThings != null)
                    {
                        var allThings = map.listerThings.AllThings;
                        for (int i = 0; i < allThings.Count; i++)
                        {
                            TryRestore(allThings[i]);
                        }
                    }

                    if (map.mapPawns?.AllPawns != null)
                    {
                        var pawns = map.mapPawns.AllPawns;
                        for (int i = 0; i < pawns.Count; i++)
                        {
                            Pawn pawn = pawns[i];
                            if (pawn == null) continue;

                            if (pawn.apparel?.WornApparel != null)
                            {
                                var worn = pawn.apparel.WornApparel;
                                for (int a = 0; a < worn.Count; a++)
                                {
                                    TryRestore(worn[a]);
                                }
                            }

                            if (pawn.equipment?.AllEquipmentListForReading != null)
                            {
                                var eq = pawn.equipment.AllEquipmentListForReading;
                                for (int e = 0; e < eq.Count; e++)
                                {
                                    TryRestore(eq[e]);
                                }
                            }

                            if (pawn.inventory?.innerContainer != null)
                            {
                                var inv = pawn.inventory.innerContainer;
                                for (int v = 0; v < inv.Count; v++)
                                {
                                    TryRestore(inv[v]);
                                }
                            }
                        }
                    }
                }
            }

            if (Find.WorldPawns?.AllPawnsAlive != null)
            {
                var worldPawns = Find.WorldPawns.AllPawnsAlive;
                for (int i = 0; i < worldPawns.Count; i++)
                {
                    Pawn pawn = worldPawns[i];
                    if (pawn == null) continue;

                    if (pawn.apparel?.WornApparel != null)
                    {
                        var worn = pawn.apparel.WornApparel;
                        for (int a = 0; a < worn.Count; a++)
                        {
                            TryRestore(worn[a]);
                        }
                    }

                    if (pawn.equipment?.AllEquipmentListForReading != null)
                    {
                        var eq = pawn.equipment.AllEquipmentListForReading;
                        for (int e = 0; e < eq.Count; e++)
                        {
                            TryRestore(eq[e]);
                        }
                    }

                    if (pawn.inventory?.innerContainer != null)
                    {
                        var inv = pawn.inventory.innerContainer;
                        for (int v = 0; v < inv.Count; v++)
                        {
                            TryRestore(inv[v]);
                        }
                    }
                }
            }

            Messages.Message("MegaMod_RestoredGearCount".Translate(restoredCount), MessageTypeDefOf.PositiveEvent, false);
        }
    }
}
