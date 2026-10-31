using SadSeal.Core.Modules;
using SadSeal.MegaMod.TimeFlow.UI;
using UnityEngine;

namespace SadSeal.MegaMod.TimeFlow
{
    public class TimeFlowModule : ModModule<TimeFlowSettings>
    {
        public static TimeFlowModule Instance { get; private set; }

        public override string Id => "TimeFlow";
        public override string ScribeTag => "timeFlow";
        public override string DisplayNameKey => "MegaMod_TabTimeFlow";
        public override string DescriptionKey => "MegaMod_ModuleTimeFlow_Desc";
        public override int Order => 11;

        public static new TimeFlowSettings ModSettings => Instance?.CurrentSettings ?? ModModule<TimeFlowSettings>.ModSettings;
        public static new TimeFlowSettings Settings => ModSettings;

        public TimeFlowModule()
        {
            Instance = this;
        }

        public override void DrawSettings(Rect inRect)
        {
            TimeFlowSettingsDrawer.DrawSettings(inRect, Settings);
        }
    }
}
