using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Durability.Patches
{
    [HarmonyPatch(typeof(Pawn_ApparelTracker), "TakeWearoutDamageForDay")]
    public static class Patch_ApparelWearout
    {
        public static bool Prefix()
        {
            if (DurabilityModule.ModSettings != null && DurabilityModule.ModSettings.removeApparelDurability)
            {
                return false;
            }
            return true;
        }
    }
}
