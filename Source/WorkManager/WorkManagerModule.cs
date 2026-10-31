using SadSeal.Core.Modules;
using SadSeal.MegaMod.WorkManager.UI;
using UnityEngine;

namespace SadSeal.MegaMod.WorkManager
{
    public class WorkManagerModule : ModModule<WorkManagerSettings>
    {
        public static WorkManagerModule Instance { get; private set; }

        public override string Id => "WorkManager";
        public override string ScribeTag => "workManager";
        public override string DisplayNameKey => "MegaMod_TabWorkManager";
        public override string DescriptionKey => "MegaMod_ModuleWorkManager_Desc";
        public override int Order => 6;

        public static new WorkManagerSettings ModSettings => Instance?.CurrentSettings ?? ModModule<WorkManagerSettings>.ModSettings;
        public static new WorkManagerSettings Settings => ModSettings;

        public WorkManagerModule()
        {
            Instance = this;
        }

        public override void DrawSettings(Rect inRect)
        {
            WorkManagerSettingsDrawer.DrawSettings(inRect, Settings);
        }
    }
}
