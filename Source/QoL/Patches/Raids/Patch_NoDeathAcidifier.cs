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
            if (QoLModule.Instance?.IsEnabled == true && QoLModule.ModSettings?.noDeathAcidifiers == true)
            {
                // Disables acidifier injury/dissolve mechanism on death
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(HediffComp_DissolveGearOnDeath), nameof(HediffComp_DissolveGearOnDeath.Notify_PawnKilled))]
    public static class Patch_NoDeathAcidifier_Killed
    {
        public static bool Prefix()
        {
            if (QoLModule.Instance?.IsEnabled == true && QoLModule.ModSettings?.noDeathAcidifiers == true)
            {
                // Disables acidifier gear destruction; gear drops intact
                return false;
            }

            return true;
        }
    }
}
