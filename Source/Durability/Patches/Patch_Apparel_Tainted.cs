using System.Collections.Generic;
using HarmonyLib;
using RimWorld;

namespace SadSeal.MegaMod.Durability.Patches
{
    [HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Notify_PawnKilled))]
    public static class Patch_Apparel_Tainted
    {
        public static void Prefix(Pawn_ApparelTracker __instance, out List<Apparel> __state)
        {
            __state = null;
            if (DurabilityModule.Instance?.IsEnabled != true ||
                DurabilityModule.ModSettings?.preventTaintedApparel != true ||
                __instance?.WornApparel == null)
            {
                return;
            }

            var worn = __instance.WornApparel;
            for (int i = 0; i < worn.Count; i++)
            {
                var apparel = worn[i];
                if (apparel != null && !apparel.WornByCorpse)
                {
                    if (__state == null)
                    {
                        __state = new List<Apparel>(worn.Count);
                    }
                    __state.Add(apparel);
                }
            }
        }

        public static void Postfix(List<Apparel> __state)
        {
            if (__state == null) return;

            for (int i = 0; i < __state.Count; i++)
            {
                var apparel = __state[i];
                if (apparel != null)
                {
                    apparel.WornByCorpse = false;
                }
            }
        }
    }
}
