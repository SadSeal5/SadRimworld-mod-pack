using UnityEngine;
using Verse;
using SadSeal.Common.UI;

namespace SadSeal.MegaMod.Incubation.UI
{
    public static class IncubationSettingsDrawer
    {
        private static Vector2 scrollPosition = Vector2.zero;
        private static readonly int[] SupportedAges = new int[] { 3, 7, 10, 13, 18 };

        public static void DrawSettings(Rect inRect, IncubationSettings settings)
        {
            if (settings == null) return;

            Rect viewRect = new Rect(0f, 0f, inRect.width - 24f, 820f);
            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            SadUI.DrawHeader(listing, "MegaMod_Incubation_Title", "MegaMod_Incubation_Desc");

            if (!ModLister.BiotechInstalled)
            {
                GUI.color = Color.yellow;
                listing.Label("MegaMod_Incubation_BiotechRequired".Translate());
                GUI.color = Color.white;
                listing.Gap(12f);
            }

            // Section 1: Speeds
            Text.Font = GameFont.Medium;
            listing.Label("MegaMod_Incubation_SpeedSection".Translate());
            Text.Font = GameFont.Small;
            listing.Gap(4f);

            settings.embryoSpeedMultiplier = listing.SliderLabeled(
                "MegaMod_Incubation_EmbryoSpeed".Translate() + $": {settings.embryoSpeedMultiplier:0.#}x",
                settings.embryoSpeedMultiplier, 0.1f, 100.0f, 0.5f,
                "MegaMod_Incubation_EmbryoSpeed_Desc".Translate());
            listing.Gap(4f);

            settings.childSpeedMultiplier = listing.SliderLabeled(
                "MegaMod_Incubation_ChildSpeed".Translate() + $": {settings.childSpeedMultiplier:0.#}x (Total: {Mathf.RoundToInt(20f * settings.childSpeedMultiplier)}x)",
                settings.childSpeedMultiplier, 0.1f, 100.0f, 0.5f,
                "MegaMod_Incubation_ChildSpeed_Desc".Translate());
            listing.Gap(4f);

            // Preset Speed Buttons
            Rect presetRow = listing.GetRect(28f);
            Widgets.Label(new Rect(presetRow.x, presetRow.y + 4f, 160f, 24f), "MegaMod_Incubation_Presets".Translate() + ":");
            float[] presetSpeeds = new float[] { 1f, 2f, 5f, 10f, 20f, 50f };
            float btnX = presetRow.x + 165f;
            float btnWidth = 46f;
            for (int i = 0; i < presetSpeeds.Length; i++)
            {
                float speed = presetSpeeds[i];
                if (Widgets.ButtonText(new Rect(btnX + i * (btnWidth + 4f), presetRow.y, btnWidth, 24f), $"{speed:0}x"))
                {
                    settings.embryoSpeedMultiplier = speed;
                    settings.childSpeedMultiplier = speed;
                }
            }

            listing.Gap(14f);

            // Section 2: Resource Consumption
            Text.Font = GameFont.Medium;
            listing.Label("MegaMod_Incubation_ResourcesSection".Translate());
            Text.Font = GameFont.Small;
            listing.Gap(4f);

            settings.nutritionUsageMultiplier = listing.SliderLabeled(
                "MegaMod_Incubation_NutritionMultiplier".Translate() + $": {settings.nutritionUsageMultiplier:P0}",
                settings.nutritionUsageMultiplier, 0.0f, 5.0f, 0.5f,
                "MegaMod_Incubation_NutritionMultiplier_Desc".Translate());
            listing.Gap(4f);

            settings.powerUsageMultiplier = listing.SliderLabeled(
                "MegaMod_Incubation_PowerMultiplier".Translate() + $": {settings.powerUsageMultiplier:P0}",
                settings.powerUsageMultiplier, 0.0f, 5.0f, 0.5f,
                "MegaMod_Incubation_PowerMultiplier_Desc".Translate());

            listing.Gap(14f);

            // Section 3: Automation & Protection
            Text.Font = GameFont.Medium;
            listing.Label("MegaMod_Incubation_SafetySection".Translate());
            Text.Font = GameFont.Small;
            listing.Gap(4f);

            listing.CheckboxLabeled(
                "MegaMod_Incubation_AutoEject".Translate(),
                ref settings.autoEjectEnabled,
                "MegaMod_Incubation_AutoEject_Desc".Translate());
            listing.Gap(4f);

            if (settings.autoEjectEnabled)
            {
                Rect ageRow = listing.GetRect(28f);
                Widgets.Label(new Rect(ageRow.x + 16f, ageRow.y + 4f, 220f, 24f), "MegaMod_Incubation_TargetAge".Translate() + $": {settings.targetEjectAge}");
                float ageBtnX = ageRow.x + 240f;
                float ageBtnWidth = 42f;
                Color oldColor = GUI.color;
                for (int i = 0; i < SupportedAges.Length; i++)
                {
                    int age = SupportedAges[i];
                    bool isSelected = settings.targetEjectAge == age;
                    GUI.color = isSelected ? Color.green : oldColor;
                    if (Widgets.ButtonText(new Rect(ageBtnX + i * (ageBtnWidth + 4f), ageRow.y, ageBtnWidth, 24f), age.ToString()))
                    {
                        settings.targetEjectAge = age;
                    }
                }
                GUI.color = oldColor;
                listing.Gap(6f);
            }

            listing.CheckboxLabeled(
                "MegaMod_Incubation_PreserveGrowthTier".Translate(),
                ref settings.preserveGrowthTier,
                "MegaMod_Incubation_PreserveGrowthTier_Desc".Translate());
            listing.Gap(4f);

            listing.CheckboxLabeled(
                "MegaMod_Incubation_StasisProtection".Translate(),
                ref settings.stasisProtection,
                "MegaMod_Incubation_StasisProtection_Desc".Translate());
            listing.Gap(4f);

            listing.CheckboxLabeled(
                "MegaMod_Incubation_DisableParentGuilt".Translate(),
                ref settings.disableParentGuilt,
                "MegaMod_Incubation_DisableParentGuilt_Desc".Translate());

            listing.Gap(16f);

            // Reset Button
            Rect resetRect = listing.GetRect(30f);
            if (Widgets.ButtonText(new Rect(resetRect.x, resetRect.y, 200f, 30f), "MegaMod_Incubation_ResetDefaults".Translate()))
            {
                settings.ResetToDefaults();
            }

            listing.End();
            Widgets.EndScrollView();
        }
    }
}
