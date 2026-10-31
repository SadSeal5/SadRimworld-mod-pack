using HarmonyLib;
using RimWorld;

namespace SadSeal.MegaMod.Skills.Patches
{
    [HarmonyPatch(typeof(SkillRecord), nameof(SkillRecord.Interval))]
    public static class Patch_SkillRecord_Interval
    {
        public static bool Prefix(SkillRecord __instance)
        {
            if (SkillsModule.ModSettings != null && SkillsModule.ModSettings.disableSkillDecay)
            {
                if (__instance != null && SkillsModule.ModSettings.ShouldApplyTo(__instance.Pawn))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
