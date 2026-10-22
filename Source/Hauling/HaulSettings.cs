using Verse;

namespace SadSeal.MegaMod.Hauling
{
    public enum HaulPreset
    {
        Custom,
        Balanced,
        Performance,
        MaximumThroughput
    }

    public class HaulSettings : IExposable
    {
        public bool EnableSmartHaul = true;
        public int SearchRadius = 12;
        public int MaxItemsPerTrip = 8;
        public float MassLimitPercent = 0.95f;
        public bool DropOnDraft = true;
        public bool AnimalsCanSmartHaul = false;
        public bool StrictClustering = true;

        public HaulPreset ActivePreset = HaulPreset.Balanced;

        public void ExposeData()
        {
            Scribe_Values.Look(ref EnableSmartHaul, "enableSmartHaul", true);
            Scribe_Values.Look(ref SearchRadius, "searchRadius", 12);
            Scribe_Values.Look(ref MaxItemsPerTrip, "maxItemsPerTrip", 8);
            Scribe_Values.Look(ref MassLimitPercent, "massLimitPercent", 0.95f);
            Scribe_Values.Look(ref DropOnDraft, "dropOnDraft", true);
            Scribe_Values.Look(ref AnimalsCanSmartHaul, "animalsCanSmartHaul", false);
            Scribe_Values.Look(ref StrictClustering, "strictClustering", true);
            Scribe_Values.Look(ref ActivePreset, "activePreset", HaulPreset.Balanced);
        }

        public void ResetToDefaults()
        {
            ApplyPreset(HaulPreset.Balanced);
        }

        public void ApplyPreset(HaulPreset preset)
        {
            ActivePreset = preset;
            EnableSmartHaul = true;
            DropOnDraft = true;

            switch (preset)
            {
                case HaulPreset.Balanced:
                    SearchRadius = 12;
                    MaxItemsPerTrip = 8;
                    MassLimitPercent = 0.95f;
                    StrictClustering = true;
                    AnimalsCanSmartHaul = false;
                    break;

                case HaulPreset.Performance:
                    SearchRadius = 8;
                    MaxItemsPerTrip = 4;
                    MassLimitPercent = 0.80f;
                    StrictClustering = true;
                    AnimalsCanSmartHaul = false;
                    break;

                case HaulPreset.MaximumThroughput:
                    SearchRadius = 20;
                    MaxItemsPerTrip = 16;
                    MassLimitPercent = 1.00f;
                    StrictClustering = false;
                    AnimalsCanSmartHaul = true;
                    break;
            }
        }
    }
}
