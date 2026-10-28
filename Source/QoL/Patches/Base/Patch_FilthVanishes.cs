using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.QoL.Patches.Base
{
    [HarmonyPatch(typeof(Filth), nameof(Filth.TickRare))]
    public static class Patch_FilthVanishes
    {
        public static void Postfix(Filth __instance)
        {
            if (MegaMod.Settings?.QoL?.filthVanishes != true) return;
            if (!__instance.Spawned || __instance.Map == null) return;

            Map map = __instance.Map;
            IntVec3 pos = __instance.Position;

            // Outdoor precipitation rapidly washes away filth
            if (map.weatherManager.RainRate > 0.05f && !pos.Roofed(map))
            {
                __instance.ThinFilth();
                return;
            }

            // Filth naturally dissipates over time (~1% chance per rare tick = ~7 minutes average lifetime)
            if (Rand.Chance(0.01f))
            {
                __instance.ThinFilth();
            }
        }
    }
}
