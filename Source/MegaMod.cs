using HarmonyLib;
using SadSeal.MegaMod.Combat.UI;
using SadSeal.MegaMod.Genes.UI;
using SadSeal.MegaMod.Hauling.UI;
using SadSeal.MegaMod.Recipes.UI;
using SadSeal.MegaMod.Skills.UI;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod
{
    public enum MegaModTab
    {
        Combat,
        Skills,
        Recipes,
        Genes,
        Hauling
    }

    public class MegaMod : Mod
    {
        public static MegaMod Instance { get; private set; }
        public static MegaModSettings Settings { get; private set; }

        private static MegaModTab currentTab = MegaModTab.Combat;

        public MegaMod(ModContentPack content) : base(content)
        {
            Instance = this;
            Settings = GetSettings<MegaModSettings>();

            var harmony = new Harmony("sadseal.megamod");
            harmony.PatchAll();

            Log.Message("[SadSeal's Mega Mod] Successfully initialized: Combat, Skills, Recipes, Genes & Smart Haul.");
        }

        public override string SettingsCategory()
        {
            return "MegaMod_ModTitle".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Rect tabRect = new Rect(inRect.x, inRect.y, inRect.width, 36f);
            float tabWidth = (tabRect.width - 32f) / 5f;

            DrawTabButton(new Rect(tabRect.x, tabRect.y, tabWidth, 36f), "MegaMod_TabCombat".Translate(), MegaModTab.Combat);
            DrawTabButton(new Rect(tabRect.x + tabWidth + 8f, tabRect.y, tabWidth, 36f), "MegaMod_TabSkills".Translate(), MegaModTab.Skills);
            DrawTabButton(new Rect(tabRect.x + (tabWidth + 8f) * 2f, tabRect.y, tabWidth, 36f), "MegaMod_TabRecipes".Translate(), MegaModTab.Recipes);
            DrawTabButton(new Rect(tabRect.x + (tabWidth + 8f) * 3f, tabRect.y, tabWidth, 36f), "MegaMod_TabGenes".Translate(), MegaModTab.Genes);
            DrawTabButton(new Rect(tabRect.x + (tabWidth + 8f) * 4f, tabRect.y, tabWidth, 36f), "MegaMod_TabHauling".Translate(), MegaModTab.Hauling);

            Widgets.DrawLineHorizontal(inRect.x, inRect.y + 42f, inRect.width);

            Rect contentRect = new Rect(inRect.x, inRect.y + 48f, inRect.width, inRect.height - 48f);

            switch (currentTab)
            {
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
            }

            base.DoSettingsWindowContents(inRect);
        }

        private void DrawTabButton(Rect rect, string label, MegaModTab tab)
        {
            Color prevColor = GUI.color;
            if (currentTab == tab)
            {
                GUI.color = new Color(0.35f, 0.85f, 1f, 1f);
            }
            if (Widgets.ButtonText(rect, label))
            {
                currentTab = tab;
            }
            GUI.color = prevColor;
        }
    }
}
