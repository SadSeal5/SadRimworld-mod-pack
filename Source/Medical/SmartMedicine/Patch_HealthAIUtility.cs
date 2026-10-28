using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.Medical.SmartMedicine
{
    [HarmonyPatch(typeof(HealthAIUtility), nameof(HealthAIUtility.FindBestMedicine))]
    public static class Patch_HealthAIUtility
    {
        public static void Postfix(Pawn healer, Pawn patient, bool onlyUseInventory, ref Thing __result)
        {
            if (__result == null || patient == null || patient.health == null) return;

            var settings = MegaMod.Settings?.Medical;
            if (settings == null || !settings.EnableSmartMedicine) return;

            // If selected medicine is Glitterworld medicine
            if (settings.NoGlitterOnScratches && __result.def == ThingDefOf.MedicineUltratech)
            {
                bool isDangerous = IsInCriticalCondition(patient, settings.LethalBleedRateThreshold);

                if (!isDangerous)
                {
                    // Trivial scratch/bruise or minor ailment: do not waste glitterworld medicine!
                    // Find lower tier medicine (Industrial or Herbal)
                    Thing lowerTier = FindLowerTierMedicine(healer, patient, onlyUseInventory);
                    if (lowerTier != null)
                    {
                        __result = lowerTier;
                    }
                }
            }
        }

        private static bool IsInCriticalCondition(Pawn patient, float bleedRateThreshold)
        {
            var hediffSet = patient.health.hediffSet;
            if (hediffSet == null) return false;

            // Check bleed rate
            if (hediffSet.BleedRateTotal >= bleedRateThreshold)
            {
                return true;
            }

            // Check deadly disease or infection
            List<Hediff> hediffs = hediffSet.hediffs;
            for (int i = 0; i < hediffs.Count; i++)
            {
                Hediff h = hediffs[i];
                if (h.def.lethalSeverity > 0f && (h.Severity >= 0.40f || h.CurStage?.lifeThreatening == true))
                {
                    return true;
                }
            }

            return false;
        }

        private static Thing FindLowerTierMedicine(Pawn healer, Pawn patient, bool onlyUseInventory)
        {
            // Check healer inventory for industrial medicine first, then herbal
            if (healer?.inventory?.innerContainer != null)
            {
                for (int i = 0; i < healer.inventory.innerContainer.Count; i++)
                {
                    Thing item = healer.inventory.innerContainer[i];
                    if (item.def == ThingDefOf.MedicineIndustrial || item.def == ThingDefOf.MedicineHerbal)
                    {
                        return item;
                    }
                }
            }

            // Check patient inventory
            if (patient?.inventory?.innerContainer != null)
            {
                for (int i = 0; i < patient.inventory.innerContainer.Count; i++)
                {
                    Thing item = patient.inventory.innerContainer[i];
                    if (item.def == ThingDefOf.MedicineIndustrial || item.def == ThingDefOf.MedicineHerbal)
                    {
                        return item;
                    }
                }
            }

            // If not restricted to inventory, search map nearby
            if (!onlyUseInventory && healer != null && healer.Map != null)
            {
                TraverseParms traverseParams = TraverseParms.For(healer);
                Thing found = GenClosest.ClosestThing_Global_Reachable(
                    healer.Position,
                    healer.Map,
                    healer.Map.listerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.Medicine)),
                    Verse.AI.PathEndMode.ClosestTouch,
                    traverseParams,
                    30f,
                    t => (t.def == ThingDefOf.MedicineIndustrial || t.def == ThingDefOf.MedicineHerbal) && !t.IsForbidden(healer) && healer.CanReserve(t)
                );
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
    }
}
