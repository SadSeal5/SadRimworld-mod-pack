using SadSeal.Core.Modules;
using SadSeal.MegaMod.AutoArm.UI;
using UnityEngine;

namespace SadSeal.MegaMod.AutoArm
{
    public class AutoArmModule : ModModule<AutoArmSettings>
    {
        public static AutoArmModule Instance { get; private set; }

        public override string Id => "AutoArm";
        public override string ScribeTag => "autoArm";
        public override string DisplayNameKey => "MegaMod_TabAutoArm";
        public override string DescriptionKey => "MegaMod_ModuleAutoArm_Desc";
        public override int Order => 13;

        public static new AutoArmSettings ModSettings => Instance?.CurrentSettings ?? ModModule<AutoArmSettings>.ModSettings;
        public static new AutoArmSettings Settings => ModSettings;

        public AutoArmModule()
        {
            Instance = this;
        }

        public override void DrawSettings(Rect inRect)
        {
            AutoArmSettingsDrawer.DrawSettings(inRect, Settings);
        }
    }
}
