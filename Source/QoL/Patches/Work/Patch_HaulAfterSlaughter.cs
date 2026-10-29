using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.QoL.Patches.Work
{
    [HarmonyPatch(typeof(ExecutionUtility), nameof(ExecutionUtility.DoExecutionByCut))]
    public static class Patch_HaulAfterSlaughter
    {
        public static void Postfix(Verse.Pawn executioner, Verse.Pawn victim)
        {
            if (MegaMod.Settings?.QoL?.haulAfterSlaughter != true) return;
            if (executioner == null || !executioner.IsColonist || !executioner.Spawned) return;
            if (executioner.WorkTagIsDisabled(WorkTags.Hauling)) return;
            if (WorkTypeDefOf.Hauling != null && executioner.WorkTypeIsDisabled(WorkTypeDefOf.Hauling)) return;
            if (victim == null || victim.Corpse == null || !victim.Corpse.Spawned) return;

            Corpse corpse = victim.Corpse;
            if (corpse.IsForbidden(executioner)) return;

            Job haulJob = HaulAIUtility.HaulToStorageJob(executioner, corpse, false);
            if (haulJob != null)
            {
                executioner.jobs?.jobQueue?.EnqueueFirst(haulJob);
            }
        }
    }
}
