using System.Collections.Generic;
using RimWorld;
using UnityEngine;
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

            SlotGroup targetSlotGroup = storeCell.IsValid ? storeCell.GetSlotGroup(pawn.Map) : null;
            StoragePriority targetPriority = targetSlotGroup != null ? targetSlotGroup.Settings.Priority : StoragePriority.Unstored;

            // Calculate destination remaining capacity to avoid over-collecting and shelf overflow
            int destinationRemainingSlots = int.MaxValue;
            if (targetSlotGroup != null)
            {
                destinationRemainingSlots = 0;
                List<IntVec3> cells = targetSlotGroup.CellsList;
                for (int i = 0; i < cells.Count; i++)
                {
                    IntVec3 c = cells[i];
                    int maxAllowed = GridsUtility.GetMaxItemsAllowedInCell(c, pawn.Map);
                    List<Thing> thingList = c.GetThingList(pawn.Map);
                    int currentItemCount = 0;
                    for (int j = 0; j < thingList.Count; j++)
                    {
                        if (thingList[j].def.category == ThingCategory.Item)
                        {
                            currentItemCount++;
                        }
                    }
                    destinationRemainingSlots += Mathf.Max(0, maxAllowed - currentItemCount);
                }

                // The primary carried item will occupy 1 slot
                destinationRemainingSlots = Mathf.Max(0, destinationRemainingSlots - 1);
                if (destinationRemainingSlots <= 0)
                {
                    // Target storage has no extra capacity for gathered items
                    return;
                }
            }

            int effectiveLimit = Mathf.Min(maxItems, destinationRemainingSlots);

            foreach (Thing thing in GenRadial.RadialDistinctThingsAround(primaryThing.Position, pawn.Map, searchRadius, true))
            {
                if (outCandidates.Count >= effectiveLimit || MassUtility.FreeSpace(pawn) <= 0.1f)
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

                // Never loot from the destination cell or destination storage unit
                if (thing.Position == storeCell)
                {
                    continue;
                }
                if (targetSlotGroup != null && targetSlotGroup.CellsList.Contains(thing.Position))
                {
                    continue;
                }

                // Never loot from valid storage if the item is already stored with equal or higher priority
                if (StoreUtility.IsInValidStorage(thing))
                {
                    if (StoreUtility.TryGetValidStoragePriority(thing, out StoragePriority currentPriority))
                    {
                        if (targetSlotGroup != null)
                        {
                            if ((int)currentPriority >= (int)targetPriority)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                if (!pawn.CanReserve(thing, 1, -1, null, false))
                {
                    continue;
                }

                // Check destination acceptance
                if (targetSlotGroup != null && targetSlotGroup.parent != null)
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
