using System;
using HarmonyLib;
using Verse;

namespace SadSeal.MegaMod.Combat.Patches
{
    [HarmonyPatch(typeof(Verb_ShootBeam), "ApplyDamage")]
    public static class Patch_BeamWeapon
    {
        [HarmonyPrefix]
        public static void Prefix(ref float damageFactor)
        {
            if (CombatModule.Instance?.IsEnabled != true) return;

            var settings = CombatModule.ModSettings;
            if (settings != null && Math.Abs(settings.damageMultiplier - 1.0f) > 0.001f)
            {
                damageFactor *= settings.damageMultiplier;
            }
        }
    }
}
