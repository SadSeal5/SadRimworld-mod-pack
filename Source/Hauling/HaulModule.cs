using SadSeal.Core.Modules;
using SadSeal.MegaMod.Hauling.UI;
using UnityEngine;

namespace SadSeal.MegaMod.Hauling
{
    public class HaulModule : ModModule<HaulSettings>
    {
        public static HaulModule Instance { get; private set; }

        public override string Id => "Hauling";
        public override string DisplayNameKey => "MegaMod_TabHauling";
        public override string DescriptionKey => "MegaMod_ModuleHauling_Desc";
        public override int Order => 5;

        public static new HaulSettings ModSettings => Instance?.CurrentSettings ?? ModModule<HaulSettings>.ModSettings;
        public static new HaulSettings Settings => ModSettings;

        public HaulModule()
        {
            Instance = this;
        }

        public override void DrawSettings(Rect inRect)
        {
            HaulSettingsDrawer.DrawSettings(inRect, Settings);
        }
    }
}
