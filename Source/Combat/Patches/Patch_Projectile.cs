using System;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Combat.Patches
{
    public static class Patch_Projectile_Subclasses
    {
        [ThreadStatic] public static bool isDamageHandled;
        [ThreadStatic] public static bool isApHandled;
        [ThreadStatic] public static bool isSpeedHandled;

        public static void SubclassDamagePostfix(ref int __result)
        {
            if (CombatModule.Instance?.IsEnabled != true)
            {
                isDamageHandled = false;
                return;
            }

            var settings = CombatModule.ModSettings;
            if (!isDamageHandled && settings != null && Math.Abs(settings.damageMultiplier - 1f) > 0.001f)
            {
                __result = Mathf.Max(1, Mathf.RoundToInt((float)__result * settings.damageMultiplier));
            }
            isDamageHandled = false;
        }

        public static void SubclassApPostfix(ref float __result)
        {
            if (CombatModule.Instance?.IsEnabled != true)
            {
                isApHandled = false;
                return;
            }

            var settings = CombatModule.ModSettings;
            if (!isApHandled && settings != null && Math.Abs(settings.armorPenetrationMultiplier - 1f) > 0.001f)
            {
                __result = Mathf.Max(0f, __result * settings.armorPenetrationMultiplier);
            }
            isApHandled = false;
        }

        public static void SubclassStartingTicksPostfix(ref float __result)
        {
            if (CombatModule.Instance?.IsEnabled != true)
            {
                isSpeedHandled = false;
                return;
            }

            var settings = CombatModule.ModSettings;
            if (!isSpeedHandled && settings != null && settings.projectileSpeedMultiplier > 0.01f && Math.Abs(settings.projectileSpeedMultiplier - 1f) > 0.001f)
            {
                __result = Mathf.Max(1f, __result / settings.projectileSpeedMultiplier);
            }
            isSpeedHandled = false;
        }
    }

    [HarmonyPatch(typeof(ProjectileProperties), nameof(ProjectileProperties.GetDamageAmount), new Type[] { typeof(float), typeof(Thing), typeof(StringBuilder) })]
    public static class Patch_ProjectileProperties_GetDamageAmount
    {
        [HarmonyPostfix]
        public static void Postfix(ref int __result, StringBuilder explanation)
        {
            if (CombatModule.Instance?.IsEnabled != true) return;

            var settings = CombatModule.ModSettings;
            if (settings != null && Math.Abs(settings.damageMultiplier - 1f) > 0.001f)
            {
                __result = Mathf.Max(1, Mathf.RoundToInt((float)__result * settings.damageMultiplier));
                Patch_Projectile_Subclasses.isDamageHandled = true;
                if (explanation != null)
                {
                    explanation.AppendLine("LethalRanged_DamageMult".Translate() + ": x" + settings.damageMultiplier.ToStringPercent());
                }
            }
        }
    }

    [HarmonyPatch(typeof(ProjectileProperties), nameof(ProjectileProperties.GetArmorPenetration), new Type[] { typeof(Thing), typeof(StringBuilder) })]
    public static class Patch_ProjectileProperties_GetArmorPenetration
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, StringBuilder explanation)
        {
            if (CombatModule.Instance?.IsEnabled != true) return;

            var settings = CombatModule.ModSettings;
            if (settings != null && Math.Abs(settings.armorPenetrationMultiplier - 1f) > 0.001f)
            {
                __result = Mathf.Max(0f, __result * settings.armorPenetrationMultiplier);
                Patch_Projectile_Subclasses.isApHandled = true;
                if (explanation != null)
                {
                    explanation.AppendLine("LethalRanged_ApMult".Translate() + ": x" + settings.armorPenetrationMultiplier.ToStringPercent());
                }
            }
        }
    }

    [HarmonyPatch(typeof(ProjectileProperties), nameof(ProjectileProperties.SpeedTilesPerTick), MethodType.Getter)]
    public static class Patch_ProjectileProperties_SpeedTilesPerTick
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result)
        {
            if (CombatModule.Instance?.IsEnabled != true) return;

            var settings = CombatModule.ModSettings;
            if (settings != null && settings.projectileSpeedMultiplier > 0.01f && Math.Abs(settings.projectileSpeedMultiplier - 1f) > 0.001f)
            {
                __result *= settings.projectileSpeedMultiplier;
                Patch_Projectile_Subclasses.isSpeedHandled = true;
            }
        }
    }

    [HarmonyPatch(typeof(Projectile), "StartingTicksToImpact", MethodType.Getter)]
    public static class Patch_Projectile_StartingTicksToImpact
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result)
        {
            Patch_Projectile_Subclasses.SubclassStartingTicksPostfix(ref __result);
        }
    }
}
