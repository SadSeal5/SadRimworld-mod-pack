using HarmonyLib;
using Verse;

namespace SadSeal.MegaMod.TimeFlow.Patches
{
    [HarmonyPatch(typeof(TimeSlower), nameof(TimeSlower.SignalForceNormalSpeed))]
    public static class Patch_TimeSlower_ForceNormal
    {
        public static bool Prefix()
        {
            var settings = MegaMod.Settings?.TimeFlow;
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
            var settings = MegaMod.Settings?.TimeFlow;
            if (settings != null && settings.DisableForcedSlowdown)
            {
                return false;
            }
            return true;
        }
    }
}
