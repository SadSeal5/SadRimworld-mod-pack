using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.WorkManager.Patches
{
    [HarmonyPatch(typeof(DefGenerator), "GenerateImpliedDefs_PreResolve")]
    [HarmonyBefore("fluffy.worktab")]
    public static class Patch_DefGenerator
    {
        public static void Postfix()
        {
            var autoPriorities = DefDatabase<PawnColumnDef>.GetNamedSilentFail("AutoWorkPriorities");
            if (autoPriorities != null && PawnTableDefOf.Work != null)
            {
                InsertBefore(PawnTableDefOf.Work, autoPriorities, x => x.defName != null && x.defName.StartsWith("WorkPriority", StringComparison.Ordinal));
            }

            var autoSchedule = DefDatabase<PawnColumnDef>.GetNamedSilentFail("AutoWorkSchedule");
            if (autoSchedule != null && PawnTableDefOf.Restrict != null)
            {
                InsertBefore(PawnTableDefOf.Restrict, autoSchedule, x => x.defName != null && x.defName.Equals("Timetable", StringComparison.Ordinal));
            }
        }

        private static void InsertBefore(PawnTableDef table, PawnColumnDef column, Func<PawnColumnDef, bool> target)
        {
            if (table.columns == null) return;
            if (table.columns.Contains(column)) return;

            int index = table.columns.FindIndex(target.Invoke);
            if (index < 0)
            {
                table.columns.Add(column);
            }
            else
            {
                table.columns.Insert(index, column);
            }
        }
    }
}
