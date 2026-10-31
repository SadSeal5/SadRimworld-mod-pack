using System;
using HarmonyLib;
using RimWorld;

namespace SadSeal.MegaMod.Medical.Mood
{
    [HarmonyPatch(typeof(Thought_Memory), nameof(Thought_Memory.MoodOffset))]
    public static class Patch_OrganHarvestMood
    {
        public static void Postfix(Thought_Memory __instance, ref float __result)
        {
            if (__result >= 0f) return; // Only soften negative mood penalties

            var settings = MedicalModule.ModSettings;
            if (settings == null || !settings.SoftenHarvestMoodPenalty) return;

            if (__instance?.def == null) return;

            string name = __instance.def.defName;
            if (name.IndexOf("Organ", StringComparison.OrdinalIgnoreCase) >= 0 ||
                name.IndexOf("HarvestedOrgan", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                __result *= settings.HarvestMoodPenaltyMultiplier;
            }
        }
    }
}
