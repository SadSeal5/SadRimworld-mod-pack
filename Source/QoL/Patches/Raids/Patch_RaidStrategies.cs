using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.QoL.Patches.Raids
{
    public static class Patch_RaidStrategies
    {
        [HarmonyPatch(typeof(RaidStrategyWorker), nameof(RaidStrategyWorker.CanUseWith))]
        public static class Patch_RaidStrategyWorker_CanUseWith
        {
            public static void Postfix(RaidStrategyWorker __instance, IncidentParms parms, ref bool __result)
            {
                if (!__result || __instance.def == null) return;

                var settings = MegaMod.Settings?.QoL;
                if (settings == null) return;

                string defName = __instance.def.defName;

                // Disable breach raids
                if (settings.disableBreachRaids && defName.IndexOf("breach", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    __result = false;
                    return;
                }

                // Disable drop pod raids
                if (settings.disableDropPodRaids)
                {
                    if (defName.IndexOf("drop", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        __result = false;
                        return;
                    }

                    if (parms?.raidArrivalMode?.defName != null &&
                        parms.raidArrivalMode.defName.IndexOf("drop", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        __result = false;
                        return;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PawnsArrivalModeWorker), nameof(PawnsArrivalModeWorker.CanUseWith))]
        public static class Patch_PawnsArrivalModeWorker_CanUseWith
        {
            public static void Postfix(PawnsArrivalModeWorker __instance, ref bool __result)
            {
                if (!__result || __instance.def == null) return;

                var settings = MegaMod.Settings?.QoL;
                if (settings == null || !settings.disableDropPodRaids) return;

                string defName = __instance.def.defName;
                if (defName.IndexOf("drop", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    __result = false;
                }
            }
        }
    }
}
