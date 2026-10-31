using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.Hauling.Patches
{
    [HarmonyPatch(typeof(WorkGiver_HaulGeneral), nameof(WorkGiver_HaulGeneral.JobOnThing))]
    public static class Patch_WorkGiver_HaulGeneral
    {
        private static JobDef cachedSmartHaulJobDef;
        private static bool defCached;

        public static void Postfix(Pawn pawn, Thing t, bool forced, ref Job __result)
        {
            if (__result == null || __result.def != JobDefOf.HaulToCell)
            {
                return;
            }

            var settings = HaulModule.ModSettings;
            if (settings == null || !settings.EnableSmartHaul)
            {
                return;
            }

            if (pawn == null)
            {
                return;
            }

            if (!pawn.RaceProps.Humanlike && !settings.AnimalsCanSmartHaul)
            {
                return;
            }

            if (!defCached)
            {
                cachedSmartHaulJobDef = DefDatabase<JobDef>.GetNamedSilentFail("SadHaul_MultiHaulToCell");
                defCached = true;
            }

            if (cachedSmartHaulJobDef != null)
            {
                __result.def = cachedSmartHaulJobDef;
            }
        }
    }
}
