using UnityEngine;
using Verse;
using SadSeal.Common.UI;

namespace SadSeal.MegaMod.Medical.UI
{
    public static class MedicalSettingsDrawer
    {
        private static Vector2 scrollPosition = Vector2.zero;

        public static void DrawSettings(Rect inRect, MedicalSettings settings)
        {
            if (settings == null) return;

            Rect viewRect = new Rect(0f, 0f, inRect.width - 24f, 850f);
            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            SadUI.DrawHeader(listing, "MegaMod_Medical_Title", "MegaMod_Medical_Desc");

            // Section 1: Autopsy Suite
            listing.Label("MegaMod_Medical_AutopsySection".Translate());
            listing.CheckboxLabeled("MegaMod_Medical_EnableAutopsy".Translate(), ref settings.EnableAutopsy, "MegaMod_Medical_EnableAutopsy_Desc".Translate());

            if (settings.EnableAutopsy)
            {
                settings.AutopsySuccessBaseChance = listing.SliderLabeled(
                    "MegaMod_Medical_AutopsyBaseChance".Translate() + $": {settings.AutopsySuccessBaseChance:P0}",
                    settings.AutopsySuccessBaseChance, 0.10f, 1.0f, 0.5f,
                    "MegaMod_Medical_AutopsyBaseChance_Desc".Translate());

                settings.AutopsyBionicExtractChance = listing.SliderLabeled(
                    "MegaMod_Medical_AutopsyBionicChance".Translate() + $": {settings.AutopsyBionicExtractChance:P0}",
                    settings.AutopsyBionicExtractChance, 0.10f, 1.0f, 0.5f,
                    "MegaMod_Medical_AutopsyBionicChance_Desc".Translate());

                listing.CheckboxLabeled("MegaMod_Medical_ExtractMechParts".Translate(), ref settings.ExtractMechanoidParts, "MegaMod_Medical_ExtractMechParts_Desc".Translate());
            }

            listing.Gap(12f);

            // Section 2: Mood Penalty Softener
            listing.Label("MegaMod_Medical_MoodSection".Translate());
            listing.CheckboxLabeled("MegaMod_Medical_SoftenMood".Translate(), ref settings.SoftenHarvestMoodPenalty, "MegaMod_Medical_SoftenMood_Desc".Translate());

            if (settings.SoftenHarvestMoodPenalty)
            {
                settings.HarvestMoodPenaltyMultiplier = listing.SliderLabeled(
                    "MegaMod_Medical_MoodMultiplier".Translate() + $": {settings.HarvestMoodPenaltyMultiplier:P0}",
                    settings.HarvestMoodPenaltyMultiplier, 0.0f, 1.0f, 0.5f,
                    "MegaMod_Medical_MoodMultiplier_Desc".Translate());
            }

            listing.Gap(12f);

            // Section 3: Field Medicine
            listing.Label("MegaMod_Medical_FieldSection".Translate());
            listing.CheckboxLabeled("MegaMod_Medical_EnableField".Translate(), ref settings.EnableFieldMedicine, "MegaMod_Medical_EnableField_Desc".Translate());

            if (settings.EnableFieldMedicine)
            {
                listing.CheckboxLabeled("MegaMod_Medical_FieldDocInv".Translate(), ref settings.FieldMedicineUseDoctorInventory, "MegaMod_Medical_FieldDocInv_Desc".Translate());
                listing.CheckboxLabeled("MegaMod_Medical_FieldPatInv".Translate(), ref settings.FieldMedicineUsePatientInventory, "MegaMod_Medical_FieldPatInv_Desc".Translate());
            }

            listing.Gap(12f);

            // Section 4: Smart Medicine
            listing.Label("MegaMod_Medical_SmartSection".Translate());
            listing.CheckboxLabeled("MegaMod_Medical_EnableSmart".Translate(), ref settings.EnableSmartMedicine, "MegaMod_Medical_EnableSmart_Desc".Translate());

            if (settings.EnableSmartMedicine)
            {
                listing.CheckboxLabeled("MegaMod_Medical_NoGlitterScratches".Translate(), ref settings.NoGlitterOnScratches, "MegaMod_Medical_NoGlitterScratches_Desc".Translate());
                listing.CheckboxLabeled("MegaMod_Medical_SaveBestLethal".Translate(), ref settings.SaveBestMedicineForLethal, "MegaMod_Medical_SaveBestLethal_Desc".Translate());

                settings.LethalBleedRateThreshold = listing.SliderLabeled(
                    "MegaMod_Medical_BleedThreshold".Translate() + $": {settings.LethalBleedRateThreshold:P0}/day",
                    settings.LethalBleedRateThreshold, 0.05f, 0.50f, 0.5f,
                    "MegaMod_Medical_BleedThreshold_Desc".Translate());
            }

            listing.End();
            Widgets.EndScrollView();
        }
    }
}
