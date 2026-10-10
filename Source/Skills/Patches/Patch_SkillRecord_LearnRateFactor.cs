using HarmonyLib;
using RimWorld;

namespace SadSeal.MegaMod.Skills.Patches
{
    [HarmonyPatch(typeof(SkillRecord), nameof(SkillRecord.LearnRateFactor))]
    public static class Patch_SkillRecord_LearnRateFactor
    {
        public static void Postfix(SkillRecord __instance, bool direct, ref float __result)
        {
            if (MegaMod.Settings?.Skills != null && MegaMod.Settings.Skills.disableDailyCap && !direct)
            {
                if (__instance != null && MegaMod.Settings.Skills.ShouldApplyTo(__instance.Pawn) && __instance.LearningSaturatedToday)
                {
                    __result /= 0.2f;
                }
            }
        }
    }
}
