using System;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Combat.Patches
{
    public static class Patch_Projectile_Subclasses
    {
        [ThreadStatic] private static int insideGetArmorPenetrationDepth;

        [ThreadStatic] private static int damageSubclassDepth;
        [ThreadStatic] private static bool damageHandledInScope;

        [ThreadStatic] private static int apSubclassDepth;
        [ThreadStatic] private static bool apHandledInScope;

        [ThreadStatic] private static int speedSubclassDepth;
        [ThreadStatic] private static bool speedHandledInScope;

        internal static bool IsInsideGetArmorPenetration => insideGetArmorPenetrationDepth > 0;

        internal static void BeginGetArmorPenetration()
        {
            insideGetArmorPenetrationDepth++;
        }

        internal static void EndGetArmorPenetration()
        {
            if (insideGetArmorPenetrationDepth > 0)
            {
                insideGetArmorPenetrationDepth--;
            }
        }

        internal static void MarkDamageHandled()
        {
            if (damageSubclassDepth > 0)
            {
                damageHandledInScope = true;
            }
        }

        internal static void MarkApHandled()
        {
            if (apSubclassDepth > 0)
            {
                apHandledInScope = true;
            }
        }

        internal static void MarkSpeedHandled()
        {
            if (speedSubclassDepth > 0)
            {
                speedHandledInScope = true;
            }
        }

        private static void EnterScope(ref int depth, ref bool handled)
        {
            if (depth == 0)
            {
                handled = false;
            }
            depth++;
        }

        private static void ExitScope(ref int depth, ref bool handled)
        {
            if (depth > 0)
            {
                depth--;
            }
            if (depth == 0)
            {
                handled = false;
            }
        }

        public static void SubclassDamagePrefix()
        {
            EnterScope(ref damageSubclassDepth, ref damageHandledInScope);
        }

        public static void SubclassDamagePostfix(ref int __result)
        {
            if (CombatModule.Instance?.IsEnabled != true || damageSubclassDepth != 1 || damageHandledInScope || __result <= 0)
            {
                return;
            }

            var settings = CombatModule.ModSettings;
            if (settings != null && Math.Abs(settings.damageMultiplier - 1f) > 0.001f)
            {
                __result = Mathf.Max(1, Mathf.RoundToInt(__result * settings.damageMultiplier));
                damageHandledInScope = true;
            }
        }

        public static void SubclassDamageFinalizer()
        {
            ExitScope(ref damageSubclassDepth, ref damageHandledInScope);
        }

        public static void SubclassApPrefix()
        {
            EnterScope(ref apSubclassDepth, ref apHandledInScope);
        }

        public static void SubclassApPostfix(ref float __result)
        {
            if (CombatModule.Instance?.IsEnabled != true || apSubclassDepth != 1 || apHandledInScope || __result <= 0f)
            {
                return;
            }

            var settings = CombatModule.ModSettings;
            if (settings != null && Math.Abs(settings.armorPenetrationMultiplier - 1f) > 0.001f)
            {
                __result = Mathf.Max(0f, __result * settings.armorPenetrationMultiplier);
                apHandledInScope = true;
            }
        }

        public static void SubclassApFinalizer()
        {
            ExitScope(ref apSubclassDepth, ref apHandledInScope);
        }

        public static void SubclassStartingTicksPrefix()
        {
            EnterScope(ref speedSubclassDepth, ref speedHandledInScope);
        }

        public static void SubclassStartingTicksPostfix(ref float __result)
        {
            if (CombatModule.Instance?.IsEnabled != true || speedSubclassDepth != 1 || speedHandledInScope || __result <= 0f)
            {
                return;
            }

            var settings = CombatModule.ModSettings;
            if (settings != null && settings.projectileSpeedMultiplier > 0.01f && Math.Abs(settings.projectileSpeedMultiplier - 1f) > 0.001f)
            {
                __result = Mathf.Max(1f, __result / settings.projectileSpeedMultiplier);
                speedHandledInScope = true;
            }
        }

        public static void SubclassStartingTicksFinalizer()
        {
            ExitScope(ref speedSubclassDepth, ref speedHandledInScope);
        }
    }

    [HarmonyPatch(typeof(ProjectileProperties), nameof(ProjectileProperties.GetDamageAmount), new Type[] { typeof(float), typeof(Thing), typeof(StringBuilder) })]
    public static class Patch_ProjectileProperties_GetDamageAmount
    {
        [HarmonyPostfix]
        public static void Postfix(ref int __result, StringBuilder explanation)
        {
            if (CombatModule.Instance?.IsEnabled != true) return;
            if (Patch_Projectile_Subclasses.IsInsideGetArmorPenetration) return;
            if (__result <= 0) return;

            var settings = CombatModule.ModSettings;
            if (settings != null && Math.Abs(settings.damageMultiplier - 1f) > 0.001f)
            {
                __result = Mathf.Max(1, Mathf.RoundToInt(__result * settings.damageMultiplier));
                Patch_Projectile_Subclasses.MarkDamageHandled();
                if (explanation != null)
                {
                    explanation.AppendLine("LethalRanged_DamageMult".Translate() + ": x" + settings.damageMultiplier.ToStringPercent() + " (" + __result + ")");
                }
            }
        }
    }

    [HarmonyPatch(typeof(ProjectileProperties), nameof(ProjectileProperties.GetArmorPenetration), new Type[] { typeof(Thing), typeof(StringBuilder) })]
    public static class Patch_ProjectileProperties_GetArmorPenetration
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            Patch_Projectile_Subclasses.BeginGetArmorPenetration();
        }

        [HarmonyPostfix]
        public static void Postfix(ref float __result, StringBuilder explanation)
        {
            if (CombatModule.Instance?.IsEnabled != true || __result <= 0f) return;

            var settings = CombatModule.ModSettings;
            if (settings != null && Math.Abs(settings.armorPenetrationMultiplier - 1f) > 0.001f)
            {
                __result = Mathf.Max(0f, __result * settings.armorPenetrationMultiplier);
                Patch_Projectile_Subclasses.MarkApHandled();
                if (explanation != null)
                {
                    explanation.AppendLine("LethalRanged_ApMult".Translate() + ": x" + settings.armorPenetrationMultiplier.ToStringPercent() + " (" + __result.ToStringPercent() + ")");
                }
            }
        }

        [HarmonyFinalizer]
        public static void Finalizer()
        {
            Patch_Projectile_Subclasses.EndGetArmorPenetration();
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
                Patch_Projectile_Subclasses.MarkSpeedHandled();
            }
        }
    }

    [HarmonyPatch(typeof(Projectile), "StartingTicksToImpact", MethodType.Getter)]
    public static class Patch_Projectile_StartingTicksToImpact
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            Patch_Projectile_Subclasses.SubclassStartingTicksPrefix();
        }

        [HarmonyPostfix]
        public static void Postfix(ref float __result)
        {
            Patch_Projectile_Subclasses.SubclassStartingTicksPostfix(ref __result);
        }

        [HarmonyFinalizer]
        public static void Finalizer()
        {
            Patch_Projectile_Subclasses.SubclassStartingTicksFinalizer();
        }
    }
}
