using Verse;

namespace SadSeal.MegaMod.Genes
{
    public enum GeneSynthesizerPreset
    {
        Custom,
        Free,
        Cheap,
        Standard,
        Hardcore
    }

    public class GeneSynthesizerSettings : IExposable
    {
        public int NeutroamineCost = 100;
        public float WorkAmount = 2500f;
        public bool InstantCraft = false;
        public bool RequireArchiteCapsules = false;
        public bool AllowArchiteGenes = true;
        public GeneSynthesizerPreset ActivePreset = GeneSynthesizerPreset.Standard;

        public void ApplyPreset(GeneSynthesizerPreset preset)
        {
            ActivePreset = preset;
            switch (preset)
            {
                case GeneSynthesizerPreset.Free:
                    NeutroamineCost = 0;
                    WorkAmount = 0f;
                    InstantCraft = true;
                    break;
                case GeneSynthesizerPreset.Cheap:
                    NeutroamineCost = 25;
                    WorkAmount = 1000f;
                    InstantCraft = false;
                    break;
                case GeneSynthesizerPreset.Standard:
                    NeutroamineCost = 100;
                    WorkAmount = 2500f;
                    InstantCraft = false;
                    break;
                case GeneSynthesizerPreset.Hardcore:
                    NeutroamineCost = 250;
                    WorkAmount = 5000f;
                    InstantCraft = false;
                    break;
            }
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref NeutroamineCost, "NeutroamineCost", 100);
            Scribe_Values.Look(ref WorkAmount, "WorkAmount", 2500f);
            Scribe_Values.Look(ref InstantCraft, "InstantCraft", false);
            Scribe_Values.Look(ref RequireArchiteCapsules, "RequireArchiteCapsules", false);
            Scribe_Values.Look(ref AllowArchiteGenes, "AllowArchiteGenes", true);
            Scribe_Values.Look(ref ActivePreset, "ActivePreset", GeneSynthesizerPreset.Standard);
        }

        public void ResetToDefaults()
        {
            NeutroamineCost = 100;
            WorkAmount = 2500f;
            InstantCraft = false;
            RequireArchiteCapsules = false;
            AllowArchiteGenes = true;
            ActivePreset = GeneSynthesizerPreset.Standard;
        }
    }
}
