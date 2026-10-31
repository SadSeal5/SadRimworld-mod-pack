using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.QoL.Patches.Pawn
{
    [HarmonyPatch(typeof(StoreUtility), nameof(StoreUtility.IsGoodStoreCell))]
    public static class Patch_DontBlockDoor
    {
        public static void Postfix(IntVec3 c, Map map, ref bool __result)
        {
            if (!__result) return;
            if (QoLModule.ModSettings?.dontBlockDoors != true) return;
            if (map == null) return;

            // Prevent items from being hauled or stored directly onto door tiles
            Building edifice = c.GetEdifice(map);
            if (edifice is Building_Door || c.GetDoor(map) != null)
            {
                __result = false;
            }
        }
    }
}
