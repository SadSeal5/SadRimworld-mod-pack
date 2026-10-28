using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.AllowTool.FinishOff
{
    public class WorkGiver_FinishOff : WorkGiver_Scanner
    {
        private static JobDef cachedJobDef;

        public static JobDef FinishOffJobDef =>
            cachedJobDef ??= DefDatabase<JobDef>.GetNamedSilentFail("Sad_Job_FinishOff");

        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Pawn);

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            if (pawn?.Map?.designationManager == null) yield break;

            DesignationDef def = Designator_FinishOff.FinishOffDef;
            if (def == null) yield break;

            foreach (Designation des in pawn.Map.designationManager.SpawnedDesignationsOfDef(def))
            {
                if (des.target.HasThing && des.target.Thing.Spawned)
                {
                    yield return des.target.Thing;
                }
            }
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            DesignationDef def = Designator_FinishOff.FinishOffDef;
            if (def == null || pawn?.Map?.designationManager == null) return true;
            return !pawn.Map.designationManager.AnySpawnedDesignationOfDef(def);
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t is Pawn victim) || victim.Dead || !victim.Downed || !victim.Spawned)
            {
                return null;
            }

            if (!pawn.CanReserve(victim, 1, -1, null, forced))
            {
                return null;
            }

            if (!pawn.CanReach(victim, PathEndMode.Touch, Danger.Some))
            {
                return null;
            }

            JobDef jobDef = FinishOffJobDef;
            if (jobDef == null) return null;

            return JobMaker.MakeJob(jobDef, victim);
        }
    }
}
