using HarmonyLib;
using RimWorld;

namespace SadSeal.MegaMod.Hauling.Patches
{
    [HarmonyPatch(typeof(Pawn_DraftController), nameof(Pawn_DraftController.Drafted), MethodType.Setter)]
    public static class Patch_Pawn_DraftController
    {
        public static void Postfix(Pawn_DraftController __instance, bool value)
        {
            if (value && (HaulModule.ModSettings?.DropOnDraft ?? true))
            {
                if (__instance?.pawn != null)
                {
                    HauledCargoTracker.DropAllHauled(__instance.pawn);
                }
            }
        }
    }
}
