using System.Collections.Generic;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Durability
{
    public static class DurabilityApplier
    {
        public static bool IsProtected(Thing thing)
        {
            if (DurabilityModule.Instance?.IsEnabled != true) return false;
            var settings = DurabilityModule.ModSettings;
            if (settings == null || thing?.def == null || !thing.def.useHitPoints) return false;

            return (thing.def.IsApparel && settings.removeApparelDurability)
                || (thing.def.IsWeapon && settings.removeWeaponDurability);
        }

        public static void ApplyDurabilitySettings()
        {
            // No-op: protection is enforced via Harmony patches without mutating ThingDef.useHitPoints.
        }

        public static void RestoreDefDefaults()
        {
            // No-op: ThingDef.useHitPoints is never mutated.
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
                if (item == null || item.def == null || !item.def.useHitPoints || processed.Contains(item)) return;
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

            void TryRestorePawnGear(Pawn pawn)
            {
                if (pawn == null) return;

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

            if (Find.Maps != null)
            {
                foreach (Map map in Find.Maps)
                {
                    if (map.listerThings?.AllThings != null)
                    {
                        var allThings = map.listerThings.AllThings;
                        for (int i = 0; i < allThings.Count; i++)
                        {
                            Thing thing = allThings[i];
                            TryRestore(thing);
                            if (thing is Corpse corpse && corpse.InnerPawn != null)
                            {
                                TryRestorePawnGear(corpse.InnerPawn);
                            }
                        }
                    }

                    if (map.mapPawns?.AllPawns != null)
                    {
                        var pawns = map.mapPawns.AllPawns;
                        for (int i = 0; i < pawns.Count; i++)
                        {
                            TryRestorePawnGear(pawns[i]);
                        }
                    }
                }
            }

            if (Find.WorldPawns?.AllPawnsAlive != null)
            {
                var worldPawns = Find.WorldPawns.AllPawnsAlive;
                for (int i = 0; i < worldPawns.Count; i++)
                {
                    TryRestorePawnGear(worldPawns[i]);
                }
            }

            Messages.Message("MegaMod_RestoredGearCount".Translate(restoredCount), MessageTypeDefOf.PositiveEvent, false);
        }
    }
}
