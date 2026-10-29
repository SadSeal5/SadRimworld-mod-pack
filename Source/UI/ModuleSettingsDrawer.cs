using UnityEngine;
using Verse;
using SadSeal.Common.UI;

namespace SadSeal.MegaMod.UI
{
    public static class ModuleSettingsDrawer
    {
        private static Vector2 scrollPosition = Vector2.zero;

        public static void DrawSettings(Rect inRect, ModuleSettings settings)
        {
            if (settings == null) return;

            Rect viewRect = new Rect(0f, 0f, inRect.width - 24f, 680f);
            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            SadUI.DrawHeader(listing, "MegaMod_TabModules", "MegaMod_ModulesDesc");

            // Prominent restart notice banner
            Rect bannerRect = listing.GetRect(44f);
            Widgets.DrawBoxSolid(bannerRect, new Color(0.2f, 0.18f, 0.08f, 0.7f));
            Widgets.DrawHighlightIfMouseover(bannerRect);
            Rect textRect = bannerRect.ContractedBy(6f);
            Text.Anchor = TextAnchor.MiddleCenter;
            GUI.color = new Color(1f, 0.86f, 0.35f);
            Widgets.Label(textRect, "MegaMod_RestartNotice".Translate());
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;

            listing.Gap(14f);

            // 12 Module Checkboxes with clear labels and descriptions
            listing.CheckboxLabeled("MegaMod_ModuleCombat".Translate(), ref settings.enableCombat, "MegaMod_ModuleCombat_Desc".Translate());
            listing.Gap(6f);
            listing.CheckboxLabeled("MegaMod_ModuleSkills".Translate(), ref settings.enableSkills, "MegaMod_ModuleSkills_Desc".Translate());
            listing.Gap(6f);
            listing.CheckboxLabeled("MegaMod_ModuleRecipes".Translate(), ref settings.enableRecipes, "MegaMod_ModuleRecipes_Desc".Translate());
            listing.Gap(6f);
            listing.CheckboxLabeled("MegaMod_ModuleGenes".Translate(), ref settings.enableGenes, "MegaMod_ModuleGenes_Desc".Translate());
            listing.Gap(6f);
            listing.CheckboxLabeled("MegaMod_ModuleHauling".Translate(), ref settings.enableHauling, "MegaMod_ModuleHauling_Desc".Translate());
            listing.Gap(6f);
            listing.CheckboxLabeled("MegaMod_ModuleWorkManager".Translate(), ref settings.enableWorkManager, "MegaMod_ModuleWorkManager_Desc".Translate());
            listing.Gap(6f);
            listing.CheckboxLabeled("MegaMod_ModuleDurability".Translate(), ref settings.enableDurability, "MegaMod_ModuleDurability_Desc".Translate());
            listing.Gap(6f);
            listing.CheckboxLabeled("MegaMod_ModuleQoL".Translate(), ref settings.enableQoL, "MegaMod_ModuleQoL_Desc".Translate());
            listing.Gap(6f);
            listing.CheckboxLabeled("MegaMod_ModuleMedical".Translate(), ref settings.enableMedical, "MegaMod_ModuleMedical_Desc".Translate());
            listing.Gap(6f);
            listing.CheckboxLabeled("MegaMod_ModuleAllowTool".Translate(), ref settings.enableAllowTool, "MegaMod_ModuleAllowTool_Desc".Translate());
            listing.Gap(6f);
            listing.CheckboxLabeled("MegaMod_ModuleTimeFlow".Translate(), ref settings.enableTimeFlow, "MegaMod_ModuleTimeFlow_Desc".Translate());
            listing.Gap(6f);
            listing.CheckboxLabeled("MegaMod_ModuleQuarry".Translate(), ref settings.enableQuarry, "MegaMod_ModuleQuarry_Desc".Translate());

            listing.Gap(16f);

            // "Enable All" and "Disable All" buttons at the bottom
            Rect btnRow = listing.GetRect(32f);
            float spacing = 12f;
            float btnWidth = (btnRow.width - spacing) / 2f;
            Rect enableAllRect = new Rect(btnRow.x, btnRow.y, btnWidth, 32f);
            Rect disableAllRect = new Rect(btnRow.x + btnWidth + spacing, btnRow.y, btnWidth, 32f);

            if (Widgets.ButtonText(enableAllRect, "MegaMod_EnableAll".Translate()))
            {
                settings.SetAll(true);
            }

            if (Widgets.ButtonText(disableAllRect, "MegaMod_DisableAll".Translate()))
            {
                settings.SetAll(false);
            }

            listing.End();
            Widgets.EndScrollView();
        }
    }
}
