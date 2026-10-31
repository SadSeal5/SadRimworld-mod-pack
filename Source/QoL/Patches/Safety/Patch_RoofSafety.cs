using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.QoL.Patches.Safety
{
    public static class RoofSafetyUtility
    {
        private static readonly Dictionary<int, int> lastWarningTickByTargetId = new Dictionary<int, int>();

        public static void NotifyRoofSafetyPrevented(Verse.Pawn pawn, Thing target)
        {
            if (target == null) return;
            int currentTick = Find.TickManager != null ? Find.TickManager.TicksGame : 0;
            int targetId = target.thingIDNumber;
            if (lastWarningTickByTargetId.TryGetValue(targetId, out int lastTick))
            {
                if (currentTick - lastTick < 300) return;
            }
            lastWarningTickByTargetId[targetId] = currentTick;
            string pawnName = pawn != null ? pawn.LabelShort : "Colonist";
            Messages.Message("MegaMod_RoofSafety_Prevented".Translate(pawnName, target.LabelShort), target, MessageTypeDefOf.CautionInput, false);
        }

        public static bool WillCauseRoofCollapse(Thing target, Map map)
        {
            if (QoLModule.ModSettings?.enableRoofSafety != true) return false;
            if (target == null) return false;
            if (map == null) map = target.Map;
            if (map == null || target.def == null) return false;
            if (!target.def.holdsRoof) return false;

            IntVec3 targetPos = target.Position;

            // Check if any roof cell within radial 7f of target.Position has NO other building edifice with def.holdsRoof == true within radial 6.9f
            foreach (IntVec3 rc in GenRadial.RadialCellsAround(targetPos, 7f, true))
            {
                if (!rc.InBounds(map)) continue;
                if (map.roofGrid.RoofAt(rc) == null) continue;

                bool hasOtherSupport = false;
                foreach (IntVec3 sc in GenRadial.RadialCellsAround(rc, 6.9f, true))
                {
                    if (!sc.InBounds(map)) continue;
                    Building edifice = sc.GetEdifice(map);
                    if (edifice != null && edifice != target && edifice.def != null && edifice.def.holdsRoof)
                    {
                        hasOtherSupport = true;
                        break;
                    }
                }

                if (!hasOtherSupport)
                {
                    return true;
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(WorkGiver_Miner), nameof(WorkGiver_Miner.JobOnThing))]
    public static class Patch_WorkGiver_Miner_RoofSafety
    {
        public static void Postfix(Verse.Pawn pawn, Thing t, bool forced, ref Job __result)
        {
            if (__result == null) return;
            if (QoLModule.ModSettings?.enableRoofSafety != true) return;
            if (forced) return;
            if (RoofSafetyUtility.WillCauseRoofCollapse(t, pawn?.Map))
            {
                __result = null;
            }
        }
    }

    [HarmonyPatch(typeof(WorkGiver_Deconstruct), nameof(WorkGiver_Deconstruct.HasJobOnThing))]
    public static class Patch_WorkGiver_Deconstruct_RoofSafety
    {
        public static void Postfix(Verse.Pawn pawn, Thing t, bool forced, ref bool __result)
        {
            if (!__result) return;
            if (QoLModule.ModSettings?.enableRoofSafety != true) return;
            if (forced) return;
            if (RoofSafetyUtility.WillCauseRoofCollapse(t, pawn?.Map))
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(JobDriver_Mine), "TryMakePreToilReservations")]
    public static class Patch_JobDriver_Mine_RoofSafety
    {
        public static bool Prefix(JobDriver_Mine __instance, bool errorOnFailed, ref bool __result)
        {
            if (QoLModule.ModSettings?.enableRoofSafety != true) return true;
            if (__instance.job != null && __instance.job.playerForced) return true;
            Verse.Pawn pawn = __instance.pawn;
            Thing target = __instance.job?.targetA.Thing;
            if (target != null && RoofSafetyUtility.WillCauseRoofCollapse(target, pawn != null ? pawn.Map : target.Map))
            {
                __result = false;
                RoofSafetyUtility.NotifyRoofSafetyPrevented(pawn, target);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(JobDriver_RemoveBuilding), "TryMakePreToilReservations")]
    public static class Patch_JobDriver_Deconstruct_RoofSafety
    {
        public static bool Prefix(JobDriver_RemoveBuilding __instance, bool errorOnFailed, ref bool __result)
        {
            if (QoLModule.ModSettings?.enableRoofSafety != true) return true;
            if (!(__instance is JobDriver_Deconstruct)) return true;
            if (__instance.job != null && __instance.job.playerForced) return true;
            Verse.Pawn pawn = __instance.pawn;
            Thing target = __instance.job?.targetA.Thing;
            if (target != null && RoofSafetyUtility.WillCauseRoofCollapse(target, pawn != null ? pawn.Map : target.Map))
            {
                __result = false;
                RoofSafetyUtility.NotifyRoofSafetyPrevented(pawn, target);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(JobDriver_Mine), "MakeNewToils")]
    public static class Patch_JobDriver_Mine_Toils_RoofSafety
    {
        public static IEnumerable<Toil> Postfix(IEnumerable<Toil> toils, JobDriver_Mine __instance)
        {
            foreach (Toil toil in toils)
            {
                toil.AddPreTickAction(() =>
                {
                    if (QoLModule.ModSettings?.enableRoofSafety != true) return;
                    if (__instance.job != null && __instance.job.playerForced) return;
                    Verse.Pawn pawn = __instance.pawn;
                    if (pawn == null || !pawn.Spawned) return;
                    if (pawn.IsHashIntervalTick(60))
                    {
                        Thing target = __instance.job?.targetA.Thing;
                        if (target != null && RoofSafetyUtility.WillCauseRoofCollapse(target, pawn.Map))
                        {
                            RoofSafetyUtility.NotifyRoofSafetyPrevented(pawn, target);
                            pawn.jobs.EndCurrentJob(JobCondition.Incompletable, true);
                        }
                    }
                });
                yield return toil;
            }
        }
    }

    [HarmonyPatch(typeof(JobDriver_Deconstruct), "MakeNewToils")]
    public static class Patch_JobDriver_Deconstruct_Toils_RoofSafety
    {
        public static IEnumerable<Toil> Postfix(IEnumerable<Toil> toils, JobDriver_Deconstruct __instance)
        {
            foreach (Toil toil in toils)
            {
                toil.AddPreTickAction(() =>
                {
                    if (QoLModule.ModSettings?.enableRoofSafety != true) return;
                    if (__instance.job != null && __instance.job.playerForced) return;
                    Verse.Pawn pawn = __instance.pawn;
                    if (pawn == null || !pawn.Spawned) return;
                    if (pawn.IsHashIntervalTick(60))
                    {
                        Thing target = __instance.job?.targetA.Thing;
                        if (target != null && RoofSafetyUtility.WillCauseRoofCollapse(target, pawn.Map))
                        {
                            RoofSafetyUtility.NotifyRoofSafetyPrevented(pawn, target);
                            pawn.jobs.EndCurrentJob(JobCondition.Incompletable, true);
                        }
                    }
                });
                yield return toil;
            }
        }
    }
}
