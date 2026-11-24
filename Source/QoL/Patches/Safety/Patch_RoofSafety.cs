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
        private static readonly HashSet<IntVec3> candidateRoofCells = new HashSet<IntVec3>();

        public static void ClearCache()
        {
            lastWarningTickByTargetId.Clear();
            candidateRoofCells.Clear();
        }

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
            if (QoLModule.Instance?.IsEnabled != true) return false;
            if (QoLModule.ModSettings?.enableRoofSafety != true) return false;
            if (target == null) return false;
            if (map == null) map = target.Map;
            if (map == null || target.def == null) return false;
            if (!target.def.holdsRoof) return false;

            RoofGrid roofGrid = map.roofGrid;
            if (roofGrid == null || map.floodFiller == null) return false;

            candidateRoofCells.Clear();
            int supportRadiusIndex = RoofCollapseUtility.RoofSupportRadialCellsCount;
            foreach (IntVec3 cell in target.OccupiedRect())
            {
                for (int i = 0; i < supportRadiusIndex; i++)
                {
                    IntVec3 rc = cell + GenRadial.RadialPattern[i];
                    if (!rc.InBounds(map)) continue;

                    RoofDef roof = roofGrid.RoofAt(rc);
                    if (roof != null && roof.canCollapse)
                    {
                        candidateRoofCells.Add(rc);
                    }
                }
            }

            try
            {
                foreach (IntVec3 rc in candidateRoofCells)
                {
                    bool supported = false;
                    map.floodFiller.FloodFill(
                        rc,
                        (IntVec3 x) => (x.Roofed(map) || x == rc) && x.InHorDistOf(rc, 6.9f),
                        (IntVec3 x) =>
                        {
                            for (int d = 0; d < 5; d++)
                            {
                                IntVec3 adj = x + GenAdj.CardinalDirectionsAndInside[d];
                                if (adj.InBounds(map) && adj.InHorDistOf(rc, 6.9f))
                                {
                                    Building edifice = adj.GetEdifice(map);
                                    if (edifice != null && edifice != target && edifice.def != null && edifice.def.holdsRoof)
                                    {
                                        supported = true;
                                        return true;
                                    }
                                }
                            }
                            return false;
                        });
                    if (!supported)
                    {
                        return true;
                    }
                }
            }
            finally
            {
                candidateRoofCells.Clear();
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(WorkGiver_Miner), nameof(WorkGiver_Miner.JobOnThing))]
    public static class Patch_WorkGiver_Miner_RoofSafety
    {
        public static void Postfix(Verse.Pawn pawn, Thing t, bool forced, ref Job __result)
        {
            if (QoLModule.Instance?.IsEnabled != true) return;
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
            if (QoLModule.Instance?.IsEnabled != true) return;
            if (!__result) return;
            if (QoLModule.ModSettings?.enableRoofSafety != true) return;
            if (forced) return;
            if (RoofSafetyUtility.WillCauseRoofCollapse(t, pawn?.Map))
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(WorkGiver_Uninstall), nameof(WorkGiver_Uninstall.HasJobOnThing))]
    public static class Patch_WorkGiver_Uninstall_RoofSafety
    {
        public static void Postfix(Verse.Pawn pawn, Thing t, bool forced, ref bool __result)
        {
            if (QoLModule.Instance?.IsEnabled != true) return;
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
            if (QoLModule.Instance?.IsEnabled != true) return true;
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
            if (QoLModule.Instance?.IsEnabled != true) return true;
            if (QoLModule.ModSettings?.enableRoofSafety != true) return true;
            if (!(__instance is JobDriver_Deconstruct || __instance is JobDriver_Uninstall)) return true;
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
                    if (QoLModule.Instance?.IsEnabled != true) return;
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

    [HarmonyPatch(typeof(JobDriver_RemoveBuilding), "MakeNewToils")]
    public static class Patch_JobDriver_Deconstruct_Toils_RoofSafety
    {
        public static IEnumerable<Toil> Postfix(IEnumerable<Toil> toils, JobDriver_RemoveBuilding __instance)
        {
            bool shouldGuard = __instance is JobDriver_Deconstruct || __instance is JobDriver_Uninstall;
            foreach (Toil toil in toils)
            {
                if (shouldGuard)
                {
                    toil.AddPreTickAction(() =>
                    {
                        if (QoLModule.Instance?.IsEnabled != true) return;
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
                }
                yield return toil;
            }
        }
    }
}
