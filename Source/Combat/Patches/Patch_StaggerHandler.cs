using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Combat.Patches
{
    [HarmonyPatch(typeof(StaggerHandler), nameof(StaggerHandler.Notify_BulletImpact))]
    public static class Patch_StaggerHandler
    {
        [HarmonyPrefix]
        public static bool Prefix(StaggerHandler __instance, Bullet bullet)
        {
            var settings = CombatModule.ModSettings;
            if (settings != null && settings.enableStaggerEnhancement)
            {
                if (bullet != null && __instance != null && __instance.parent != null)
                {
                    float stoppingPower = bullet.def?.projectile?.stoppingPower ?? 1f;
                    float bodySize = __instance.parent.BodySize;
                    if (stoppingPower < bodySize * 0.75f)
                    {
                        return true; // Use vanilla handling (insufficient stopping power to stagger giant creatures)
                    }

                    int baseTicks = __instance.parent.RaceProps?.bulletStaggerDelayTicks ?? 75;
                    int staggerTicks = Mathf.RoundToInt((float)baseTicks * Mathf.Max(0.8f, stoppingPower));
                    __instance.StaggerFor(staggerTicks, settings.staggerMoveSpeedFactor);
                }
                return false;
            }
            return true;
        }
    }
}
