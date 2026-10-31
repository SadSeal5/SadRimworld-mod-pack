using SadSeal.Common;
using SadSeal.Core.Modules;
using SadSeal.MegaMod.Quarry.UI;
using UnityEngine;

namespace SadSeal.MegaMod.Quarry
{
    public class QuarryModule : ModModule<QuarrySettings>
    {
        public static QuarryModule Instance { get; private set; }

        public override string Id => "Quarry";
        public override string DisplayNameKey => "MegaMod_TabQuarry";
        public override string DescriptionKey => "MegaMod_ModuleQuarry_Desc";
        public override int Order => 12;

        public static new QuarrySettings ModSettings => Instance?.CurrentSettings ?? ModModule<QuarrySettings>.ModSettings;
        public static new QuarrySettings Settings => ModSettings;

        public QuarryModule()
        {
            Instance = this;
        }

        public override void OnDefsLoaded()
        {
            if (!IsEnabled)
            {
                DefUtility.HideThingDef("Sad_DeepQuarry");
            }
        }

        public override void DrawSettings(Rect inRect)
        {
            QuarrySettingsDrawer.DrawSettings(inRect, Settings);
        }
    }
}
