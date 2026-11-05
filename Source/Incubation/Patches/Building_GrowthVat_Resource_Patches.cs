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
            if (IncubationModule.Instance.IsEnabled)
            {
                __result *= IncubationModule.Settings.nutritionUsageMultiplier;
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
            if (!IncubationModule.Instance.IsEnabled) return;

            var settings = IncubationModule.Settings;
            if (settings.nutritionUsageMultiplier <= 0.001f)
            {
                __result = 0f;
                return;
            }

            if (settings.stasisProtection)
            {
                bool powerRequired = settings.powerUsageMultiplier > 0.001f;
                bool unpowered = powerRequired && !__instance.PowerOn;
                bool foodRequired = settings.nutritionUsageMultiplier > 0.001f;
                bool starving = foodRequired && (__instance.NutritionStored <= 0f);
                bool unpoweredOrStarving = unpowered || starving;

                if (unpoweredOrStarving)
                {
                    __result = 0f;
                }
            }
        }
    }
}
