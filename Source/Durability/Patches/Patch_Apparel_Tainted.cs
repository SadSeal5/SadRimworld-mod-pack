using HarmonyLib;
using RimWorld;

namespace SadSeal.MegaMod.Durability.Patches
{
    [HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Notify_PawnKilled))]
    public static class Patch_Apparel_Tainted
    {
        public static bool SuppressTaint = false;

        public static void Prefix()
        {
            if (DurabilityModule.ModSettings?.preventTaintedApparel == true)
            {
                SuppressTaint = true;
            }
        }

        public static void Postfix(Pawn_ApparelTracker __instance)
        {
            if (SuppressTaint)
            {
                SuppressTaint = false;
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
}
