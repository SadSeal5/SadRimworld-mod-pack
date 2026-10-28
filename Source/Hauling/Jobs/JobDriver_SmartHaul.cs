using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.Hauling.Jobs
{
    public class JobDriver_SmartHaul : JobDriver
    {
        private const TargetIndex HaulableInd = TargetIndex.A;
        private const TargetIndex StoreCellInd = TargetIndex.B;

        private List<Thing> haulCandidates = new List<Thing>();
        private int candidateIndex = 0;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref haulCandidates, "haulCandidates", LookMode.Reference);
            Scribe_Values.Look(ref candidateIndex, "candidateIndex", 0);
            if (haulCandidates == null)
            {
                haulCandidates = new List<Thing>();
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(HaulableInd), job, 1, -1, null, errorOnFailed) &&
                   pawn.Reserve(job.GetTarget(StoreCellInd), job, 1, -1, null, errorOnFailed);
        }

        public virtual void Notify_PawnJobEnded()
        {
            HauledCargoTracker.DropAllHauled(pawn);
        }

        public override string GetReport()
        {
            Thing primary = (pawn.CurJob == job && pawn.carryTracker.CarriedThing != null)
                ? pawn.carryTracker.CarriedThing
                : ((TargetThingA != null && TargetThingA.Spawned) ? TargetThingA : null);

            if (primary == null)
            {
                return "ReportHaulingUnknown".Translate();
            }

            string destLabel = null;
            if (job.targetB.IsValid)
            {
                SlotGroup slotGroup = job.targetB.Cell.GetSlotGroup(Map);
                if (slotGroup != null && slotGroup.parent != null)
                {
                    destLabel = slotGroup.parent.SlotYielderLabel();
                }
            }

            if (!string.IsNullOrEmpty(destLabel))
            {
                return "ReportHaulingTo".Translate(primary.Named("THING"), destLabel.Named("DESTINATION"));
            }

            return "ReportHauling".Translate(primary.Label, primary);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            AddFinishAction(condition =>
            {
                if (condition != JobCondition.Succeeded)
                {
                    Notify_PawnJobEnded();
                }
            });

            // Toil 1: Goto primary thing
            Toil gotoPrimary = Toils_Goto.GotoThing(HaulableInd, PathEndMode.ClosestTouch);
            gotoPrimary.FailOnDespawnedNullOrForbidden(HaulableInd);
            yield return gotoPrimary;

            // Toil 2: Pick up primary thing into carryTracker
            Toil pickupPrimary = ToilMaker.MakeToil("PickupPrimary");
            pickupPrimary.initAction = () =>
            {
                Thing primary = job.GetTarget(HaulableInd).Thing;
                if (primary == null || !primary.Spawned)
                {
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                int count = Mathf.Min(job.count, primary.stackCount);
                if (count <= 0) count = primary.stackCount;
                pawn.carryTracker.TryStartCarry(primary, count);
                if (pawn.carryTracker.CarriedThing == null)
                {
                    EndJobWith(JobCondition.Incompletable);
                }
            };
            yield return pickupPrimary;

            // --- Multi-item gathering loop ---
            Toil findNextCandidate = ToilMaker.MakeToil("FindNextCandidate");
            Toil gotoCandidate = Toils_Goto.GotoThing(HaulableInd, PathEndMode.ClosestTouch);
            Toil pickupCandidate = ToilMaker.MakeToil("PickupCandidate");
            Toil gotoDestination = Toils_Goto.GotoCell(StoreCellInd, PathEndMode.ClosestTouch);

            findNextCandidate.initAction = () =>
            {
                bool smartHaulEnabled = MegaMod.Settings?.Hauling?.EnableSmartHaul ?? true;
                if (!smartHaulEnabled)
                {
                    pawn.jobs.curDriver.JumpToToil(gotoDestination);
                    return;
                }

                var settings = MegaMod.Settings?.Hauling;
                float maxMassRatio = settings != null ? settings.MassLimitPercent : 0.95f;
                float capacity = MassUtility.Capacity(pawn);
                float currentMass = MassUtility.GearAndInventoryMass(pawn);
                int maxItems = settings != null ? settings.MaxItemsPerTrip : 8;

                if (MassUtility.FreeSpace(pawn) < 0.1f ||
                    currentMass >= capacity * maxMassRatio ||
                    HauledCargoTracker.GetHauledThings(pawn).Count >= maxItems)
                {
                    pawn.jobs.curDriver.JumpToToil(gotoDestination);
                    return;
                }

                haulCandidates.Clear();
                HaulHelper.FindNearbyHaulables(pawn, pawn.Position, pawn.carryTracker.CarriedThing, job.GetTarget(StoreCellInd).Cell, haulCandidates);

                Thing nextCandidate = null;
                for (int i = 0; i < haulCandidates.Count; i++)
                {
                    Thing c = haulCandidates[i];
                    if (c != null && c.Spawned && !c.Destroyed && !c.IsForbidden(pawn) &&
                        pawn.CanReserve(c, 1, -1, null, false) &&
                        pawn.CanReach(c, PathEndMode.ClosestTouch, Danger.Some))
                    {
                        if (pawn.Reserve(c, job, 1, -1, null, false))
                        {
                            nextCandidate = c;
                            break;
                        }
                    }
                }

                if (nextCandidate != null)
                {
                    job.SetTarget(HaulableInd, nextCandidate);
                    pawn.jobs.curDriver.JumpToToil(gotoCandidate);
                }
                else
                {
                    pawn.jobs.curDriver.JumpToToil(gotoDestination);
                }
            };
            yield return findNextCandidate;

            gotoCandidate.FailOnDespawnedNullOrForbidden(HaulableInd);
            yield return gotoCandidate;

            pickupCandidate.initAction = () =>
            {
                Thing candidate = job.GetTarget(HaulableInd).Thing;
                if (candidate == null || !candidate.Spawned || candidate.Destroyed || candidate.IsForbidden(pawn))
                {
                    pawn.jobs.curDriver.JumpToToil(findNextCandidate);
                    return;
                }

                int maxPickable = MassUtility.CountToPickUpUntilOverEncumbered(pawn, candidate);
                int countToTake = Mathf.Min(candidate.stackCount, maxPickable);

                if (countToTake <= 0)
                {
                    pawn.jobs.curDriver.JumpToToil(gotoDestination);
                    return;
                }

                Thing taken = candidate.SplitOff(countToTake);

                bool hasPersonalStack = false;
                Thing existingHauledStack = null;
                for (int i = 0; i < pawn.inventory.innerContainer.Count; i++)
                {
                    Thing invThing = pawn.inventory.innerContainer[i];
                    if (invThing != null && invThing.CanStackWith(taken))
                    {
                        if (HauledCargoTracker.IsHauled(pawn, invThing))
                        {
                            existingHauledStack = invThing;
                        }
                        else
                        {
                            hasPersonalStack = true;
                        }
                    }
                }

                bool added = pawn.inventory.innerContainer.TryAdd(taken, !hasPersonalStack);
                if (added)
                {
                    if (existingHauledStack != null && !taken.Spawned && !pawn.inventory.innerContainer.Contains(taken))
                    {
                        HauledCargoTracker.Register(pawn, existingHauledStack);
                    }
                    else
                    {
                        HauledCargoTracker.Register(pawn, taken);
                    }
                }
                else
                {
                    if (!taken.Spawned)
                    {
                        GenPlace.TryPlaceThing(taken, pawn.Position, pawn.Map, ThingPlaceMode.Near);
                    }
                    pawn.jobs.curDriver.JumpToToil(gotoDestination);
                    return;
                }

                pawn.jobs.curDriver.JumpToToil(findNextCandidate);
            };
            yield return pickupCandidate;

            // Destination Toil: Goto destination storage cell
            yield return gotoDestination;

            // Deposit carried primary thing into storage cell
            yield return Toils_Haul.PlaceHauledThingInCell(StoreCellInd, null, true, true);

            // Batch unload all gathered cargo from inventory
            yield return CreateBatchUnloadToil();
        }

        private Toil CreateBatchUnloadToil()
        {
            Toil unloadToil = ToilMaker.MakeToil("BatchUnloadHauled");
            unloadToil.initAction = () =>
            {
                List<Thing> hauledItems = HauledCargoTracker.GetHauledThings(pawn);
                if (hauledItems == null || hauledItems.Count == 0)
                {
                    HauledCargoTracker.Clear(pawn);
                    return;
                }

                IntVec3 primaryStoreCell = job.GetTarget(StoreCellInd).Cell;
                SlotGroup primarySlotGroup = primaryStoreCell.IsValid ? primaryStoreCell.GetSlotGroup(pawn.Map) : null;

                for (int i = 0; i < hauledItems.Count; i++)
                {
                    Thing item = hauledItems[i];
                    if (item == null || item.Destroyed || !pawn.inventory.innerContainer.Contains(item))
                    {
                        HauledCargoTracker.Unregister(pawn, item);
                        continue;
                    }

                    bool placed = false;

                    // 1. Try placing into cells of primary destination SlotGroup (shelves, stockpiles)
                    if (primarySlotGroup != null && primarySlotGroup.parent != null && primarySlotGroup.parent.Accepts(item))
                    {
                        List<IntVec3> cells = primarySlotGroup.CellsList;
                        for (int cIdx = 0; cIdx < cells.Count; cIdx++)
                        {
                            IntVec3 targetCell = cells[cIdx];
                            if (!targetCell.InBounds(pawn.Map)) continue;

                            int maxAllowed = GridsUtility.GetMaxItemsAllowedInCell(targetCell, pawn.Map);
                            List<Thing> thingsInCell = targetCell.GetThingList(pawn.Map);
                            int currentItemCount = 0;
                            bool canStack = false;

                            for (int t = 0; t < thingsInCell.Count; t++)
                            {
                                Thing th = thingsInCell[t];
                                if (th.def.category == ThingCategory.Item)
                                {
                                    currentItemCount++;
                                    if (th.CanStackWith(item) && th.stackCount < th.def.stackLimit)
                                    {
                                        canStack = true;
                                    }
                                }
                            }

                            if (canStack || currentItemCount < maxAllowed)
                            {
                                if (pawn.inventory.innerContainer.TryDrop(item, targetCell, pawn.Map, ThingPlaceMode.Direct, out Thing dropped))
                                {
                                    placed = true;
                                    if (primarySlotGroup.parent is Building_Storage bStorage)
                                    {
                                        bStorage.Notify_ReceivedThing(dropped ?? item);
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    // 2. If primary storage is full, search for an adjacent valid store cell
                    if (!placed)
                    {
                        if (StoreUtility.TryFindBestBetterStoreCellFor(item, pawn, pawn.Map, StoragePriority.Unstored, pawn.Faction, out IntVec3 altCell, true))
                        {
                            if (altCell.AdjacentTo8WayOrInside(pawn.Position))
                            {
                                if (pawn.inventory.innerContainer.TryDrop(item, altCell, pawn.Map, ThingPlaceMode.Direct, out Thing dropped))
                                {
                                    placed = true;
                                    SlotGroup altSlot = altCell.GetSlotGroup(pawn.Map);
                                    if (altSlot?.parent is Building_Storage altStorage)
                                    {
                                        altStorage.Notify_ReceivedThing(dropped ?? item);
                                    }
                                }
                            }
                        }
                    }

                    // 3. If still not placed, keep safely in inventory to prevent infinite drop/haul loops
                    HauledCargoTracker.Unregister(pawn, item);
                }

                HauledCargoTracker.Clear(pawn);
            };

            return unloadToil;
        }
    }
}
