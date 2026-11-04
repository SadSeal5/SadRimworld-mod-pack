using HarmonyLib;
using RimWorld;

namespace SadSeal.MegaMod.Durability.Patches
{
    [HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Notify_PawnKilled))]
    public static class Patch_Apparel_Tainted
    {
        public static void Prefix(out bool __state)
        {
            __state = MegaMod.Settings?.Durability?.preventTaintedApparel == true;
        }

        public static void Postfix(Pawn_ApparelTracker __instance, bool __state)
        {
            if (!__state)
            {
                return;
            }

            if (__instance?.WornApparel != null)
            {
                var worn = __instance.WornApparel;
                for (int i = 0; i < worn.Count; i++)
                {
                    var apparel = worn[i];
                    if (apparel != null)
                    {
                        apparel.WornByCorpse = false;
                    }
                }
            }
        }
    }
}
