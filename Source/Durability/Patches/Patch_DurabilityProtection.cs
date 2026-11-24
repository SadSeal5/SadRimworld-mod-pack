using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Durability.Patches
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.TakeDamage))]
    public static class Patch_Thing_TakeDamage
    {
        public static bool Prefix(Thing __instance, DamageInfo dinfo, ref DamageWorker.DamageResult __result)
        {
            if (!DurabilityApplier.IsProtected(__instance)) return true;
            if (dinfo.Def == DamageDefOf.Flame && __instance.Spawned && __instance.FlammableNow) return true;

            __result = new DamageWorker.DamageResult();
            return false;
        }
    }

    [HarmonyPatch(typeof(SteadyEnvironmentEffects), nameof(SteadyEnvironmentEffects.FinalDeteriorationRate), new[] { typeof(Thing), typeof(bool), typeof(bool), typeof(TerrainDef), typeof(List<string>) })]
    public static class Patch_SteadyEnvironmentEffects_FinalDeteriorationRate
    {
        public static bool Prefix(Thing t, ref float __result)
        {
            if (DurabilityApplier.IsProtected(t))
            {
                __result = 0f;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Thing), nameof(Thing.ExposeData))]
    public static class Patch_Thing_ExposeData
    {
        public static void Postfix(Thing __instance)
        {
            if (Scribe.mode != LoadSaveMode.PostLoadInit) return;
            if (__instance?.def == null || !__instance.def.useHitPoints || __instance.Destroyed) return;
            if (!__instance.def.IsWeapon && !__instance.def.IsApparel) return;

            if (__instance.HitPoints <= 0)
            {
                __instance.HitPoints = __instance.MaxHitPoints;
                return;
            }

            if (DurabilityModule.ModSettings?.restoreExistingOnLoad == true &&
                DurabilityApplier.IsProtected(__instance) &&
                __instance.HitPoints < __instance.MaxHitPoints)
            {
                __instance.HitPoints = __instance.MaxHitPoints;
            }
        }
    }
}
