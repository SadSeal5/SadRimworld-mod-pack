using RimWorld;
using SadSeal.Core.Modules;
using SadSeal.MegaMod.Genes.Patches;
using SadSeal.MegaMod.Genes.UI;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Genes
{
    public class GenesModule : ModModule<GeneSynthesizerSettings>
    {
        public static GenesModule Instance { get; private set; }

        public override string Id => "Genes";
        public override string DisplayNameKey => "MegaMod_TabGenes";
        public override string DescriptionKey => "MegaMod_ModuleGenes_Desc";
        public override int Order => 4;

        public static new GeneSynthesizerSettings ModSettings => Instance?.CurrentSettings ?? ModModule<GeneSynthesizerSettings>.ModSettings;
        public static new GeneSynthesizerSettings Settings => ModSettings;

        public GenesModule()
        {
            Instance = this;
        }

        public override void OnDefsLoaded()
        {
            if (IsEnabled)
            {
                if (ModLister.BiotechInstalled && ThingDefOf.GeneAssembler != null)
                {
                    GeneAssemblerPatch.EnsureDefHasComp(ThingDefOf.GeneAssembler);
                }
            }
        }

        public override void DrawSettings(Rect inRect)
        {
            GeneSynthesizerSettingsDrawer.DrawSettings(inRect, Settings);
        }
    }
}
