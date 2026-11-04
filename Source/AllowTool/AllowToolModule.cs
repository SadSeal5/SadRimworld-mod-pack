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
                DisableAll();
                return;
            }

            var settings = Settings;
            if (settings == null) return;

            if (!settings.EnableHaulUrgently)
            {
                DefUtility.DisableWorkGiver("SadHaul_WorkGiver_HaulUrgently");
                DefUtility.RemoveDesignators("SadSeal.MegaMod.AllowTool.HaulUrgently.Designator_HaulUrgently");
            }

            if (!settings.EnableFinishOff)
            {
                DefUtility.DisableWorkGiver("Sad_WorkGiver_FinishOff");
                DefUtility.RemoveDesignators("SadSeal.MegaMod.AllowTool.FinishOff.Designator_FinishOff");
            }

            if (!settings.EnableUnforbidAll)
            {
                DefUtility.RemoveDesignators(
                    "SadSeal.MegaMod.AllowTool.Unforbid.Designator_UnforbidAll",
                    "SadSeal.MegaMod.AllowTool.Unforbid.Designator_Unforbid");
            }

            if (!settings.EnableSelectSimilar)
            {
                DefUtility.RemoveDesignators("SadSeal.MegaMod.AllowTool.SelectSimilar.Designator_SelectSimilar");
            }
        }

        private static void DisableAll()
        {
            DefUtility.DisableWorkGiver("SadHaul_WorkGiver_HaulUrgently");
            DefUtility.DisableWorkGiver("Sad_WorkGiver_FinishOff");
            DefUtility.RemoveDesignators(
                "SadSeal.MegaMod.AllowTool.Unforbid.Designator_UnforbidAll",
                "SadSeal.MegaMod.AllowTool.Unforbid.Designator_Unforbid",
                "SadSeal.MegaMod.AllowTool.HaulUrgently.Designator_HaulUrgently",
                "SadSeal.MegaMod.AllowTool.SelectSimilar.Designator_SelectSimilar",
                "SadSeal.MegaMod.AllowTool.FinishOff.Designator_FinishOff",
                "SadSeal.MegaMod.AllowTool.Area.Designator_StripArea",
                "SadSeal.MegaMod.AllowTool.Area.Designator_CancelArea");
        }

        public override void DrawSettings(Rect inRect)
        {
            AllowToolSettingsDrawer.DrawSettings(inRect, Settings);
        }
    }
}
