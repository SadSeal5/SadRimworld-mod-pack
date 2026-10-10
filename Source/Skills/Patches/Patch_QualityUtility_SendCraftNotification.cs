using HarmonyLib;
using RimWorld;

namespace SadSeal.MegaMod.Skills.Patches
{
    [HarmonyPatch(typeof(QualityUtility), nameof(QualityUtility.SendCraftNotification))]
    public static class Patch_QualityUtility_SendCraftNotification
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            if (MegaMod.Settings?.Skills != null && MegaMod.Settings.Skills.muteMasterworkLegendaryNotifications)
            {
                // Silences the loud letter sound effect and prevents the masterwork/legendary popup letter.
                return false;
            }
            return true;
        }
    }
}
