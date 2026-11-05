using SadSeal.Core.Modules;
using SadSeal.MegaMod.Incubation.UI;
using UnityEngine;

namespace SadSeal.MegaMod.Incubation
{
    public class IncubationModule : ModModule<IncubationSettings>
    {
        public static IncubationModule Instance { get; private set; }

        public override string Id => "Incubation";
        public override string ScribeTag => "incubation";
        public override string DisplayNameKey => "MegaMod_TabIncubation";
        public override string DescriptionKey => "MegaMod_ModuleIncubation_Desc";
        public override int Order => 14;

        public static new IncubationSettings ModSettings => Instance?.CurrentSettings ?? ModModule<IncubationSettings>.ModSettings;
        public static new IncubationSettings Settings => ModSettings;

        public IncubationModule()
        {
            Instance = this;
        }

        public override void DrawSettings(Rect inRect)
        {
            IncubationSettingsDrawer.DrawSettings(inRect, Settings);
        }
    }
}
