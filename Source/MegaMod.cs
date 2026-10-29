using System;
using System.Collections.Generic;
using HarmonyLib;
using SadSeal.MegaMod.AllowTool.UI;
using SadSeal.MegaMod.Combat.UI;
using SadSeal.MegaMod.Durability.UI;
using SadSeal.MegaMod.Genes.UI;
using SadSeal.MegaMod.Hauling.UI;
using SadSeal.MegaMod.Medical.UI;
using SadSeal.MegaMod.QoL.UI;
using SadSeal.MegaMod.Quarry.UI;
using SadSeal.MegaMod.Recipes.UI;
using SadSeal.MegaMod.Skills.UI;
using SadSeal.MegaMod.TimeFlow.UI;
using SadSeal.MegaMod.UI;
using SadSeal.MegaMod.WorkManager.UI;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod
{
    public enum MegaModTab
    {
        Modules,
        Combat,
        Skills,
        Recipes,
        Genes,
        Hauling,
        WorkManager,
        Durability,
        QoL,
        Medical,
        AllowTool,
        TimeFlow,
        Quarry
    }

    public class MegaMod : Mod
    {
        public static MegaMod Instance { get; private set; }
        public static MegaModSettings Settings { get; private set; }

        private static MegaModTab currentTab = MegaModTab.Modules;
        public static MegaModTab CurrentTab
        {
            get => currentTab;
            set => currentTab = value;
        }

        public MegaMod(ModContentPack content) : base(content)
        {
            Instance = this;
            Settings = GetSettings<MegaModSettings>();

            try
            {
                var harmony = new Harmony("sadseal.megamod");
                ApplyModulePatches(harmony);
            }
            catch (Exception ex)
            {
                Log.Error($"[SadSeal's Mega Mod] Error during initialization: {ex}");
            }
        }

        private void ApplyModulePatches(Harmony harmony)
        {
            int enabledCount = 0;
            int skippedCount = 0;

            foreach (Type type in typeof(MegaMod).Assembly.GetTypes())
            {
                if (!type.IsDefined(typeof(HarmonyPatch), inherit: true))
                {
                    continue;
                }

                if (IsPatchTypeEnabled(type))
                {
                    try
                    {
                        harmony.CreateClassProcessor(type).Patch();
                        enabledCount++;
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"[SadSeal's Mega Mod] Failed to apply patch class {type.FullName}: {ex}");
                    }
                }
                else
                {
                    skippedCount++;
                }
            }

            Log.Message($"[SadSeal's Mega Mod] Applied active modules: {enabledCount} patch classes enabled, {skippedCount} skipped.");
        }

        private bool IsPatchTypeEnabled(Type type)
        {
            var modules = Settings?.Modules;
            if (modules == null) return true;

            string ns = type.Namespace ?? string.Empty;

            if (ns.StartsWith("SadSeal.MegaMod.Combat")) return modules.enableCombat;
            if (ns.StartsWith("SadSeal.MegaMod.Skills")) return modules.enableSkills;
            if (ns.StartsWith("SadSeal.MegaMod.Recipes")) return modules.enableRecipes;
            if (ns.StartsWith("SadSeal.MegaMod.Genes")) return modules.enableGenes;
            if (ns.StartsWith("SadSeal.MegaMod.Hauling")) return modules.enableHauling;
            if (ns.StartsWith("SadSeal.MegaMod.WorkManager")) return modules.enableWorkManager;
            if (ns.StartsWith("SadSeal.MegaMod.Durability")) return modules.enableDurability;
            if (ns.StartsWith("SadSeal.MegaMod.QoL")) return modules.enableQoL;
            if (ns.StartsWith("SadSeal.MegaMod.Medical")) return modules.enableMedical;
            if (ns.StartsWith("SadSeal.MegaMod.AllowTool")) return modules.enableAllowTool;
            if (ns.StartsWith("SadSeal.MegaMod.TimeFlow")) return modules.enableTimeFlow;
            if (ns.StartsWith("SadSeal.MegaMod.Quarry")) return modules.enableQuarry;

            return true;
        }

        public override string SettingsCategory()
        {
            return "MegaMod_ModTitle".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            float navButtonWidth = 44f;
            float spacing = 8f;
            float topBarHeight = 36f;

            Rect prevRect = new Rect(inRect.x, inRect.y, navButtonWidth, topBarHeight);
            float centerWidth = inRect.width - (navButtonWidth * 2f) - (spacing * 2f);
            Rect centerRect = new Rect(prevRect.xMax + spacing, inRect.y, centerWidth, topBarHeight);
            Rect nextRect = new Rect(centerRect.xMax + spacing, inRect.y, navButtonWidth, topBarHeight);

            var allTabs = (MegaModTab[])Enum.GetValues(typeof(MegaModTab));
            int currentIndex = Array.IndexOf(allTabs, currentTab);

            // Left navigation button: ◄ (switches to previous tab)
            if (Widgets.ButtonText(prevRect, "◄"))
            {
                int prevIndex = (currentIndex - 1 + allTabs.Length) % allTabs.Length;
                currentTab = allTabs[prevIndex];
            }

            // Center dropdown button: [ 🔽 Modülü Seç: {Localized Tab Name} ]
            string selectModuleText = "MegaMod_SelectModule".Translate();
            string currentTabLabel = GetTabLabel(currentTab);
            string dropdownLabel = $"▼ {selectModuleText} {currentTabLabel}";

            if (Widgets.ButtonText(centerRect, dropdownLabel))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                foreach (MegaModTab tab in allTabs)
                {
                    MegaModTab targetTab = tab;
                    string optionLabel = GetTabLabel(targetTab);
                    if (targetTab == currentTab)
                    {
                        optionLabel = "● " + optionLabel;
                    }
                    options.Add(new FloatMenuOption(optionLabel, () => currentTab = targetTab));
                }
                Find.WindowStack.Add(new FloatMenu(options));
            }

            // Right navigation button: ► (switches to next tab)
            if (Widgets.ButtonText(nextRect, "►"))
            {
                int nextIndex = (currentIndex + 1) % allTabs.Length;
                currentTab = allTabs[nextIndex];
            }

            Widgets.DrawLineHorizontal(inRect.x, inRect.y + 44f, inRect.width);

            Rect contentRect = new Rect(inRect.x, inRect.y + 50f, inRect.width, inRect.height - 50f);

            switch (currentTab)
            {
                case MegaModTab.Modules:
                    ModuleSettingsDrawer.DrawSettings(contentRect, Settings.Modules);
                    break;
                case MegaModTab.Combat:
                    CombatSettingsDrawer.DrawSettings(contentRect, Settings.Combat);
                    break;
                case MegaModTab.Skills:
                    SkillSettingsDrawer.DrawSettings(contentRect, Settings.Skills);
                    break;
                case MegaModTab.Recipes:
                    RecipeSettingsDrawer.DrawSettings(contentRect, Settings.Recipes);
                    break;
                case MegaModTab.Genes:
                    GeneSynthesizerSettingsDrawer.DrawSettings(contentRect, Settings.Genes);
                    break;
                case MegaModTab.Hauling:
                    HaulSettingsDrawer.DrawSettings(contentRect, Settings.Hauling);
                    break;
                case MegaModTab.WorkManager:
                    WorkManagerSettingsDrawer.DrawSettings(contentRect, Settings.WorkManager);
                    break;
                case MegaModTab.Durability:
                    DurabilitySettingsDrawer.DrawSettings(contentRect, Settings.Durability);
                    break;
                case MegaModTab.QoL:
                    QoLSettingsDrawer.DrawSettings(contentRect, Settings.QoL);
                    break;
                case MegaModTab.Medical:
                    MedicalSettingsDrawer.DrawSettings(contentRect, Settings.Medical);
                    break;
                case MegaModTab.AllowTool:
                    AllowToolSettingsDrawer.DrawSettings(contentRect, Settings.AllowTool);
                    break;
                case MegaModTab.TimeFlow:
                    TimeFlowSettingsDrawer.DrawSettings(contentRect, Settings.TimeFlow);
                    break;
                case MegaModTab.Quarry:
                    QuarrySettingsDrawer.DrawSettings(contentRect, Settings.Quarry);
                    break;
            }

            base.DoSettingsWindowContents(inRect);
        }

        private static string GetTabLabel(MegaModTab tab)
        {
            string label = tab switch
            {
                MegaModTab.Modules => "MegaMod_TabModules".Translate(),
                MegaModTab.Combat => "MegaMod_TabCombat".Translate(),
                MegaModTab.Skills => "MegaMod_TabSkills".Translate(),
                MegaModTab.Recipes => "MegaMod_TabRecipes".Translate(),
                MegaModTab.Genes => "MegaMod_TabGenes".Translate(),
                MegaModTab.Hauling => "MegaMod_TabHauling".Translate(),
                MegaModTab.WorkManager => "MegaMod_TabWorkManager".Translate(),
                MegaModTab.Durability => "MegaMod_TabDurability".Translate(),
                MegaModTab.QoL => "MegaMod_TabQoL".Translate(),
                MegaModTab.Medical => "MegaMod_TabMedical".Translate(),
                MegaModTab.AllowTool => "MegaMod_TabAllowTool".Translate(),
                MegaModTab.TimeFlow => "MegaMod_TabTimeFlow".Translate(),
                MegaModTab.Quarry => "MegaMod_TabQuarry".Translate(),
                _ => tab.ToString()
            };

            if (tab != MegaModTab.Modules && Settings?.Modules != null && !Settings.Modules.IsModuleEnabled(tab))
            {
                label += " (" + "MegaMod_Disabled".Translate() + ")";
            }

            return label;
        }
    }
}
