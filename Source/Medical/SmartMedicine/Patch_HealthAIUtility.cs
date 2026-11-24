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
            if (MedicalModule.Instance?.IsEnabled != true) return;
            if (patient == null || patient.health == null) return;

            var settings = MedicalModule.ModSettings;
            if (settings == null) return;

            MedicalCareCategory care = patient.playerSettings?.medCare ?? MedicalCareCategory.NoMeds;
            if ((int)care <= (int)MedicalCareCategory.NoMeds) return;
            if (Medicine.GetMedicineCountToFullyHeal(patient) <= 0) return;

            // 1. Field Medicine: If healer has no medicine, check patient's inventory
            if (!onlyUseInventory && settings.EnableFieldMedicine && settings.FieldMedicineUsePatientInventory)
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
                if (__result.def == ThingDefOf.MedicineUltratech)
                {
                    bool isDangerous = IsInCriticalCondition(patient, settings.LethalBleedRateThreshold);
                    bool shouldDowngrade = !isDangerous &&
                        (settings.NoGlitterOnScratches || settings.SaveBestMedicineForLethal);

                    if (shouldDowngrade)
                    {
                        // Trivial scratch/bruise or minor ailment: do not waste glitterworld medicine!
                        Thing lowerTier = FindLowerTierMedicine(healer, patient, onlyUseInventory, settings);
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
            MedicalCareCategory care = patient.playerSettings?.medCare ?? MedicalCareCategory.NoMeds;
            return FindHighestPotencyMedicineInContainer(container, care, lowerTierOnly: false, ultratechPotency: 0f);
        }

        private static bool IsLowerTierMedicine(Thing item, MedicalCareCategory care, float ultratechPotency)
        {
            return item?.def != null &&
                   item.def.IsMedicine &&
                   item.def != ThingDefOf.MedicineUltratech &&
                   item.GetStatValue(StatDefOf.MedicalPotency) < ultratechPotency &&
                   MedicalCareUtility.AllowsMedicine(care, item.def);
        }

        private static Thing FindHighestPotencyMedicineInContainer(ThingOwner container, MedicalCareCategory care, bool lowerTierOnly, float ultratechPotency)
        {
            if (container == null) return null;

            Thing best = null;
            float bestPotency = -1f;

            for (int i = 0; i < container.Count; i++)
            {
                Thing item = container[i];
                if (item?.def == null || !item.def.IsMedicine || !MedicalCareUtility.AllowsMedicine(care, item.def))
                {
                    continue;
                }

                float potency = item.GetStatValue(StatDefOf.MedicalPotency);
                if (lowerTierOnly && (item.def == ThingDefOf.MedicineUltratech || potency >= ultratechPotency))
                {
                    continue;
                }

                if (potency > bestPotency)
                {
                    bestPotency = potency;
                    best = item;
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

        private static Thing FindLowerTierMedicine(Pawn healer, Pawn patient, bool onlyUseInventory, MedicalSettings settings)
        {
            MedicalCareCategory care = patient?.playerSettings?.medCare ?? MedicalCareCategory.NoMeds;
            float ultratechPotency = ThingDefOf.MedicineUltratech != null
                ? ThingDefOf.MedicineUltratech.GetStatValueAbstract(StatDefOf.MedicalPotency)
                : 1.6f;

            // Check healer inventory for highest-potency lower-tier medicine
            Thing healerMed = FindHighestPotencyMedicineInContainer(healer?.inventory?.innerContainer, care, lowerTierOnly: true, ultratechPotency);
            if (healerMed != null)
            {
                return healerMed;
            }

            // Check patient inventory only when allowed and Field Medicine patient inventory is enabled
            if (!onlyUseInventory && settings.EnableFieldMedicine && settings.FieldMedicineUsePatientInventory)
            {
                Thing patientMed = FindHighestPotencyMedicineInContainer(patient?.inventory?.innerContainer, care, lowerTierOnly: true, ultratechPotency);
                if (patientMed != null)
                {
                    return patientMed;
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
                    t => IsLowerTierMedicine(t, care, ultratechPotency) &&
                         !t.IsForbidden(healer) && healer.CanReserve(t, 10, 1)
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
