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
            if (pawn.WorkTagIsDisabled(WorkTags.Violent)) yield break;

            DesignationDef def = Designator_FinishOff.FinishOffDef;
            if (def == null) yield break;

            List<Designation> designations = new List<Designation>(pawn.Map.designationManager.SpawnedDesignationsOfDef(def));
            for (int i = 0; i < designations.Count; i++)
            {
                Designation des = designations[i];
                if (des.target.HasThing && des.target.Thing is Pawn victim)
                {
                    if (victim.Dead || !victim.Downed || !victim.Spawned)
                    {
                        // Stale designation: victim died or got back up. Clean it up to save TPS!
                        pawn.Map.designationManager.RemoveDesignation(des);
                        continue;
                    }
                    yield return victim;
                }
            }
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            if (pawn == null || pawn.WorkTagIsDisabled(WorkTags.Violent)) return true;

            DesignationDef def = Designator_FinishOff.FinishOffDef;
            if (def == null || pawn.Map?.designationManager == null) return true;
            return !pawn.Map.designationManager.AnySpawnedDesignationOfDef(def);
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (pawn.WorkTagIsDisabled(WorkTags.Violent)) return null;

            if (!(t is Pawn victim) || victim.Dead || !victim.Downed || !victim.Spawned)
            {
                if (t != null && pawn?.Map?.designationManager != null)
                {
                    DesignationDef def = Designator_FinishOff.FinishOffDef;
                    if (def != null)
                    {
                        Designation des = pawn.Map.designationManager.DesignationOn(t, def);
                        if (des != null) pawn.Map.designationManager.RemoveDesignation(des);
                    }
                }
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
