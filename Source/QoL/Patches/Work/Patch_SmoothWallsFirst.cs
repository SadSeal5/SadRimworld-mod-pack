using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.QoL.Patches.Work
{
    [HarmonyPatch(typeof(WorkGiver_ConstructSmoothFloor), nameof(WorkGiver_ConstructSmoothFloor.JobOnThing))]
    public static class Patch_SmoothWallsFirst
    {
        public static bool Prefix(Verse.Pawn pawn, Thing t, bool forced, ref Job __result)
        {
            if (forced) return true;
            if (MegaMod.Settings?.QoL?.smoothWallsBeforeFloors != true) return true;
            if (pawn?.Map == null) return true;

            // Prioritize smoothing rough rock walls before smoothing stone floors
            if (pawn.Map.designationManager.SpawnedDesignationsOfDef(DesignationDefOf.SmoothWall).Any())
            {
                __result = null;
                return false;
            }

            return true;
        }
    }
}
