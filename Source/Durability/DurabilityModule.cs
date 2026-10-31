using SadSeal.Core.Modules;
using SadSeal.MegaMod.Durability.UI;
using UnityEngine;

namespace SadSeal.MegaMod.Durability
{
    public class DurabilityModule : ModModule<DurabilitySettings>
    {
        public static DurabilityModule Instance { get; private set; }

        public override string Id => "Durability";
        public override string DisplayNameKey => "MegaMod_TabDurability";
        public override string DescriptionKey => "MegaMod_ModuleDurability_Desc";
        public override int Order => 7;

        public static new DurabilitySettings ModSettings => Instance?.CurrentSettings ?? ModModule<DurabilitySettings>.ModSettings;
        public static new DurabilitySettings Settings => ModSettings;

        public DurabilityModule()
        {
            Instance = this;
        }

        public override void OnDefsLoaded()
        {
            if (IsEnabled)
            {
                DurabilityApplier.ApplyDurabilitySettings();
            }
        }

        public override void DrawSettings(Rect inRect)
        {
            DurabilitySettingsDrawer.DrawSettings(inRect, Settings);
        }
    }
}
