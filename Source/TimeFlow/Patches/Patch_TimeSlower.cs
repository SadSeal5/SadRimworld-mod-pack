using HarmonyLib;
using Verse;

namespace SadSeal.MegaMod.TimeFlow.Patches
{
    [HarmonyPatch(typeof(TimeSlower), nameof(TimeSlower.ForcedNormalSpeed), MethodType.Getter)]
    public static class Patch_TimeSlower_ForcedNormalSpeed
    {
        public static bool Prefix(ref bool __result)
        {
            var settings = TimeFlowModule.ModSettings;
            if (settings != null && settings.DisableForcedSlowdown)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(TimeSlower), nameof(TimeSlower.SignalForceNormalSpeed))]
    public static class Patch_TimeSlower_ForceNormal
    {
        public static bool Prefix()
        {
            var settings = TimeFlowModule.ModSettings;
            if (settings != null && settings.DisableForcedSlowdown)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(TimeSlower), nameof(TimeSlower.SignalForceNormalSpeedShort))]
    public static class Patch_TimeSlower_ForceNormalShort
    {
        public static bool Prefix()
        {
            var settings = TimeFlowModule.ModSettings;
            if (settings != null && settings.DisableForcedSlowdown)
            {
                return false;
            }
            return true;
        }
    }
}
