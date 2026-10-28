using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.QoL.Patches.Pawn
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    public static class Patch_FloatMenu_Sleep
    {
        public static void Postfix(Vector3 clickPos, Verse.Pawn pawn, List<FloatMenuOption> opts)
        {
            if (MegaMod.Settings?.QoL?.prioritizeSleep != true) return;
            if (pawn == null || !pawn.IsColonistPlayerControlled || pawn.Drafted || pawn.Map == null) return;
            if (pawn.needs?.rest == null || pawn.needs.rest.CurLevel >= 0.75f) return;

            IntVec3 cell = IntVec3.FromVector3(clickPos);
            Building_Bed bed = cell.GetFirstThing<Building_Bed>(pawn.Map);
            if (bed == null) return;

            if (RestUtility.IsValidBedFor(bed, pawn, pawn, false) && pawn.CanReach(bed, PathEndMode.OnCell, Danger.Some))
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
}
