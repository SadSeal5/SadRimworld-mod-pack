using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Quarry
{
    public class Building_Quarry : Building_WorkTable, IBillGiver
    {
        public int TotalExcavationsCompleted = 0;
        public const int MaxExcavations = 1000;

        public bool IsDepleted => QuarryModule.ModSettings?.InfiniteQuarry == false && TotalExcavationsCompleted >= MaxExcavations;

        public new bool CurrentlyUsableForBills() => !IsDepleted && base.CurrentlyUsableForBills();
        bool IBillGiver.CurrentlyUsableForBills() => CurrentlyUsableForBills();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref TotalExcavationsCompleted, "TotalExcavationsCompleted", 0);
        }

        public override string GetInspectString()
        {
            string baseString = base.GetInspectString();
            var settings = QuarryModule.ModSettings;

            string customInfo;
            if (settings != null && !settings.InfiniteQuarry)
            {
                int remaining = Math.Max(0, MaxExcavations - TotalExcavationsCompleted);
                customInfo = "MegaMod_Quarry_InspectStringRemaining".Translate(TotalExcavationsCompleted, remaining);
            }
            else
            {
                customInfo = "MegaMod_Quarry_InspectString".Translate(TotalExcavationsCompleted);
            }

            if (string.IsNullOrEmpty(baseString))
            {
                return customInfo;
            }
            return baseString + "\n" + customInfo;
        }
    }

    [HarmonyPatch(typeof(Building_WorkTable), nameof(Building_WorkTable.CurrentlyUsableForBills))]
    public static class Patch_Building_WorkTable_CurrentlyUsableForBills
    {
        public static void Postfix(Building_WorkTable __instance, ref bool __result)
        {
            if (__instance is Building_Quarry quarry && quarry.IsDepleted)
            {
                __result = false;
            }
        }
    }
}
