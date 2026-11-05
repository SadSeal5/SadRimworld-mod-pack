using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Incubation.Patches
{
    [HarmonyPatch(typeof(ThoughtWorker_ChildInGrowthVat), "CurrentStateInternal")]
    public static class ThoughtWorker_ChildInGrowthVat_Patch
    {
        [HarmonyPrepare]
        public static bool Prepare() => ModsConfig.BiotechActive;

        [HarmonyPrefix]
        public static bool Prefix(ref ThoughtState __result)
        {
            if (IncubationModule.Instance.IsEnabled && IncubationModule.Settings.disableParentGuilt)
            {
                __result = ThoughtState.Inactive;
                return false;
            }
            return true;
        }
    }
}
