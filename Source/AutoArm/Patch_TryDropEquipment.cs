using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.AutoArm
{
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.TryDropEquipment))]
    public static class Patch_TryDropEquipment
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn_EquipmentTracker __instance, ref ThingWithComps resultingEq)
        {
            if (resultingEq != null && resultingEq.Spawned && __instance?.pawn?.Faction?.IsPlayer == true)
            {
                var settings = AutoArmModule.ModSettings;
                if (AutoArmModule.Instance?.IsEnabled != false && settings != null && settings.enableAutoArm && settings.autoUnforbidDropped)
                {
                    resultingEq.SetForbidden(false, false);
                }
            }
        }
    }
}
