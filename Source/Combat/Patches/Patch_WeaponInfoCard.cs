using System;
using HarmonyLib;
using RimWorld;

namespace SadSeal.MegaMod.Combat.Patches
{
    [HarmonyPatch(typeof(StatWorker_MeleeAverageDPS), nameof(StatWorker_MeleeAverageDPS.GetValueUnfinalized))]
    public static class Patch_StatWorker_MeleeAverageDPS
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result)
        {
            if (CombatModule.Instance?.IsEnabled != true) return;

            var settings = CombatModule.ModSettings;
            if (settings != null && Math.Abs(settings.meleeDamageMultiplier - 1.0f) > 0.001f)
            {
                __result *= settings.meleeDamageMultiplier;
            }
        }
    }

    [HarmonyPatch(typeof(StatWorker_MeleeAverageArmorPenetration), nameof(StatWorker_MeleeAverageArmorPenetration.GetValueUnfinalized))]
    public static class Patch_StatWorker_MeleeAverageArmorPenetration
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result)
        {
            if (CombatModule.Instance?.IsEnabled != true) return;

            var settings = CombatModule.ModSettings;
            if (settings != null && Math.Abs(settings.meleeArmorPenetrationMultiplier - 1.0f) > 0.001f)
            {
                __result *= settings.meleeArmorPenetrationMultiplier;
            }
        }
    }
}
