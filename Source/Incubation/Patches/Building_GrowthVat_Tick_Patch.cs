using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Incubation.Patches
{
    [HarmonyPatch(typeof(Building_GrowthVat), "Tick")]
    public static class Building_GrowthVat_Tick_Patch
    {
        [HarmonyPrepare]
        public static bool Prepare() => ModLister.BiotechInstalled;

        private static readonly AccessTools.FieldRef<Building_Enterable, int> StartTickRef =
            AccessTools.FieldRefAccess<Building_Enterable, int>("startTick");

        private static readonly Dictionary<int, float> embryoAccumulators = new Dictionary<int, float>();

        [HarmonyPrefix]
        public static bool Prefix(Building_GrowthVat __instance)
        {
            if (!IncubationModule.Instance.IsEnabled) return true;

            var settings = IncubationModule.Settings;
            if (settings.autoEjectEnabled && __instance.SelectedPawn != null && __instance.innerContainer.Contains(__instance.SelectedPawn))
            {
                Pawn pawn = __instance.SelectedPawn;
                if (pawn.ageTracker != null && pawn.ageTracker.AgeBiologicalYearsFloat >= settings.targetEjectAge)
                {
                    int targetAge = settings.targetEjectAge;
                    __instance.EjectContents();
                    Messages.Message("SadSeal_Incubation_AutoEjectedTargetAge".Translate(pawn.Named("PAWN"), targetAge.Named("TARGETAGE")), pawn, MessageTypeDefOf.PositiveEvent);
                    return false;
                }
            }

            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(Building_GrowthVat __instance)
        {
            if (!IncubationModule.Instance.IsEnabled) return;

            var settings = IncubationModule.Settings;

            // Power consumption adjustment
            if (__instance.IsHashIntervalTick(250))
            {
                CompPowerTrader power = __instance.TryGetComp<CompPowerTrader>();
                if (power != null && power.PowerOn)
                {
                    float mult = settings.powerUsageMultiplier;
                    CompPower powerComp = __instance.PowerComp;
                    float baseDraw = powerComp != null
                        ? (__instance.Working ? (0f - powerComp.Props.PowerConsumption) : (0f - powerComp.Props.idlePowerDraw))
                        : 0f;
                    power.PowerOutput = baseDraw * mult;
                }
            }

            // Embryo gestation speed & safe stasis
            if (__instance.selectedEmbryo != null && __instance.innerContainer.Contains(__instance.selectedEmbryo))
            {
                bool unpoweredOrStarving = !__instance.PowerOn || __instance.NutritionStored <= 0f;
                if (settings.stasisProtection && unpoweredOrStarving)
                {
                    // Safe stasis: freeze embryo gestation progress
                    StartTickRef(__instance)++;
                    return;
                }

                float mult = settings.embryoSpeedMultiplier;
                if (Math.Abs(mult - 1.0f) > 0.001f)
                {
                    float extra = mult - 1.0f;
                    int id = __instance.thingIDNumber;
                    if (!embryoAccumulators.TryGetValue(id, out float acc))
                    {
                        acc = 0f;
                    }
                    acc += extra;
                    int ticksToShift = (int)acc;
                    if (ticksToShift != 0)
                    {
                        StartTickRef(__instance) -= ticksToShift;
                        acc -= ticksToShift;
                    }
                    embryoAccumulators[id] = acc;
                }
            }
        }
    }
}
