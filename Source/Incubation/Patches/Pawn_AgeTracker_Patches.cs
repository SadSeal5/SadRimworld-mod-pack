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
        public static bool Prepare() => ModLister.BiotechInstalled;

        [HarmonyPrefix]
        public static bool Prefix(Pawn_AgeTracker __instance, ref int ticks, Pawn ___pawn)
        {
            if (!IncubationModule.Instance.IsEnabled) return true;

            var settings = IncubationModule.Settings;
            var vat = ___pawn.ParentHolder as Building_GrowthVat;

            if (vat != null)
            {
                // Safe stasis protection: if unpowered or food is depleted, pause biological aging
                if (settings.stasisProtection && (!vat.PowerOn || vat.NutritionStored <= 0f))
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

            // Preserve growth tier progression
            if (settings.preserveGrowthTier && __instance.canGainGrowthPoints)
            {
                float age = __instance.AgeBiologicalYearsFloat;
                if (age >= 3f && age < 13f)
                {
                    float cycleStart = age < 7f ? 3f : (age < 10f ? 7f : 10f);
                    float cycleEnd = age < 7f ? 7f : (age < 10f ? 10f : 13f);
                    float progress = Mathf.Clamp01((age - cycleStart) / (cycleEnd - cycleStart));
                    float targetPoints = progress * 162f; // 162f is Tier 8 max requirement
                    if (__instance.growthPoints < targetPoints)
                    {
                        __instance.growthPoints = targetPoints;
                    }
                }
            }

            // Auto-eject occupant when reaching target age
            if (settings.autoEjectEnabled && __instance.AgeBiologicalYearsFloat >= settings.targetEjectAge)
            {
                int targetAge = settings.targetEjectAge;
                Pawn pawn = ___pawn;
                vat.EjectContents();
                Messages.Message("SadSeal_Incubation_AutoEjectedTargetAge".Translate(pawn.Named("PAWN"), targetAge.Named("TARGETAGE")), pawn, MessageTypeDefOf.PositiveEvent);
            }
        }
    }

    [HarmonyPatch(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.TryChildGrowthMoment))]
    public static class Pawn_AgeTracker_TryChildGrowthMoment_Patch
    {
        [HarmonyPrepare]
        public static bool Prepare() => ModLister.BiotechInstalled;

        [HarmonyPrefix]
        public static void Prefix(Pawn_AgeTracker __instance, Pawn ___pawn, int birthdayAge)
        {
            if (IncubationModule.Instance.IsEnabled && IncubationModule.Settings.preserveGrowthTier)
            {
                // If the pawn is in a growth vat or was grown in one, ensure they achieve Tier 8 choices
                if (___pawn.ParentHolder is Building_GrowthVat || __instance.vatGrowTicks > 0)
                {
                    if (__instance.growthPoints < 162f)
                    {
                        __instance.growthPoints = 162f;
                    }
                }
            }
        }
    }
}
