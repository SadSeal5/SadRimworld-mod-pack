using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Medical.FieldMedicine
{
    [HarmonyPatch(typeof(WorkGiver_Tend), "GoodLayingStatusForTend")]
    public static class Patch_WorkGiver_Tend
    {
        public static void Postfix(Pawn patient, Pawn doctor, ref bool __result)
        {
            if (__result) return;

            var settings = MedicalModule.ModSettings;
            if (settings == null || !settings.EnableFieldMedicine) return;

            if (patient == null || doctor == null || patient.Dead || patient.health == null) return;

            // Never tend hostile enemies or wild aggressive threats in the field
            if (patient.HostileTo(doctor) || patient.InAggroMentalState)
            {
                return;
            }

            // Allow tending colonists, allies, colony animals, and prisoners/slaves on the spot
            bool isFriendlyOrColony = patient.Faction == doctor.Faction ||
                                      patient.HostFaction == doctor.Faction ||
                                      patient.IsPrisonerOfColony ||
                                      patient.IsSlaveOfColony ||
                                      (patient.Faction != null && !patient.Faction.HostileTo(doctor.Faction));

            if (isFriendlyOrColony && patient.health.HasHediffsNeedingTend())
            {
                __result = true;
            }
        }
    }
}
