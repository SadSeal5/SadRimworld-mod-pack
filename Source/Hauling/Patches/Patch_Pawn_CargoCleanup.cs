using HarmonyLib;
using Verse;

namespace SadSeal.MegaMod.Hauling.Patches
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.Kill))]
    public static class Patch_Pawn_Kill_CargoCleanup
    {
        public static void Postfix(Pawn __instance)
        {
            HauledCargoTracker.Clear(__instance);
        }
    }

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.DeSpawn))]
    public static class Patch_Pawn_DeSpawn_CargoCleanup
    {
        public static void Prefix(Pawn __instance)
        {
            if (__instance != null && __instance.Spawned && __instance.Map != null)
            {
                HauledCargoTracker.DropAllHauled(__instance);
            }
        }
    }
}
