using UnityEngine;
using Verse;
using SadSeal.Common.UI;

namespace SadSeal.MegaMod.AutoArm.UI
{
    public static class AutoArmSettingsDrawer
    {
        private static Vector2 scrollPosition = Vector2.zero;

        public static void DrawSettings(Rect inRect, AutoArmSettings settings)
        {
            if (settings == null) return;

            Rect viewRect = new Rect(0f, 0f, inRect.width - 24f, 840f);
            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            SadUI.DrawHeader(listing, "MegaMod_AutoArm_Title", "MegaMod_AutoArm_Desc");

            listing.CheckboxLabeled("MegaMod_AutoArm_Enable".Translate(), ref settings.enableAutoArm, "MegaMod_AutoArm_Enable_Desc".Translate());
            listing.Gap(8f);

            if (settings.enableAutoArm)
            {
                listing.CheckboxLabeled("MegaMod_AutoArm_OnlyStockpile".Translate(), ref settings.onlyFromStockpile, "MegaMod_AutoArm_OnlyStockpile_Desc".Translate());
                listing.Gap(6f);

                listing.CheckboxLabeled("MegaMod_AutoArm_AutoSkill".Translate(), ref settings.autoAssignBySkill, "MegaMod_AutoArm_AutoSkill_Desc".Translate());
                listing.Gap(6f);

                listing.CheckboxLabeled("MegaMod_AutoArm_ConsiderQuality".Translate(), ref settings.considerQuality, "MegaMod_AutoArm_ConsiderQuality_Desc".Translate());
                listing.Gap(6f);

                listing.CheckboxLabeled("MegaMod_AutoArm_ConsiderHP".Translate(), ref settings.considerHitPoints, "MegaMod_AutoArm_ConsiderHP_Desc".Translate());
                listing.Gap(6f);

                listing.CheckboxLabeled("MegaMod_AutoArm_UnforbidDropped".Translate(), ref settings.autoUnforbidDropped, "MegaMod_AutoArm_UnforbidDropped_Desc".Translate());
                listing.Gap(10f);

                if (listing.ButtonText("MegaMod_AutoArm_ManageBlacklist".Translate()))
                {
                    Find.WindowStack.Add(new Dialog_AutoArmBlacklist(settings));
                }
                listing.Gap(12f);

                settings.minScoreDifference = listing.SliderLabeled(
                    "MegaMod_AutoArm_MinDiff".Translate() + $": {settings.minScoreDifference:P0}",
                    settings.minScoreDifference, 0.05f, 0.50f, 0.5f,
                    "MegaMod_AutoArm_MinDiff_Desc".Translate());
                listing.Gap(8f);

                settings.checkIntervalSeconds = (int)listing.SliderLabeled(
                    "MegaMod_AutoArm_Interval".Translate() + $": {settings.checkIntervalSeconds} s",
                    settings.checkIntervalSeconds, 10f, 120f, 0.5f,
                    "MegaMod_AutoArm_Interval_Desc".Translate());
                listing.Gap(16f);

                DrawHierarchyExplanation(listing);
            }

            listing.End();
            Widgets.EndScrollView();
        }

        private static void DrawHierarchyExplanation(Listing_Standard listing)
        {
            Rect boxRect = listing.GetRect(220f);
            Widgets.DrawBoxSolid(boxRect, new Color(0.12f, 0.13f, 0.16f, 0.9f));
            Widgets.DrawHighlightIfMouseover(boxRect);

            Rect content = boxRect.ContractedBy(10f);
            Listing_Standard inner = new Listing_Standard();
            inner.Begin(content);

            Text.Font = GameFont.Small;
            GUI.color = new Color(1f, 0.88f, 0.4f);
            inner.Label("MegaMod_AutoArm_HierarchyHeader".Translate());
            GUI.color = Color.white;
            inner.Gap(4f);

            // Tier 1: Ranged Single-Shot
            GUI.color = new Color(0.4f, 0.8f, 1f);
            inner.Label("MegaMod_AutoArm_Tier1_Title".Translate() + " - " + "MegaMod_AutoArm_Tier1_Desc".Translate());
            inner.Gap(3f);

            // Tier 2: Ranged Burst
            GUI.color = new Color(1f, 0.75f, 0.3f);
            inner.Label("MegaMod_AutoArm_Tier2_Title".Translate() + " - " + "MegaMod_AutoArm_Tier2_Desc".Translate());
            inner.Gap(3f);

            // Tier 3: Melee Sharp
            GUI.color = new Color(1f, 0.45f, 0.45f);
            inner.Label("MegaMod_AutoArm_Tier3_Title".Translate() + " - " + "MegaMod_AutoArm_Tier3_Desc".Translate());
            inner.Gap(3f);

            // Tier 4: Melee Blunt
            GUI.color = new Color(0.45f, 0.95f, 0.55f);
            inner.Label("MegaMod_AutoArm_Tier4_Title".Translate() + " - " + "MegaMod_AutoArm_Tier4_Desc".Translate());

            GUI.color = Color.white;
            inner.End();
        }
    }
}
