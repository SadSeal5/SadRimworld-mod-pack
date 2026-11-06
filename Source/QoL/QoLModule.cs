using SadSeal.Core.Modules;
using SadSeal.MegaMod.QoL.Patches.Pawn;
using SadSeal.MegaMod.QoL.Patches.Production;
using SadSeal.MegaMod.QoL.Patches.Safety;
using SadSeal.MegaMod.QoL.UI;
using UnityEngine;

namespace SadSeal.MegaMod.QoL
{
    public class QoLModule : ModModule<QoLSettings>
    {
        public static QoLModule Instance { get; private set; }

        public override string Id => "QoL";
        public override string DisplayNameKey => "MegaMod_TabQoL";
        public override string DescriptionKey => "MegaMod_ModuleQoL_Desc";
        public override int Order => 8;

        public static new QoLSettings ModSettings => Instance?.CurrentSettings ?? ModModule<QoLSettings>.ModSettings;
        public static new QoLSettings Settings => ModSettings;

        public QoLModule()
        {
            Instance = this;
        }

        public override void OnDefsLoaded()
        {
            if (IsEnabled)
            {
                QoLApplier.ApplyAll();
            }
        }

        public override void DrawSettings(Rect inRect)
        {
            QoLSettingsDrawer.DrawSettings(inRect, Settings);
        }

        public override void OnGameEnded()
        {
            SnapOutUtility.ClearCache();
            RoofSafetyUtility.ClearCache();
            WhereIsMyWeaponGameComponent.ResetInstance();
            BillClipboard.ClearClipboard();
            Patch_ReplaceStuff.ResetState();
        }
    }
}
