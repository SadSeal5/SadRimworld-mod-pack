using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Incubation.Patches
{
    [HarmonyPatch(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.Notify_TickedInGrowthVat))]
    public static class Pawn_AgeTracker_Notify_TickedInGrowthVat_Patch
    {
        [HarmonyPrepare]
        public static bool Prepare() => ModsConfig.BiotechActive;

        [HarmonyPrefix]
        public static bool Prefix(Pawn_AgeTracker __instance, ref int ticks, Pawn ___pawn)
        {
            if (!IncubationModule.Instance.IsEnabled) return true;

            var settings = IncubationModule.Settings;
            var vat = ___pawn.ParentHolder as Building_GrowthVat;

            if (vat != null)
            {
                // Safe stasis protection: if unpowered or food is depleted, pause biological aging
                bool powerRequired = settings.powerUsageMultiplier > 0.001f;
                bool unpowered = powerRequired && !vat.PowerOn;
                bool foodRequired = settings.nutritionUsageMultiplier > 0.001f;
                bool starving = foodRequired && (vat.NutritionStored <= 0f);
                bool unpoweredOrStarving = unpowered || starving;

                if (settings.stasisProtection && unpoweredOrStarving)
                {
                    return false;
                }

                // Child aging speed multiplier
                float mult = settings.childSpeedMultiplier;
                ticks = Mathf.RoundToInt(ticks * mult);
                if (ticks < 1 && mult > 0f)
                {
                    ticks = 1;
                }
            }

            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(Pawn_AgeTracker __instance, int ticks, Pawn ___pawn)
        {
            if (!IncubationModule.Instance.IsEnabled) return;

            var settings = IncubationModule.Settings;
            var vat = ___pawn.ParentHolder as Building_GrowthVat;
            if (vat == null) return;

            // Preserve growth tier progression: accrue points smoothly
            if (settings.preserveGrowthTier && __instance.canGainGrowthPoints)
            {
                float age = __instance.AgeBiologicalYearsFloat;
                if (age >= 3f && age < 13f)
                {
                    float cycleYears = age < 7f ? 4f : 3f;
                    float cycleTicks = cycleYears * 3600000f; // 60 days * 60,000 ticks/day
                    float maxPoints = GrowthUtility.GrowthTiers != null && GrowthUtility.GrowthTiers.Length > 0
                        ? GrowthUtility.GrowthTiers[GrowthUtility.GrowthTiers.Length - 1].pointsRequirement
                        : 162f;
                    float pointsToAdd = (ticks / cycleTicks) * maxPoints;
                    __instance.growthPoints = Mathf.Min(__instance.growthPoints + pointsToAdd, maxPoints);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.TryChildGrowthMoment))]
    public static class Pawn_AgeTracker_TryChildGrowthMoment_Patch
    {
        [HarmonyPrepare]
        public static bool Prepare() => ModsConfig.BiotechActive;

        [HarmonyPrefix]
        public static void Prefix(Pawn_AgeTracker __instance, Pawn ___pawn, int birthdayAge)
        {
            if (IncubationModule.Instance.IsEnabled && IncubationModule.Settings.preserveGrowthTier)
            {
                // Only apply if pawn is currently in a growth vat and this is a growth birthday
                if (___pawn.ParentHolder is Building_GrowthVat && GrowthUtility.IsGrowthBirthday(birthdayAge))
                {
                    float maxPoints = GrowthUtility.GrowthTiers != null && GrowthUtility.GrowthTiers.Length > 0
                        ? GrowthUtility.GrowthTiers[GrowthUtility.GrowthTiers.Length - 1].pointsRequirement
                        : 162f;
                    if (__instance.growthPoints < maxPoints)
                    {
                        __instance.growthPoints = maxPoints;
                    }
                }
            }
        }
    }
}
