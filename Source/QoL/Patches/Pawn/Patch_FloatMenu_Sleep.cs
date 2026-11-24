using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.QoL.Patches.Pawn
{
    [HarmonyPatch]
    public static class Patch_FloatMenu_Sleep
    {
        public static bool Prepare() => AccessTools.Method(typeof(FloatMenuMakerMap), "AddHumanlikeOrders") != null;
        public static MethodBase TargetMethod() => AccessTools.Method(typeof(FloatMenuMakerMap), "AddHumanlikeOrders");

        public static void Postfix(Vector3 clickPos, Verse.Pawn pawn, List<FloatMenuOption> opts)
        {
            AddSleepOption(clickPos, pawn, opts);
        }

        public static void AddSleepOption(Vector3 clickPos, Verse.Pawn pawn, List<FloatMenuOption> opts)
        {
            if (opts == null) return;
            if (QoLModule.Instance?.IsEnabled != true) return;
            if (QoLModule.ModSettings?.prioritizeSleep != true) return;
            if (pawn == null || !pawn.IsColonistPlayerControlled || pawn.Drafted || pawn.Downed || pawn.Deathresting || pawn.InMentalState || pawn.Map == null) return;
            if (!FloatMenuMakerMap.ShouldGenerateFloatMenuForPawn(pawn).Accepted) return;
            if (pawn.needs?.rest == null || pawn.needs.rest.CurLevel >= 0.75f) return;

            IntVec3 cell = IntVec3.FromVector3(clickPos);
            if (!cell.InBounds(pawn.Map)) return;
            Building_Bed bed = cell.GetFirstThing<Building_Bed>(pawn.Map);
            if (bed == null) return;

            if (!bed.IsForbidden(pawn) && RestUtility.IsValidBedFor(bed, pawn, pawn, false) && pawn.CanReach(bed, PathEndMode.OnCell, Danger.Some) && pawn.CanReserve(bed, bed.SleepingSlotsCount, 0))
            {
                string label = "MegaMod_QoLPrioritizeRest".Translate(bed.LabelShort);
                FloatMenuOption option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, () =>
                {
                    Job job = JobMaker.MakeJob(JobDefOf.LayDown, bed);
                    pawn.jobs?.TryTakeOrderedJob(job, JobTag.Misc);
                }), pawn, bed);

                opts.Add(option);
            }
        }
    }

    [HarmonyPatch]
    public static class Patch_FloatMenu_Sleep_1_7
    {
        public static bool Prepare() => AccessTools.Method(typeof(FloatMenuMakerMap), "GetOptions") != null;
        public static MethodBase TargetMethod() => AccessTools.Method(typeof(FloatMenuMakerMap), "GetOptions");

        public static void Postfix(List<Verse.Pawn> selectedPawns, Vector3 clickPos, List<FloatMenuOption> __result)
        {
            if (selectedPawns != null && selectedPawns.Count == 1 && __result != null)
            {
                Patch_FloatMenu_Sleep.AddSleepOption(clickPos, selectedPawns[0], __result);
            }
        }
    }
}
