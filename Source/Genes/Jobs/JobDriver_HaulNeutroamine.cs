using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.Genes.Jobs
{
    public class JobDriver_HaulNeutroamine : JobDriver
    {
        private const TargetIndex AssemblerIndex = TargetIndex.A;
        private const TargetIndex NeutroamineIndex = TargetIndex.B;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (!pawn.Reserve(job.GetTarget(AssemblerIndex), job, 1, -1, null, errorOnFailed))
            {
                return false;
            }
            if (!pawn.Reserve(job.GetTarget(NeutroamineIndex), job, 10, job.count, null, errorOnFailed))
            {
                return false;
            }
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(AssemblerIndex);
            this.FailOnBurningImmobile(AssemblerIndex);
            this.FailOnDespawnedNullOrForbidden(NeutroamineIndex);
            this.FailOn(() =>
            {
                Thing building = job.GetTarget(AssemblerIndex).Thing;
                if (building == null) return true;
                CompGeneSynthesizer comp = building.TryGetComp<CompGeneSynthesizer>();
                if (comp == null || !comp.IsSynthesizing || comp.CurrentOrder == null) return true;
                ThingDef resourceDef = job.GetTarget(NeutroamineIndex).Thing?.def ?? pawn.carryTracker?.CarriedThing?.def;
                bool isArchite = resourceDef != null && resourceDef == CompGeneSynthesizer.ArchiteDef;
                int needed = isArchite ? comp.ArchiteNeeded : comp.NeutroamineNeeded;
                return needed <= 0;
            });

            yield return Toils_Goto.GotoThing(NeutroamineIndex, PathEndMode.ClosestTouch)
                .FailOnSomeonePhysicallyInteracting(NeutroamineIndex);

            yield return Toils_Haul.StartCarryThing(NeutroamineIndex, putRemainderInQueue: false, subtractNumTakenFromJobCount: true, failIfStackCountLessThanJobCount: false);

            yield return Toils_Goto.GotoThing(AssemblerIndex, PathEndMode.Touch);

            Toil deposit = ToilMaker.MakeToil("DepositNeutroamine");
            deposit.initAction = () =>
            {
                Thing carried = pawn.carryTracker.CarriedThing;
                if (carried != null)
                {
                    Thing building = job.GetTarget(AssemblerIndex).Thing;
                    CompGeneSynthesizer comp = building?.TryGetComp<CompGeneSynthesizer>();
                    if (comp != null && comp.CurrentOrder != null)
                    {
                        bool isArchite = carried.def != null && carried.def == CompGeneSynthesizer.ArchiteDef;
                        int needed = isArchite ? comp.ArchiteNeeded : comp.NeutroamineNeeded;
                        int transfer = Mathf.Min(carried.stackCount, needed);
                        if (transfer > 0)
                        {
                            pawn.carryTracker.innerContainer.TryTransferToContainer(carried, comp.innerContainer, transfer);
                        }

                        if (comp.HasAllMaterials && comp.CurrentOrder.WorkRequired <= 0f)
                        {
                            comp.FinishCurrentOrder();
                        }
                    }

                    if (pawn.carryTracker.CarriedThing != null && pawn.carryTracker.CarriedThing.stackCount > 0)
                    {
                        pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Near, out _);
                    }
                }
            };
            deposit.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return deposit;
        }
    }
}
