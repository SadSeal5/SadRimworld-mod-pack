using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.QoL.Patches.Base
{
    [HarmonyPatch(typeof(Plant), nameof(Plant.TickLong))]
    public static class Patch_TreesDontDie
    {
        public static void Prefix(Plant __instance)
        {
            if (QoLModule.ModSettings?.treesDontDie != true) return;
            if (__instance.def?.plant == null || !__instance.def.plant.IsTree) return;

            // Keep mature trees from reaching fatal rot/old age
            if (__instance.Age >= __instance.def.plant.LifespanTicks - 10000 && __instance.def.plant.LifespanTicks > 0)
            {
                __instance.Age = __instance.def.plant.LifespanTicks / 2;
            }
        }
    }
}
