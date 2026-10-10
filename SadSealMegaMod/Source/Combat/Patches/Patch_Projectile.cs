using System;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Combat.Patches
{
    [HarmonyPatch(typeof(Projectile), "StartingTicksToImpact", MethodType.Getter)]
    public static class Patch_Projectile_StartingTicksToImpact
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result)
        {
            var settings = MegaMod.Settings?.Combat;
            if (settings != null && settings.projectileSpeedMultiplier > 0.01f && Math.Abs(settings.projectileSpeedMultiplier - 1.0f) > 0.001f)
            {
                __result = Mathf.Max(1f, __result / settings.projectileSpeedMultiplier);
            }
        }
    }

    [HarmonyPatch(typeof(Projectile), "DamageAmount", MethodType.Getter)]
    public static class Patch_Projectile_DamageAmount
    {
        [HarmonyPostfix]
        public static void Postfix(ref int __result)
        {
            var settings = MegaMod.Settings?.Combat;
            if (settings != null && Math.Abs(settings.damageMultiplier - 1.0f) > 0.001f)
            {
                __result = Mathf.Max(1, Mathf.RoundToInt((float)__result * settings.damageMultiplier));
            }
        }
    }

    [HarmonyPatch(typeof(Projectile), "ArmorPenetration", MethodType.Getter)]
    public static class Patch_Projectile_ArmorPenetration
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result)
        {
            var settings = MegaMod.Settings?.Combat;
            if (settings != null && Math.Abs(settings.armorPenetrationMultiplier - 1.0f) > 0.001f)
            {
                __result = Mathf.Max(0f, __result * settings.armorPenetrationMultiplier);
            }
        }
    }
}
