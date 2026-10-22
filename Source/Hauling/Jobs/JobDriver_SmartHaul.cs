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
            Thing primary = TargetThingA ?? pawn.carryTracker.CarriedThing;
            if (primary != null)
            {
                return "ReportHauling".Translate(primary.LabelCap, job.targetB.Cell.ToString());
            }
            return base.GetReport();
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

                pawn.carryTracker.TryStartCarry(primary, primary.stackCount);
                if (pawn.carryTracker.CarriedThing == null)
                {
                    EndJobWith(JobCondition.Incompletable);
                }
            };
            yield return pickupPrimary;

            // Toil 3: Dynamic gathering toil
            yield return CreateGatheringToil();

            // Toil 4: Goto destination storage cell
            yield return Toils_Goto.GotoCell(StoreCellInd, PathEndMode.ClosestTouch);

            // Toil 5: Deposit carried primary thing into storage cell
            yield return Toils_Haul.PlaceHauledThingInCell(StoreCellInd, null, true, true);

            // Toil 6: Batch unload all gathered cargo from inventory
            yield return CreateBatchUnloadToil();
        }

        private Toil CreateGatheringToil()
        {
            Toil gatherToil = ToilMaker.MakeToil("DynamicGatherNearby");
            gatherToil.defaultCompleteMode = ToilCompleteMode.Never;

            gatherToil.initAction = () =>
            {
                haulCandidates.Clear();
                candidateIndex = 0;

                bool smartHaulEnabled = MegaMod.Settings?.Hauling?.EnableSmartHaul ?? true;
                if (smartHaulEnabled)
                {
                    Thing primary = pawn.carryTracker.CarriedThing ?? job.GetTarget(HaulableInd).Thing;
                    if (primary != null)
                    {
                        HaulHelper.FindNearbyHaulables(pawn, primary, job.GetTarget(StoreCellInd).Cell, haulCandidates);
                    }
                }

                if (haulCandidates.Count == 0)
                {
                    ReadyForNextToil();
                    return;
                }

                StartNextCandidatePath();
            };

            gatherToil.tickAction = () =>
            {
                var settings = MegaMod.Settings?.Hauling;
                float maxMassRatio = settings != null ? settings.MassLimitPercent : 0.95f;
                float capacity = MassUtility.Capacity(pawn);
                float currentMass = MassUtility.GearAndInventoryMass(pawn);

                if (MassUtility.FreeSpace(pawn) < 0.1f ||
                    currentMass >= capacity * maxMassRatio ||
                    HauledCargoTracker.GetHauledThings(pawn).Count >= (settings != null ? settings.MaxItemsPerTrip : 8) ||
                    candidateIndex >= haulCandidates.Count)
                {
                    ReadyForNextToil();
                    return;
                }

                Thing candidate = haulCandidates[candidateIndex];
                if (candidate == null || !candidate.Spawned || candidate.Destroyed || candidate.IsForbidden(pawn))
                {
                    AdvanceCandidate();
                    return;
                }

                if (pawn.pather.Moving)
                {
                    return;
                }

                if (pawn.Position.AdjacentTo8WayOrInside(candidate))
                {
                    int maxPickable = MassUtility.CountToPickUpUntilOverEncumbered(pawn, candidate);
                    int countToTake = Mathf.Min(candidate.stackCount, maxPickable);

                    if (countToTake <= 0)
                    {
                        ReadyForNextToil();
                        return;
                    }

                    Thing taken = (countToTake < candidate.stackCount) ? candidate.SplitOff(countToTake) : candidate;
                    if (pawn.inventory.innerContainer.TryAddOrTransfer(taken, false))
                    {
                        HauledCargoTracker.Register(pawn, taken);
                    }
                    else
                    {
                        if (taken != candidate && !taken.Spawned)
                        {
                            GenPlace.TryPlaceThing(taken, pawn.Position, pawn.Map, ThingPlaceMode.Near);
                        }
                        ReadyForNextToil();
                        return;
                    }

                    AdvanceCandidate();
                }
                else
                {
                    AdvanceCandidate();
                }
            };

            return gatherToil;
        }

        private void AdvanceCandidate()
        {
            candidateIndex++;

            var settings = MegaMod.Settings?.Hauling;
            float maxMassRatio = settings != null ? settings.MassLimitPercent : 0.95f;
            float capacity = MassUtility.Capacity(pawn);
            float currentMass = MassUtility.GearAndInventoryMass(pawn);

            if (candidateIndex >= haulCandidates.Count ||
                MassUtility.FreeSpace(pawn) < 0.1f ||
                currentMass >= capacity * maxMassRatio ||
                HauledCargoTracker.GetHauledThings(pawn).Count >= (settings != null ? settings.MaxItemsPerTrip : 8))
            {
                ReadyForNextToil();
                return;
            }

            StartNextCandidatePath();
        }

        private void StartNextCandidatePath()
        {
            while (candidateIndex < haulCandidates.Count)
            {
                Thing candidate = haulCandidates[candidateIndex];
                if (candidate != null && candidate.Spawned && !candidate.Destroyed && !candidate.IsForbidden(pawn) &&
                    pawn.CanReach(candidate, PathEndMode.Touch, Danger.Some))
                {
                    if (pawn.Reserve(candidate, job, 1, -1, null, false))
                    {
                        pawn.pather.StartPath(candidate, PathEndMode.Touch);
                        return;
                    }
                }
                candidateIndex++;
            }

            ReadyForNextToil();
        }

        private Toil CreateBatchUnloadToil()
        {
            Toil unloadToil = ToilMaker.MakeToil("BatchUnloadHauled");
            unloadToil.initAction = () =>
            {
                List<Thing> hauledItems = HauledCargoTracker.GetHauledThings(pawn);
                for (int i = 0; i < hauledItems.Count; i++)
                {
                    Thing item = hauledItems[i];
                    if (item == null || item.Destroyed || !pawn.inventory.innerContainer.Contains(item))
                    {
                        HauledCargoTracker.Unregister(pawn, item);
                        continue;
                    }

                    Thing dropped;
                    if (StoreUtility.TryFindBestBetterStoreCellFor(item, pawn, pawn.Map, StoragePriority.Unstored, pawn.Faction, out IntVec3 cell, true))
                    {
                        if (!pawn.inventory.innerContainer.TryDrop(item, cell, pawn.Map, ThingPlaceMode.Direct, out dropped))
                        {
                            pawn.inventory.innerContainer.TryDrop(item, pawn.Position, pawn.Map, ThingPlaceMode.Near, out dropped);
                        }
                    }
                    else
                    {
                        pawn.inventory.innerContainer.TryDrop(item, pawn.Position, pawn.Map, ThingPlaceMode.Near, out dropped);
                    }

                    HauledCargoTracker.Unregister(pawn, item);
                }

                HauledCargoTracker.Clear(pawn);
            };

            return unloadToil;
        }
    }
}
