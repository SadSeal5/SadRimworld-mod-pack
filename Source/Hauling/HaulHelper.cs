using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.Hauling
{
    public static class HaulHelper
    {
        public static bool Accepts(this ISlotGroupParent parent, Thing thing)
        {
            if (parent == null || thing == null) return false;
            var settings = parent.GetSlotGroup()?.Settings;
            return settings != null && settings.AllowedToAccept(thing);
        }

        public static void FindNearbyHaulables(Pawn pawn, Thing primaryThing, IntVec3 storeCell, List<Thing> outCandidates)
        {
            if (outCandidates == null) return;
            outCandidates.Clear();

            if (pawn == null || primaryThing == null || pawn.Map == null) return;

            var settings = MegaMod.Settings?.Hauling;
            int searchRadius = settings != null ? settings.SearchRadius : 12;
            int maxItems = settings != null ? settings.MaxItemsPerTrip : 8;
            bool strictClustering = settings == null || settings.StrictClustering;

            if (MassUtility.FreeSpace(pawn) <= 0.1f || maxItems <= 0)
            {
                return;
            }

            SlotGroup targetSlotGroup = strictClustering && storeCell.IsValid ? storeCell.GetSlotGroup(pawn.Map) : null;

            foreach (Thing thing in GenRadial.RadialDistinctThingsAround(primaryThing.Position, pawn.Map, searchRadius, true))
            {
                if (outCandidates.Count >= maxItems || MassUtility.FreeSpace(pawn) <= 0.1f)
                {
                    break;
                }

                if (thing == null || !thing.Spawned || thing.Destroyed || thing == primaryThing)
                {
                    continue;
                }

                if (thing.def == null || thing.def.category != ThingCategory.Item || !thing.def.EverHaulable)
                {
                    continue;
                }

                if (thing.IsForbidden(pawn) || thing.IsBurning())
                {
                    continue;
                }

                if (!pawn.CanReserve(thing, 1, -1, null, false))
                {
                    continue;
                }

                if (strictClustering && targetSlotGroup != null)
                {
                    if (!targetSlotGroup.parent.Accepts(thing))
                    {
                        continue;
                    }
                }

                if (!pawn.CanReach(thing, PathEndMode.Touch, Danger.Some))
                {
                    continue;
                }

                outCandidates.Add(thing);
            }
        }
    }
}
