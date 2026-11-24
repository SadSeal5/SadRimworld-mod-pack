using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.Medical.FieldMedicine
{
    [HarmonyPatch(typeof(WorkGiver_Tend), "GoodLayingStatusForTend")]
    public static class Patch_WorkGiver_Tend
    {
        public static void Postfix(Pawn patient, Pawn doctor, ref bool __result)
        {
            if (MedicalModule.Instance?.IsEnabled != true) return;
            if (__result) return;

            var settings = MedicalModule.ModSettings;
            if (settings == null || !settings.EnableFieldMedicine) return;

            if (patient == null || doctor == null || patient.Dead || patient.health == null) return;
            if (patient.Drafted) return;

            // Never tend hostile enemies or wild aggressive threats in the field
            if (patient.HostileTo(doctor) || patient.InAggroMentalState)
            {
                return;
            }

            // Allow tending colonists, colony animals, and prisoners/slaves on the spot;
            // non-colony friendlies who are not downed are only tended if actively bleeding and not leaving the map.
            bool isColonyOwned = patient.Faction == doctor.Faction ||
                                 patient.HostFaction == doctor.Faction ||
                                 patient.IsPrisonerOfColony ||
                                 patient.IsSlaveOfColony;

            bool isNonColonyFriendly = !isColonyOwned &&
                                       patient.Faction != null &&
                                       !patient.Faction.HostileTo(doctor.Faction);

            if (!isColonyOwned && !isNonColonyFriendly)
            {
                return;
            }

            if (isNonColonyFriendly && !patient.Downed)
            {
                DutyDef dutyDef = patient.mindState?.duty?.def;
                if (dutyDef == DutyDefOf.ExitMapBest ||
                    dutyDef == DutyDefOf.ExitMapBestAndDefendSelf ||
                    dutyDef == DutyDefOf.TravelOrLeave)
                {
                    return;
                }

                if ((patient.health.hediffSet?.BleedRateTotal ?? 0f) <= 0f)
                {
                    return;
                }
            }

            if (patient.health.HasHediffsNeedingTend())
            {
                __result = true;
            }
        }
    }
}
