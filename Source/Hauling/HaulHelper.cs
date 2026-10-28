using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.Hauling
{
    public class StorageSlotSimulator
    {
        private class SimulatedCell
        {
            public IntVec3 Cell;
            public int MaxAllowed;
            public int ItemCount;
            public List<SimulatedStack> Stacks = new List<SimulatedStack>();
        }

        private class SimulatedStack
        {
            public ThingDef Def;
            public int RemainingSpace;
        }

        private readonly List<SimulatedCell> cells = new List<SimulatedCell>();
        private readonly StorageSettings settings;

        public StorageSlotSimulator(SlotGroup slotGroup, Map map)
        {
            if (slotGroup == null || map == null) return;
            this.settings = slotGroup.Settings;

            List<IntVec3> cellList = slotGroup.CellsList;
            if (cellList == null) return;

            for (int i = 0; i < cellList.Count; i++)
            {
                IntVec3 c = cellList[i];
                if (!c.InBounds(map)) continue;

                var simCell = new SimulatedCell
                {
                    Cell = c,
                    MaxAllowed = GridsUtility.GetMaxItemsAllowedInCell(c, map),
                    ItemCount = 0
                };

                List<Thing> thingList = c.GetThingList(map);
                for (int j = 0; j < thingList.Count; j++)
                {
                    Thing th = thingList[j];
                    if (th.def.category == ThingCategory.Item)
                    {
                        simCell.ItemCount++;
                        int spaceLeft = th.def.stackLimit - th.stackCount;
                        if (spaceLeft > 0)
                        {
                            simCell.Stacks.Add(new SimulatedStack
                            {
                                Def = th.def,
                                RemainingSpace = spaceLeft
                            });
                        }
                    }
                }

                cells.Add(simCell);
            }
        }

        public bool HasAnyCapacityLeft()
        {
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].ItemCount < cells[i].MaxAllowed || cells[i].Stacks.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool TrySimulateAdd(Thing thing)
        {
            if (thing == null || thing.def == null) return false;
            if (settings != null && !settings.AllowedToAccept(thing)) return false;

            int needed = thing.stackCount;

            // 1. First try to merge with existing simulated partial stacks
            for (int i = 0; i < cells.Count; i++)
            {
                var c = cells[i];
                for (int s = 0; s < c.Stacks.Count; s++)
                {
                    var st = c.Stacks[s];
                    if (st.Def == thing.def && st.RemainingSpace > 0)
                    {
                        int take = Mathf.Min(needed, st.RemainingSpace);
                        st.RemainingSpace -= take;
                        needed -= take;
                        if (needed <= 0)
                        {
                            return true;
                        }
                    }
                }
            }

            // 2. If remaining amount needs a new slot, check if any cell has empty slots
            for (int i = 0; i < cells.Count; i++)
            {
                var c = cells[i];
                if (c.ItemCount < c.MaxAllowed)
                {
                    c.ItemCount++;
                    int spaceLeftInNewStack = thing.def.stackLimit - needed;
                    if (spaceLeftInNewStack > 0)
                    {
                        c.Stacks.Add(new SimulatedStack
                        {
                            Def = thing.def,
                            RemainingSpace = spaceLeftInNewStack
                        });
                    }
                    return true;
                }
            }

            return false;
        }
    }

    public static class HaulHelper
    {
        public static bool Accepts(this ISlotGroupParent parent, Thing thing)
        {
            if (parent == null || thing == null) return false;
            var settings = parent.GetSlotGroup()?.Settings;
            return settings != null && settings.AllowedToAccept(thing);
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

            // Initialize destination capacity simulator
            StorageSlotSimulator sim = null;
            if (targetSlotGroup != null)
            {
                sim = new StorageSlotSimulator(targetSlotGroup, pawn.Map);

                // Reserve destination capacity for the primary carried item first!
                if (primaryThing != null)
                {
                    sim.TrySimulateAdd(primaryThing);
                }

                // If destination is completely full after primary item, do not collect extra items
                if (!sim.HasAnyCapacityLeft())
                {
                    return;
                }
            }

            // Safe search center: rootPos passed in, falling back to pawn position
            IntVec3 searchCenter = rootPos.IsValid ? rootPos : pawn.Position;

            foreach (Thing thing in GenRadial.RadialDistinctThingsAround(searchCenter, pawn.Map, searchRadius, true))
            {
                if (outCandidates.Count >= maxItems || MassUtility.FreeSpace(pawn) <= 0.1f)
                {
                    break;
                }

                if (sim != null && !sim.HasAnyCapacityLeft())
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

                // Check destination acceptance & capacity simulation
                if (sim != null)
                {
                    if (!sim.TrySimulateAdd(thing))
                    {
                        continue;
                    }
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
