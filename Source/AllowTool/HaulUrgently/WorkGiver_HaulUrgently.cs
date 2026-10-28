using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.AllowTool.HaulUrgently
{
    public class WorkGiver_HaulUrgently : WorkGiver_Scanner
    {
        private static JobDef cachedSmartHaulDef;

        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.HaulableAlways);

        public override PathEndMode PathEndMode => PathEndMode.ClosestTouch;

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            if (pawn?.Map?.designationManager == null) yield break;

            DesignationDef def = Designator_HaulUrgently.HaulUrgentlyDef;
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
            DesignationDef def = Designator_HaulUrgently.HaulUrgentlyDef;
            if (def == null || pawn?.Map?.designationManager == null) return true;
            return !pawn.Map.designationManager.AnySpawnedDesignationOfDef(def);
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t == null || !t.Spawned || t.IsForbidden(pawn) || !pawn.CanReserve(t, 1, -1, null, forced))
            {
                return null;
            }

            if (!HaulAIUtility.PawnCanAutomaticallyHaulFast(pawn, t, forced))
            {
                return null;
            }

            if (!StoreUtility.TryFindBestBetterStoreCellFor(t, pawn, pawn.Map, StoragePriority.Unstored, pawn.Faction, out IntVec3 storeCell, true))
            {
                return null;
            }

            if (!pawn.CanReserve(storeCell, 1, -1, null, forced))
            {
                return null;
            }

            if (cachedSmartHaulDef == null)
            {
                cachedSmartHaulDef = DefDatabase<JobDef>.GetNamedSilentFail("SadHaul_MultiHaulToCell");
            }

            bool useSmartHaul = MegaMod.Settings?.Hauling?.EnableSmartHaul == true && cachedSmartHaulDef != null;
            JobDef chosenJobDef = useSmartHaul ? cachedSmartHaulDef : JobDefOf.HaulToCell;

            Job job = JobMaker.MakeJob(chosenJobDef, t, storeCell);
            job.count = t.stackCount;
            return job;
        }
    }
}
