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
        private List<Thing> hauledCargo = new List<Thing>();
        private int candidateIndex = 0;

        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.Saving && pawn != null)
            {
                hauledCargo = HauledCargoTracker.GetHauledThings(pawn);
                hauledCargo.RemoveAll(t => t == null || t.Destroyed);
            }

            Scribe_Collections.Look(ref haulCandidates, "haulCandidates", LookMode.Reference);
            Scribe_Collections.Look(ref hauledCargo, "hauledCargo", LookMode.Reference);
            Scribe_Values.Look(ref candidateIndex, "candidateIndex", 0);

            haulCandidates ??= new List<Thing>();
            hauledCargo ??= new List<Thing>();

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                hauledCargo.RemoveAll(t => t == null || t.Destroyed);
                if (pawn != null)
                {
                    for (int i = 0; i < hauledCargo.Count; i++)
                    {
                        HauledCargoTracker.Register(pawn, hauledCargo[i]);
                    }
                }
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
            hauledCargo = HauledCargoTracker.GetHauledThings(pawn);
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
                bool smartHaulEnabled = HaulModule.Instance?.IsEnabled == true && (HaulModule.ModSettings?.EnableSmartHaul ?? true);
                if (!smartHaulEnabled || pawn.inventory?.innerContainer == null)
                {
                    pawn.jobs.curDriver.JumpToToil(gotoDestination);
                    return;
                }

                var settings = HaulModule.ModSettings;
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
                if (pawn.inventory?.innerContainer == null)
                {
                    pawn.jobs.curDriver.JumpToToil(gotoDestination);
                    return;
                }

                Thing candidate = job.GetTarget(HaulableInd).Thing;
                if (candidate == null || !candidate.Spawned || candidate.Destroyed || candidate.IsForbidden(pawn))
                {
                    pawn.jobs.curDriver.JumpToToil(findNextCandidate);
                    return;
                }

                float massLimitPercent = HaulModule.ModSettings?.MassLimitPercent ?? 0.95f;
                int maxPickable = HaulHelper.CountToPickUpWithinMassLimit(pawn, candidate, massLimitPercent);
                int countToTake = Mathf.Min(candidate.stackCount, maxPickable);

                if (countToTake <= 0)
                {
                    if (pawn.Map?.reservationManager?.ReservedBy(candidate, pawn, job) == true)
                    {
                        pawn.Map.reservationManager.Release(candidate, pawn, job);
                    }
                    pawn.jobs.curDriver.JumpToToil(gotoDestination);
                    return;
                }

                Thing taken = candidate.SplitOff(countToTake);
                if (candidate != taken && candidate.Spawned && !candidate.Destroyed)
                {
                    if (pawn.Map?.reservationManager?.ReservedBy(candidate, pawn, job) == true)
                    {
                        pawn.Map.reservationManager.Release(candidate, pawn, job);
                    }
                }

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
                    Thing registeredItem = (existingHauledStack != null && !taken.Spawned && !pawn.inventory.innerContainer.Contains(taken))
                        ? existingHauledStack
                        : taken;
                    HauledCargoTracker.Register(pawn, registeredItem);
                    if (!hauledCargo.Contains(registeredItem))
                    {
                        hauledCargo.Add(registeredItem);
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
                if (pawn.inventory?.innerContainer == null)
                {
                    HauledCargoTracker.Clear(pawn);
                    hauledCargo.Clear();
                    return;
                }

                if (pawn.Map == null)
                {
                    return;
                }

                List<Thing> hauledItems = HauledCargoTracker.GetHauledThings(pawn);
                if (hauledItems == null || hauledItems.Count == 0)
                {
                    HauledCargoTracker.Clear(pawn);
                    hauledCargo.Clear();
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
                        hauledCargo.Remove(item);
                        continue;
                    }

                    bool placed = false;
                    bool placedInStorage = false;
                    Thing dropped = null;

                    // 1. Try placing into cells of primary destination SlotGroup (shelves, stockpiles)
                    if (primarySlotGroup != null && primarySlotGroup.parent != null && primarySlotGroup.parent.Accepts(item))
                    {
                        List<IntVec3> cells = primarySlotGroup.CellsList;
                        for (int cIdx = 0; cIdx < cells.Count; cIdx++)
                        {
                            IntVec3 targetCell = cells[cIdx];
                            if (!targetCell.InBounds(pawn.Map) ||
                                (!targetCell.AdjacentTo8WayOrInside(pawn.Position) && targetCell.DistanceToSquared(primaryStoreCell) > 144))
                            {
                                continue;
                            }

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
                                    if (th.CanStackWith(item) && th.stackCount + item.stackCount <= th.def.stackLimit)
                                    {
                                        canStack = true;
                                    }
                                }
                            }

                            if (canStack || currentItemCount < maxAllowed)
                            {
                                if (pawn.inventory.innerContainer.TryDrop(item, targetCell, pawn.Map, ThingPlaceMode.Direct, out dropped))
                                {
                                    placed = true;
                                    placedInStorage = true;
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
                                if (pawn.inventory.innerContainer.TryDrop(item, altCell, pawn.Map, ThingPlaceMode.Direct, out dropped))
                                {
                                    placed = true;
                                    placedInStorage = true;
                                    SlotGroup altSlot = altCell.GetSlotGroup(pawn.Map);
                                    if (altSlot?.parent is Building_Storage altStorage)
                                    {
                                        altStorage.Notify_ReceivedThing(dropped ?? item);
                                    }
                                }
                            }
                        }
                    }

                    // 3. Fallback: drop near current position if storage full to prevent cargo from being trapped
                    if (!placed)
                    {
                        if (pawn.inventory.innerContainer.TryDrop(item, pawn.Position, pawn.Map, ThingPlaceMode.Near, out Thing _))
                        {
                            placed = true;
                        }
                    }

                    if (placedInStorage)
                    {
                        DesignationDef urgentDef = DefDatabase<DesignationDef>.GetNamedSilentFail("SadHaul_HaulUrgently");
                        if (urgentDef != null && pawn.Map?.designationManager != null)
                        {
                            Thing targetDes = dropped ?? item;
                            Designation des = pawn.Map.designationManager.DesignationOn(targetDes, urgentDef);
                            if (des != null) pawn.Map.designationManager.RemoveDesignation(des);
                        }
                    }

                    HauledCargoTracker.Unregister(pawn, item);
                    hauledCargo.Remove(item);
                }

                HauledCargoTracker.Clear(pawn);
                hauledCargo.Clear();
            };

            return unloadToil;
        }
    }
}
