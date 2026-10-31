using SadSeal.Common.UI;
using SadSeal.Core.Registry;
using UnityEngine;
using Verse;

namespace SadSeal.Core.UI
{
    public static class CoreModuleSettingsDrawer
    {
        private static Vector2 scrollPosition = Vector2.zero;

        public static void DrawSettings(Rect inRect)
        {
            var modules = ModuleManager.Modules;
            float totalHeight = 180f + (modules.Count * 36f);
            Rect viewRect = new Rect(0f, 0f, inRect.width - 24f, Mathf.Max(inRect.height, totalHeight));
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

            // Dynamic checkboxes for discovered modules
            foreach (var module in modules)
            {
                bool enabled = module.IsEnabled;
                string label = module.DisplayNameKey.CanTranslate() ? module.DisplayNameKey.Translate().Resolve() : module.Id;
                string tooltip = module.DescriptionKey.CanTranslate() ? module.DescriptionKey.Translate().Resolve() : null;

                listing.CheckboxLabeled(label, ref enabled, tooltip);
                if (enabled != module.IsEnabled)
                {
                    module.IsEnabled = enabled;
                }
                listing.Gap(6f);
            }

            listing.Gap(16f);

            // Enable All and Disable All buttons
            Rect btnRow = listing.GetRect(32f);
            float spacing = 12f;
            float btnWidth = (btnRow.width - spacing) / 2f;
            Rect enableAllRect = new Rect(btnRow.x, btnRow.y, btnWidth, 32f);
            Rect disableAllRect = new Rect(btnRow.x + btnWidth + spacing, btnRow.y, btnWidth, 32f);

            if (Widgets.ButtonText(enableAllRect, "MegaMod_EnableAll".Translate()))
            {
                foreach (var module in modules)
                {
                    module.IsEnabled = true;
                }
            }

            if (Widgets.ButtonText(disableAllRect, "MegaMod_DisableAll".Translate()))
            {
                foreach (var module in modules)
                {
                    module.IsEnabled = false;
                }
            }

            listing.End();
            Widgets.EndScrollView();
        }
    }
}
