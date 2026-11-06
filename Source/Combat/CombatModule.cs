using System;
using System.Reflection;
using HarmonyLib;
using SadSeal.Common;
using SadSeal.Core.Modules;
using SadSeal.MegaMod.Combat.Patches;
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
                PatchProjectileSubclasses(harmony);
            }
        }

        private void PatchProjectileSubclasses(Harmony harmony)
        {
            var damagePostfix = new HarmonyMethod(typeof(Patch_Projectile_Subclasses), nameof(Patch_Projectile_Subclasses.SubclassDamagePostfix));
            var apPostfix = new HarmonyMethod(typeof(Patch_Projectile_Subclasses), nameof(Patch_Projectile_Subclasses.SubclassApPostfix));
            var ticksPostfix = new HarmonyMethod(typeof(Patch_Projectile_Subclasses), nameof(Patch_Projectile_Subclasses.SubclassStartingTicksPostfix));

            foreach (Type type in GenTypes.AllSubclasses(typeof(Verse.Projectile)))
            {
                try
                {
                    var dmgMethod = type.GetMethod("get_DamageAmount", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                    if (dmgMethod != null)
                    {
                        harmony.Patch(dmgMethod, postfix: damagePostfix);
                    }

                    var apMethod = type.GetMethod("get_ArmorPenetration", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                    if (apMethod != null)
                    {
                        harmony.Patch(apMethod, postfix: apPostfix);
                    }

                    var ticksMethod = type.GetMethod("get_StartingTicksToImpact", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
                    if (ticksMethod != null)
                    {
                        harmony.Patch(ticksMethod, postfix: ticksPostfix);
                    }
                }
                catch (Exception ex)
                {
                    SadLog.Warning("Combat", $"Could not dynamically patch projectile subclass {type.FullName}: {ex.Message}");
                }
            }
        }

        public override void DrawSettings(Rect inRect)
        {
            CombatSettingsDrawer.DrawSettings(inRect, Settings);
        }
    }
}
