using HarmonyLib;
using Verse;

namespace SadSeal.MegaMod.Hauling.Patches
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.Kill))]
    public static class Patch_Pawn_Kill
    {
        public static void Postfix(Pawn __instance)
        {
            HauledCargoTracker.Clear(__instance);
        }
    }
}
