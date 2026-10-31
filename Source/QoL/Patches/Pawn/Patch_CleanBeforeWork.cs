using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.QoL.Patches.Pawn
{
    [HarmonyPatch(typeof(WorkGiver_DoBill), nameof(WorkGiver_DoBill.JobOnThing))]
    public static class Patch_CleanBeforeWork
    {
        public static void Postfix(WorkGiver_DoBill __instance, Verse.Pawn pawn, Thing thing, bool forced, ref Job __result)
        {
            if (__result == null) return;
            if (QoLModule.ModSettings?.enableCleanBeforeWork != true) return;
            // Respect player priority: if forced, proceed immediately
            if (forced) return;
            if (pawn == null || thing == null || !pawn.Spawned || !thing.Spawned) return;
            if (pawn.WorkTypeIsDisabled(WorkTypeDefOf.Cleaning)) return;

            // Check if workstation is a cooking stove or medical/surgery bed
            bool isCooking = IsCookingWorkstation(thing);
            bool isMedical = IsMedicalWorkstation(thing);

            if (!isCooking && !isMedical) return;

            Job cleanJob = TryGetCleanJob(pawn, thing);
            if (cleanJob != null)
            {
                __result = cleanJob;
            }
        }

        public static Job TryGetCleanJob(Verse.Pawn pawn, Thing thing)
        {
            if (pawn == null || thing == null || !pawn.Spawned || !thing.Spawned) return null;
            if (pawn.WorkTypeIsDisabled(WorkTypeDefOf.Cleaning)) return null;

            Room room = thing.GetRoom();
            if (room == null || room.TouchesMapEdge || room.PsychologicallyOutdoors) return null;

            Map map = pawn.Map;
            if (map == null) return null;
            IntVec3 center = thing.Position;
            List<Thing> candidateFilth = new List<Thing>();

            // Bound filth search to 8 tiles around thing in the same room
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(center, 8f, true))
            {
                if (!cell.InBounds(map)) continue;
                if (cell.GetRoom(map) != room) continue;

                List<Thing> thingList = cell.GetThingList(map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    if (thingList[i] is Filth filth && pawn.CanReserve(filth))
                    {
                        candidateFilth.Add(filth);
                    }
                }
            }

            if (candidateFilth.Count == 0) return null;

            // Pick closest reachable filth as primary target
            candidateFilth.SortBy(f => f.Position.DistanceToSquared(pawn.Position));
            Thing primaryFilth = null;
            for (int i = 0; i < candidateFilth.Count; i++)
            {
                if (pawn.CanReach(candidateFilth[i], PathEndMode.Touch, Danger.Some))
                {
                    primaryFilth = candidateFilth[i];
                    break;
                }
            }

            if (primaryFilth == null) return null;

            Job cleanJob = JobMaker.MakeJob(JobDefOf.Clean);
            cleanJob.AddQueuedTarget(TargetIndex.A, primaryFilth);
            int queued = 0;
            for (int i = 0; i < candidateFilth.Count; i++)
            {
                Thing f = candidateFilth[i];
                if (f != primaryFilth && pawn.CanReserve(f))
                {
                    cleanJob.AddQueuedTarget(TargetIndex.A, f);
                    queued++;
                    if (queued >= 4) break;
                }
            }

            return cleanJob;
        }

        private static bool IsCookingWorkstation(Thing thing)
        {
            if (thing.def == null) return false;
            string defName = thing.def.defName;
            if (defName.Contains("Stove") || defName.Contains("Campfire") || defName.Contains("Butcher")) return true;

            if (thing is Building_WorkTable bwt && bwt.def?.recipes != null)
            {
                return bwt.def.recipes.Any(r => r.products != null && r.products.Any(p => p.thingDef != null && p.thingDef.IsNutritionGivingIngestible));
            }

            return false;
        }

        private static bool IsMedicalWorkstation(Thing thing)
        {
            if (thing is Building_Bed bed && bed.Medical) return true;
            if (thing is Verse.Pawn patient && patient.CurrentBed() != null && patient.CurrentBed().Medical) return true;
            return false;
        }
    }

    [HarmonyPatch(typeof(WorkGiver_TakeToBedToOperate), nameof(WorkGiver_TakeToBedToOperate.JobOnThing))]
    public static class Patch_TakeToBedToOperate_CleanBeforeWork
    {
        public static void Postfix(WorkGiver_TakeToBedToOperate __instance, Verse.Pawn pawn, Thing t, bool forced, ref Job __result)
        {
            if (__result == null) return;
            if (QoLModule.ModSettings?.enableCleanBeforeWork != true) return;
            // Respect player priority: if forced, proceed immediately
            if (forced) return;
            if (pawn == null || t == null || !pawn.Spawned || !t.Spawned) return;
            if (pawn.WorkTypeIsDisabled(WorkTypeDefOf.Cleaning)) return;

            // Target bed can be targetB of the job or patient's current bed
            Thing bed = __result.targetB.Thing ?? (t is Verse.Pawn patient ? patient.CurrentBed() : t);
            if (bed == null) return;

            Job cleanJob = Patch_CleanBeforeWork.TryGetCleanJob(pawn, bed);
            if (cleanJob != null)
            {
                __result = cleanJob;
            }
        }
    }
}
