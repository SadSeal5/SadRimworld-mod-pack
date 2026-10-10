using System;
using HarmonyLib;
using Verse;

namespace SadSeal.MegaMod.Combat.Patches
{
    [HarmonyPatch(typeof(ShotReport), nameof(ShotReport.HitFactorFromShooter), new Type[] { typeof(Thing), typeof(float), typeof(float?) })]
    public static class Patch_ShotReport_HitFactor
    {
        [HarmonyPrefix]
        public static bool Prefix(Thing caster, float distance, float? acc, ref float __result)
        {
            if (MegaMod.Settings?.Combat != null && MegaMod.Settings.Combat.enableAccuracyOverhaul)
            {
                __result = AccuracyCalculator.CalculateShooterFactor(caster, distance, acc);
                return false;
            }
            return true;
        }
    }
}
