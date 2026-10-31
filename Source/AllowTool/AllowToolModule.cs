using SadSeal.Common;
using SadSeal.Core.Modules;
using SadSeal.MegaMod.AllowTool.UI;
using UnityEngine;

namespace SadSeal.MegaMod.AllowTool
{
    public class AllowToolModule : ModModule<AllowToolSettings>
    {
        public static AllowToolModule Instance { get; private set; }

        public override string Id => "AllowTool";
        public override string ScribeTag => "allowTool";
        public override string DisplayNameKey => "MegaMod_TabAllowTool";
        public override string DescriptionKey => "MegaMod_ModuleAllowTool_Desc";
        public override int Order => 10;

        public static new AllowToolSettings ModSettings => Instance?.CurrentSettings ?? ModModule<AllowToolSettings>.ModSettings;
        public static new AllowToolSettings Settings => ModSettings;

        public AllowToolModule()
        {
            Instance = this;
        }

        public override void OnDefsLoaded()
        {
            if (!IsEnabled)
            {
                DefUtility.DisableWorkGiver("HaulUrgently");
                DefUtility.DisableWorkGiver("FinishOff");
            }
        }

        public override void DrawSettings(Rect inRect)
        {
            AllowToolSettingsDrawer.DrawSettings(inRect, Settings);
        }
    }
}
