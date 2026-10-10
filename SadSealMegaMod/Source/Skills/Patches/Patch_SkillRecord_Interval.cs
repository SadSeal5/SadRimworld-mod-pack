using HarmonyLib;
using RimWorld;

namespace SadSeal.MegaMod.Skills.Patches
{
    [HarmonyPatch(typeof(SkillRecord), nameof(SkillRecord.Interval))]
    public static class Patch_SkillRecord_Interval
    {
        public static bool Prefix(SkillRecord __instance)
        {
            if (MegaMod.Settings?.Skills != null && MegaMod.Settings.Skills.disableSkillDecay)
            {
                if (__instance != null && MegaMod.Settings.Skills.ShouldApplyTo(__instance.Pawn))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
