using System;
using HarmonyLib;
using SadSeal.Common;
using SadSeal.Core.Modules;
using SadSeal.MegaMod.Combat.UI;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Combat
{
    public class CombatModule : ModModule<CombatSettings>
    {
        public static CombatModule Instance { get; private set; }

        public override string Id => "Combat";
        public override string DisplayNameKey => "MegaMod_TabCombat";
        public override string DescriptionKey => "MegaMod_ModuleCombat_Desc";
        public override int Order => 1;

        public static new CombatSettings ModSettings => Instance?.CurrentSettings ?? ModModule<CombatSettings>.ModSettings;
        public static new CombatSettings Settings => ModSettings;

        public CombatModule()
        {
            Instance = this;
        }

        public override void Initialize(Harmony harmony)
        {
            if (ModBridge.IsCombatExtendedActive)
            {
                bool loggedCeNotice = false;
                foreach (Type type in GetType().Assembly.GetTypes())
                {
                    if (!type.IsDefined(typeof(HarmonyPatch), inherit: true))
                    {
                        continue;
                    }

                    if (type.Name == "Patch_ShotReport_HitFactor" || type.Name.StartsWith("Patch_Projectile"))
                    {
                        if (!loggedCeNotice)
                        {
                            SadLog.Message("Combat", "Combat Extended detected. Gracefully delegating ballistic/projectile patches to CE.");
                            loggedCeNotice = true;
                        }
                        continue;
                    }

                    try
                    {
                        harmony.CreateClassProcessor(type).Patch();
                    }
                    catch (Exception ex)
                    {
                        SadLog.Error("Combat", $"Failed to apply patch class {type.FullName}: {ex}");
                    }
                }
            }
            else
            {
                harmony.PatchAll(GetType().Assembly);
            }
        }

        public override void DrawSettings(Rect inRect)
        {
            CombatSettingsDrawer.DrawSettings(inRect, Settings);
        }
    }
}
