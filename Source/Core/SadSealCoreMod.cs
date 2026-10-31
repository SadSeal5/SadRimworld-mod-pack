using System;
using System.Collections.Generic;
using HarmonyLib;
using SadSeal.Common;
using SadSeal.Core.Registry;
using SadSeal.Core.Settings;
using SadSeal.Core.UI;
using UnityEngine;
using Verse;

namespace SadSeal.Core
{
    public class SadSealCoreMod : Mod
    {
        public static SadSealCoreMod Instance { get; private set; }
        public static CoreSettings Settings { get; private set; }

        private static int currentTabIndex = 0; // 0 = Modules, 1..N = modules

        public static int CurrentTabIndex
        {
            get => currentTabIndex;
            set => currentTabIndex = value;
        }

        public static void SelectModule(string moduleId)
        {
            var modules = ModuleManager.Modules;
            for (int i = 0; i < modules.Count; i++)
            {
                if (string.Equals(modules[i].Id, moduleId, StringComparison.OrdinalIgnoreCase))
                {
                    currentTabIndex = i + 1;
                    return;
                }
            }
        }

        public SadSealCoreMod(ModContentPack content) : base(content)
        {
            Instance = this;

            try
            {
                var harmony = new Harmony("sadseal.megamod");

                // Pre-deserialization module discovery (MUST be called before GetSettings)
                ModuleManager.DiscoverModules();

                // Load mod settings and deserialize modules
                Settings = GetSettings<CoreSettings>();

                // Patch core assembly
                harmony.PatchAll(typeof(SadSealCoreMod).Assembly);

                // Initialize all enabled modules
                ModuleManager.InitializeAll(harmony);
            }
            catch (Exception ex)
            {
                SadLog.Error("Core", $"Error during Core Mod initialization: {ex}");
            }
        }

        public override string SettingsCategory()
        {
            return "MegaMod_ModTitle".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var modules = ModuleManager.Modules;
            int totalTabs = 1 + modules.Count;

            if (currentTabIndex >= totalTabs)
            {
                currentTabIndex = 0;
            }

            float navButtonWidth = 44f;
            float spacing = 8f;
            float topBarHeight = 36f;

            Rect prevRect = new Rect(inRect.x, inRect.y, navButtonWidth, topBarHeight);
            float centerWidth = inRect.width - (navButtonWidth * 2f) - (spacing * 2f);
            Rect centerRect = new Rect(prevRect.xMax + spacing, inRect.y, centerWidth, topBarHeight);
            Rect nextRect = new Rect(centerRect.xMax + spacing, inRect.y, navButtonWidth, topBarHeight);

            // Left navigation button: ◄
            if (Widgets.ButtonText(prevRect, "◄"))
            {
                currentTabIndex = (currentTabIndex - 1 + totalTabs) % totalTabs;
            }

            // Center dropdown button
            string selectModuleText = "MegaMod_SelectModule".Translate();
            string currentTabLabel = GetTabLabel(currentTabIndex);
            string dropdownLabel = $"▼ {selectModuleText} {currentTabLabel}";

            if (Widgets.ButtonText(centerRect, dropdownLabel))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                for (int i = 0; i < totalTabs; i++)
                {
                    int index = i;
                    string optionLabel = GetTabLabel(index);
                    if (index == currentTabIndex)
                    {
                        optionLabel = "● " + optionLabel;
                    }
                    options.Add(new FloatMenuOption(optionLabel, () => currentTabIndex = index));
                }
                Find.WindowStack.Add(new FloatMenu(options));
            }

            // Right navigation button: ►
            if (Widgets.ButtonText(nextRect, "►"))
            {
                currentTabIndex = (currentTabIndex + 1) % totalTabs;
            }

            Widgets.DrawLineHorizontal(inRect.x, inRect.y + 44f, inRect.width);

            Rect contentRect = new Rect(inRect.x, inRect.y + 50f, inRect.width, inRect.height - 50f);

            if (currentTabIndex == 0)
            {
                CoreModuleSettingsDrawer.DrawSettings(contentRect);
            }
            else
            {
                int moduleIndex = currentTabIndex - 1;
                if (moduleIndex >= 0 && moduleIndex < modules.Count)
                {
                    modules[moduleIndex].DrawSettings(contentRect);
                }
            }

            base.DoSettingsWindowContents(inRect);
        }

        private static string GetTabLabel(int tabIndex)
        {
            if (tabIndex == 0)
            {
                return "MegaMod_TabModules".Translate();
            }

            int moduleIndex = tabIndex - 1;
            var modules = ModuleManager.Modules;
            if (moduleIndex >= 0 && moduleIndex < modules.Count)
            {
                var mod = modules[moduleIndex];
                string label = mod.DisplayNameKey.CanTranslate() ? mod.DisplayNameKey.Translate().Resolve() : mod.Id;
                if (!mod.IsEnabled)
                {
                    label += " (" + "MegaMod_Disabled".Translate() + ")";
                }
                return label;
            }

            return string.Empty;
        }
    }
}
