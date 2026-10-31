using SadSeal.Common;
using SadSeal.Core.Modules;
using SadSeal.MegaMod.Medical.UI;
using UnityEngine;

namespace SadSeal.MegaMod.Medical
{
    public class MedicalModule : ModModule<MedicalSettings>
    {
        public static MedicalModule Instance { get; private set; }

        public override string Id => "Medical";
        public override string DisplayNameKey => "MegaMod_TabMedical";
        public override string DescriptionKey => "MegaMod_ModuleMedical_Desc";
        public override int Order => 9;

        public static new MedicalSettings ModSettings => Instance?.CurrentSettings ?? ModModule<MedicalSettings>.ModSettings;
        public static new MedicalSettings Settings => ModSettings;

        public MedicalModule()
        {
            Instance = this;
        }

        public override void OnDefsLoaded()
        {
            if (!IsEnabled)
            {
                DefUtility.HideThingDef("SadMedical_AutopsyTable");
            }
        }

        public override void DrawSettings(Rect inRect)
        {
            MedicalSettingsDrawer.DrawSettings(inRect, Settings);
        }
    }
}
