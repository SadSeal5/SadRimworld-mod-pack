using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.AllowTool.FinishOff
{
    public class JobDriver_FinishOff : JobDriver
    {
        private const TargetIndex VictimInd = TargetIndex.A;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(VictimInd), job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(VictimInd);
            this.FailOn(() =>
            {
                Pawn victim = TargetA.Pawn;
                return victim == null || victim.Dead || !victim.Downed;
            });

            yield return Toils_Goto.GotoThing(VictimInd, PathEndMode.Touch);

            Toil executeWait = ToilMaker.MakeToil("ExecuteWait");
            executeWait.defaultCompleteMode = ToilCompleteMode.Delay;
            executeWait.defaultDuration = 80;
            executeWait.WithProgressBarToilDelay(VictimInd);
            executeWait.handlingFacing = true;
            executeWait.tickAction = () =>
            {
                pawn.rotationTracker.FaceTarget(TargetA);
            };
            yield return executeWait;

            Toil strike = ToilMaker.MakeToil("ExecuteStrike");
            strike.initAction = () =>
            {
                Pawn victim = TargetA.Pawn;
                if (victim != null && !victim.Dead)
                {
                    DamageInfo dinfo = new DamageInfo(DamageDefOf.ExecutionCut, 9999f, 999f, -1, pawn);
                    victim.Kill(dinfo);

                    if (Map?.designationManager != null)
                    {
                        Map.designationManager.RemoveAllDesignationsOn(victim);
                    }

                    Messages.Message("MegaMod_FinishedOffSuccess".Translate(pawn.LabelShort, victim.LabelShort),
                        MessageTypeDefOf.PositiveEvent, false);
                }
            };
            yield return strike;
        }
    }
}
