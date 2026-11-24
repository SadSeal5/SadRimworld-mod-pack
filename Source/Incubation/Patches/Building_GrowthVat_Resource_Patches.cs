using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Incubation.Patches
{
    [HarmonyPatch(typeof(Building_GrowthVat), nameof(Building_GrowthVat.NutritionConsumedPerDay), MethodType.Getter)]
    public static class Building_GrowthVat_NutritionConsumedPerDay_Patch
    {
        [HarmonyPrepare]
        public static bool Prepare() => ModsConfig.BiotechActive;

        public static void Postfix(ref float __result)
        {
            if (IncubationModule.Instance?.IsEnabled != true) return;

            var settings = IncubationModule.Settings;
            if (settings != null)
            {
                __result *= settings.nutritionUsageMultiplier;
            }
        }
    }

    [HarmonyPatch(typeof(Building_GrowthVat), nameof(Building_GrowthVat.BiostarvationDailyOffset), MethodType.Getter)]
    public static class Building_GrowthVat_BiostarvationDailyOffset_Patch
    {
        [HarmonyPrepare]
        public static bool Prepare() => ModsConfig.BiotechActive;

        public static void Postfix(Building_GrowthVat __instance, ref float __result)
        {
            if (IncubationModule.Instance?.IsEnabled != true) return;
            if (__result <= 0f) return;

            var settings = IncubationModule.Settings;
            if (settings == null || __instance == null) return;

            bool unpowered = !__instance.PowerOn;
            bool starving = __instance.NutritionStored <= 0f;
            bool powerCovered = !unpowered || settings.stasisProtection || settings.powerUsageMultiplier <= 0.001f;
            bool foodCovered = !starving || settings.stasisProtection || settings.nutritionUsageMultiplier <= 0.001f;
            if (powerCovered && foodCovered)
            {
                __result = 0f;
            }
        }
    }
}
