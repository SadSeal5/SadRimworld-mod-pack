using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Incubation.Patches
{
    [HarmonyPatch(typeof(Building_GrowthVat), nameof(Building_GrowthVat.NutritionConsumedPerDay), MethodType.Getter)]
    public static class Building_GrowthVat_NutritionConsumedPerDay_Patch
    {
        [HarmonyPrepare]
        public static bool Prepare() => ModLister.BiotechInstalled;

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
        public static bool Prepare() => ModLister.BiotechInstalled;

        public static void Postfix(Building_GrowthVat __instance, ref float __result)
        {
            if (IncubationModule.Instance.IsEnabled && IncubationModule.Settings.stasisProtection)
            {
                // If power is lost or food is depleted, freeze biostarvation increase
                if (!__instance.PowerOn || __instance.NutritionStored <= 0f)
                {
                    __result = 0f;
                }
            }
        }
    }
}
