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
            if (patient == null || patient.health == null) return;

            var settings = MegaMod.Settings?.Medical;
            if (settings == null) return;

            // 1. Field Medicine: If healer has no medicine, check patient's inventory
            if (settings.EnableFieldMedicine && settings.FieldMedicineUsePatientInventory)
            {
                if (__result == null && patient.inventory?.innerContainer != null)
                {
                    Thing patientMed = FindBestMedicineInContainer(patient.inventory.innerContainer, patient);
                    if (patientMed != null)
                    {
                        __result = patientMed;
                    }
                }
            }

            // 2. Smart Medicine rules
            if (__result != null && settings.EnableSmartMedicine)
            {
                // If selected medicine is Glitterworld medicine
                if (settings.NoGlitterOnScratches && __result.def == ThingDefOf.MedicineUltratech)
                {
                    bool isDangerous = IsInCriticalCondition(patient, settings.LethalBleedRateThreshold);
                    if (!isDangerous)
                    {
                        // Trivial scratch/bruise or minor ailment: do not waste glitterworld medicine!
                        Thing lowerTier = FindLowerTierMedicine(healer, patient, onlyUseInventory);
                        if (lowerTier != null)
                        {
                            __result = lowerTier;
                        }
                    }
                }
            }
        }

        private static Thing FindBestMedicineInContainer(ThingOwner container, Pawn patient)
        {
            MedicalCareCategory care = patient.playerSettings?.medCare ?? MedicalCareCategory.Best;
            Thing best = null;
            float bestPotency = -1f;

            for (int i = 0; i < container.Count; i++)
            {
                Thing item = container[i];
                if (item?.def != null && item.def.IsMedicine && MedicalCareUtility.AllowsMedicine(care, item.def))
                {
                    float potency = item.GetStatValue(StatDefOf.MedicalPotency);
                    if (potency > bestPotency)
                    {
                        bestPotency = potency;
                        best = item;
                    }
                }
            }

            return best;
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
            MedicalCareCategory care = patient?.playerSettings?.medCare ?? MedicalCareCategory.Best;

            // Check healer inventory for industrial medicine first, then herbal
            if (healer?.inventory?.innerContainer != null)
            {
                for (int i = 0; i < healer.inventory.innerContainer.Count; i++)
                {
                    Thing item = healer.inventory.innerContainer[i];
                    if ((item.def == ThingDefOf.MedicineIndustrial || item.def == ThingDefOf.MedicineHerbal) &&
                        MedicalCareUtility.AllowsMedicine(care, item.def))
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
                    if ((item.def == ThingDefOf.MedicineIndustrial || item.def == ThingDefOf.MedicineHerbal) &&
                        MedicalCareUtility.AllowsMedicine(care, item.def))
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
                    t => (t.def == ThingDefOf.MedicineIndustrial || t.def == ThingDefOf.MedicineHerbal) &&
                         !t.IsForbidden(healer) && healer.CanReserve(t) && MedicalCareUtility.AllowsMedicine(care, t.def)
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
