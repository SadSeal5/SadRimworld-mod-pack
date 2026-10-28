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

        public static bool CanStorageAcceptMore(SlotGroup slotGroup, Thing thing, Map map)
        {
            if (slotGroup == null || thing == null || map == null) return true;
            if (slotGroup.Settings == null || !slotGroup.Settings.AllowedToAccept(thing)) return false;

            List<IntVec3> cells = slotGroup.CellsList;
            if (cells == null || cells.Count == 0) return false;

            for (int i = 0; i < cells.Count; i++)
            {
                IntVec3 c = cells[i];
                if (!c.InBounds(map)) continue;

                int maxAllowed = GridsUtility.GetMaxItemsAllowedInCell(c, map);
                List<Thing> thingList = c.GetThingList(map);
                int currentItemCount = 0;
                bool canStack = false;

                for (int j = 0; j < thingList.Count; j++)
                {
                    Thing th = thingList[j];
                    if (th.def.category == ThingCategory.Item)
                    {
                        currentItemCount++;
                        if (th.CanStackWith(thing) && th.stackCount < th.def.stackLimit)
                        {
                            canStack = true;
                            break;
                        }
                    }
                }

                if (canStack || currentItemCount < maxAllowed)
                {
                    return true;
                }
            }

            return false;
        }

        public static void FindNearbyHaulables(Pawn pawn, IntVec3 rootPos, Thing primaryThing, IntVec3 storeCell, List<Thing> outCandidates)
        {
            if (outCandidates == null) return;
            outCandidates.Clear();

            if (pawn == null || pawn.Map == null) return;

            var settings = MegaMod.Settings?.Hauling;
            int searchRadius = settings != null ? settings.SearchRadius : 12;
            int maxItems = settings != null ? settings.MaxItemsPerTrip : 8;

            if (MassUtility.FreeSpace(pawn) <= 0.1f || maxItems <= 0)
            {
                return;
            }

            SlotGroup targetSlotGroup = storeCell.IsValid ? storeCell.GetSlotGroup(pawn.Map) : null;
            StoragePriority targetPriority = targetSlotGroup != null ? targetSlotGroup.Settings.Priority : StoragePriority.Unstored;

            // Safe search center: rootPos passed in, falling back to pawn position
            IntVec3 searchCenter = rootPos.IsValid ? rootPos : pawn.Position;

            foreach (Thing thing in GenRadial.RadialDistinctThingsAround(searchCenter, pawn.Map, searchRadius, true))
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

                // Never loot from the destination cell
                if (thing.Position == storeCell)
                {
                    continue;
                }

                // Never loot from within the destination slot group itself
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

                // Destination acceptance & capacity check
                if (targetSlotGroup != null && !CanStorageAcceptMore(targetSlotGroup, thing, pawn.Map))
                {
                    continue;
                }

                if (!pawn.CanReach(thing, PathEndMode.ClosestTouch, Danger.Some))
                {
                    continue;
                }

                if (MassUtility.CountToPickUpUntilOverEncumbered(pawn, thing) <= 0)
                {
                    continue;
                }

                outCandidates.Add(thing);
            }
        }
    }
}
