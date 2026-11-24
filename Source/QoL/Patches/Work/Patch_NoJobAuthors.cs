using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.QoL.Patches.Work
{
    [HarmonyPatch(typeof(WorkGiver_DoBill), "ClosestUnfinishedThingForBill")]
    public static class Patch_NoJobAuthors
    {
        [ThreadStatic]
        internal static Verse.Pawn currentBillWorker;

        public static bool Prefix(Verse.Pawn pawn, Bill_ProductionWithUft bill, ref UnfinishedThing __result)
        {
            if (QoLModule.Instance?.IsEnabled != true || QoLModule.ModSettings?.noJobAuthors != true) return true;
            if (pawn == null || bill?.recipe?.unfinishedThingDef == null || pawn.Map == null) return true;

            Predicate<Thing> validator = (Thing t) =>
            {
                if (t.IsForbidden(pawn)) return false;
                if (!(t is UnfinishedThing uft)) return false;
                if (uft.Recipe != bill.recipe) return false;
                if (uft.BoundBill != null && uft.BoundBill != bill) return false;
                if (uft.ingredients != null && !uft.ingredients.TrueForAll(x => x != null && bill.IsFixedOrAllowedIngredient(x.def))) return false;
                return pawn.CanReserve(t);
            };

            Thing found = GenClosest.ClosestThingReachable(
                pawn.Position,
                pawn.Map,
                ThingRequest.ForDef(bill.recipe.unfinishedThingDef),
                PathEndMode.InteractionCell,
                TraverseParms.For(pawn, pawn.NormalMaxDanger()),
                9999f,
                validator);

            __result = (UnfinishedThing)found;
            return false;
        }
    }

    [HarmonyPatch(typeof(WorkGiver_DoBill), "FinishUftJob")]
    public static class Patch_NoJobAuthors_FinishUftJob
    {
        public static bool Prefix(Verse.Pawn pawn, UnfinishedThing uft, Bill_ProductionWithUft bill, ref Job __result)
        {
            if (QoLModule.Instance?.IsEnabled != true || QoLModule.ModSettings?.noJobAuthors != true) return true;
            if (pawn == null || uft == null || bill == null || uft.Creator == pawn) return true;
            if (!(bill.billStack?.billGiver is Thing billGiver))
            {
                __result = null;
                return false;
            }

            Job haulJob = WorkGiverUtility.HaulStuffOffBillGiverJob(pawn, bill.billStack.billGiver, uft);
            if (haulJob != null && haulJob.targetA.Thing != uft)
            {
                __result = haulJob;
                return false;
            }

            Job job = JobMaker.MakeJob(JobDefOf.DoBill, billGiver);
            job.bill = bill;
            job.targetQueueB = new List<LocalTargetInfo> { uft };
            job.countQueue = new List<int> { 1 };
            job.haulMode = HaulMode.ToCellNonStorage;
            __result = job;
            return false;
        }
    }

    [HarmonyPatch(typeof(WorkGiver_DoBill), nameof(WorkGiver_DoBill.JobOnThing))]
    public static class Patch_WorkGiver_DoBill_JobOnThing
    {
        public static void Prefix(Verse.Pawn pawn)
        {
            if (QoLModule.Instance?.IsEnabled == true && QoLModule.ModSettings?.noJobAuthors == true)
            {
                Patch_NoJobAuthors.currentBillWorker = pawn;
            }
        }

        public static void Finalizer()
        {
            Patch_NoJobAuthors.currentBillWorker = null;
        }
    }

    [HarmonyPatch(typeof(Bill_ProductionWithUft), nameof(Bill_ProductionWithUft.BoundWorker), MethodType.Getter)]
    public static class Patch_Bill_ProductionWithUft_BoundWorker
    {
        public static void Postfix(Bill_ProductionWithUft __instance, ref Verse.Pawn __result)
        {
            if (QoLModule.Instance?.IsEnabled == true &&
                QoLModule.ModSettings?.noJobAuthors == true &&
                Patch_NoJobAuthors.currentBillWorker != null &&
                __instance?.BoundUft != null)
            {
                __result = Patch_NoJobAuthors.currentBillWorker;
            }
        }
    }
}
