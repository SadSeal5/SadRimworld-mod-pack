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

            var settings = MegaMod.Settings?.Medical;
            if (settings == null || !settings.EnableFieldMedicine) return;

            if (patient != null && !patient.Dead && patient.health != null && patient.health.HasHediffsNeedingTend())
            {
                // Allow tending in the field if patient is wounded and needs immediate medical attention
                __result = true;
            }
        }
    }
}
