using HarmonyLib;
using Verse;

namespace SadSeal.MegaMod.Durability.Patches
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.SpawnSetup))]
    public static class Patch_Thing_SpawnSetup
    {
        public static void Postfix(Thing __instance)
        {
            if (__instance?.def == null) return;

            if (MegaMod.Settings?.Durability?.restoreExistingOnLoad == true &&
                !__instance.def.useHitPoints &&
                (__instance.def.IsWeapon || __instance.def.IsApparel))
            {
                if (__instance.HitPoints < __instance.MaxHitPoints)
                {
                    __instance.HitPoints = __instance.MaxHitPoints;
                }
            }
        }
    }
}
