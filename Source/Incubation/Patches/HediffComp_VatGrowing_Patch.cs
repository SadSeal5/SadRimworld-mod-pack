using HarmonyLib;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Incubation.Patches
{
    [HarmonyPatch(typeof(HediffComp_VatGrowing), nameof(HediffComp_VatGrowing.CompTipStringExtra), MethodType.Getter)]
    public static class HediffComp_VatGrowing_Patch
    {
        [HarmonyPrepare]
        public static bool Prepare() => ModsConfig.BiotechActive;

        [HarmonyPostfix]
        public static void Postfix(ref string __result)
        {
            if (IncubationModule.Instance.IsEnabled)
            {
                float mult = IncubationModule.Settings.childSpeedMultiplier;
                int totalSpeed = Mathf.RoundToInt(20f * mult);
                __result = "AgingSpeed".Translate() + $": x{totalSpeed}";
            }
        }
    }
}
