using HarmonyLib;
using Verse;

namespace SadSeal.MegaMod.Durability.Patches
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.SpawnSetup))]
    public static class Patch_Thing_SpawnSetup
    {
        public static void Postfix(Thing __instance, bool respawningAfterLoad)
        {
            if (__instance?.def == null || !__instance.def.useHitPoints || __instance.Destroyed) return;

            if ((__instance.def.IsWeapon || __instance.def.IsApparel) && __instance.HitPoints <= 0)
            {
                __instance.HitPoints = __instance.MaxHitPoints;
                return;
            }

            if (respawningAfterLoad &&
                DurabilityModule.ModSettings?.restoreExistingOnLoad == true &&
                DurabilityApplier.IsProtected(__instance) &&
                __instance.HitPoints < __instance.MaxHitPoints)
            {
                __instance.HitPoints = __instance.MaxHitPoints;
            }
        }
    }
}
