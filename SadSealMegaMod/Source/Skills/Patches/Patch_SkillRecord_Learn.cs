using HarmonyLib;
using RimWorld;

namespace SadSeal.MegaMod.Skills.Patches
{
    [HarmonyPatch(typeof(SkillRecord), nameof(SkillRecord.Learn))]
    public static class Patch_SkillRecord_Learn
    {
        public static void Prefix(SkillRecord __instance, ref float xp, bool direct, bool ignoreLearnRate)
        {
            if (xp > 0f && __instance != null && __instance.def != null && MegaMod.Settings?.Skills != null)
            {
                if (!MegaMod.Settings.Skills.ShouldApplyTo(__instance.Pawn))
                {
                    return;
                }

                float multiplier = MegaMod.Settings.Skills.GetMultiplierForSkill(__instance.def);
                xp *= multiplier;
            }
        }
    }
}
