using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Incubation.Patches
{
    [HarmonyPatch(typeof(Hediff_VatLearning), nameof(Hediff_VatLearning.PostTickInterval))]
    public static class Hediff_VatLearning_PostTickInterval_Patch
    {
        [HarmonyPrepare]
        public static bool Prepare() => ModsConfig.BiotechActive;

        [HarmonyPrefix]
        public static bool Prefix(Hediff_VatLearning __instance)
        {
            if (!IncubationModule.Instance.IsEnabled) return true;

            var settings = IncubationModule.Settings;
            if (!settings.stasisProtection) return true;

            var vat = __instance.pawn?.ParentHolder as Building_GrowthVat;
            if (vat != null && vat.IsInStasis(settings))
            {
                return false;
            }

            return true;
        }
    }
}
