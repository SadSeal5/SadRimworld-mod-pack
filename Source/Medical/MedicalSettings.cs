using Verse;

namespace SadSeal.MegaMod.Medical
{
    public class MedicalSettings : IExposable
    {
        public bool EnableAutopsy = true;
        public float AutopsySuccessBaseChance = 0.65f;
        public float AutopsySkillFactor = 0.025f;
        public float AutopsyBionicExtractChance = 0.85f;
        public bool AutopsyRequiresMedicine = false;
        public bool ExtractMechanoidParts = true;

        public bool SoftenHarvestMoodPenalty = true;
        public float HarvestMoodPenaltyMultiplier = 0.20f;

        public bool EnableFieldMedicine = true;
        public bool FieldMedicineUseDoctorInventory = true;
        public bool FieldMedicineUsePatientInventory = true;

        public bool EnableSmartMedicine = true;
        public bool NoGlitterOnScratches = true;
        public bool SaveBestMedicineForLethal = true;
        public float LethalBleedRateThreshold = 0.20f;

        public void ExposeData()
        {
            Scribe_Values.Look(ref EnableAutopsy, "EnableAutopsy", true);
            Scribe_Values.Look(ref AutopsySuccessBaseChance, "AutopsySuccessBaseChance", 0.65f);
            Scribe_Values.Look(ref AutopsySkillFactor, "AutopsySkillFactor", 0.025f);
            Scribe_Values.Look(ref AutopsyBionicExtractChance, "AutopsyBionicExtractChance", 0.85f);
            Scribe_Values.Look(ref AutopsyRequiresMedicine, "AutopsyRequiresMedicine", false);
            Scribe_Values.Look(ref ExtractMechanoidParts, "ExtractMechanoidParts", true);

            Scribe_Values.Look(ref SoftenHarvestMoodPenalty, "SoftenHarvestMoodPenalty", true);
            Scribe_Values.Look(ref HarvestMoodPenaltyMultiplier, "HarvestMoodPenaltyMultiplier", 0.20f);

            Scribe_Values.Look(ref EnableFieldMedicine, "EnableFieldMedicine", true);
            Scribe_Values.Look(ref FieldMedicineUseDoctorInventory, "FieldMedicineUseDoctorInventory", true);
            Scribe_Values.Look(ref FieldMedicineUsePatientInventory, "FieldMedicineUsePatientInventory", true);

            Scribe_Values.Look(ref EnableSmartMedicine, "EnableSmartMedicine", true);
            Scribe_Values.Look(ref NoGlitterOnScratches, "NoGlitterOnScratches", true);
            Scribe_Values.Look(ref SaveBestMedicineForLethal, "SaveBestMedicineForLethal", true);
            Scribe_Values.Look(ref LethalBleedRateThreshold, "LethalBleedRateThreshold", 0.20f);
        }
    }
}
