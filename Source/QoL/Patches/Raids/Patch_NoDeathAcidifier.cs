using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.QoL.Patches.Raids
{
    [HarmonyPatch(typeof(HediffComp_DissolveGearOnDeath), nameof(HediffComp_DissolveGearOnDeath.Notify_PawnDied))]
    public static class Patch_NoDeathAcidifier
    {
        public static bool Prefix()
        {
            if (QoLModule.ModSettings?.noDeathAcidifiers == true)
            {
                // Disables acidifier dissolve mechanism entirely; gear drops intact
                return false;
            }

            return true;
        }
    }
}
