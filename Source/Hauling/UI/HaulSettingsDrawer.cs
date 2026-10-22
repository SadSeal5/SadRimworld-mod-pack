using System;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Hauling.UI
{
    public static class HaulSettingsDrawer
    {
        private static Vector2 scrollPosition = Vector2.zero;

        public static void DrawSettings(Rect inRect, HaulSettings settings)
        {
            if (settings == null) return;

            Rect viewRect = new Rect(0f, 0f, inRect.width - 24f, 540f);
            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            Text.Font = GameFont.Medium;
            listing.Label("SadHaul_ModTitle".Translate());
            Text.Font = GameFont.Small;
            listing.Label("SadHaul_ModDesc".Translate());
            listing.Gap(10f);

            // Preset Buttons Section
            listing.Label("SadHaul_PresetsHeader".Translate());
            Rect presetRect = listing.GetRect(32f);
            float btnWidth = (presetRect.width - 15f) / 4f;

            Color oldColor = GUI.color;

            if (settings.ActivePreset == HaulPreset.Balanced) GUI.color = Color.green;
            if (Widgets.ButtonText(new Rect(presetRect.x, presetRect.y, btnWidth, 32f), "SadHaul_PresetBalanced".Translate()))
            {
                settings.ApplyPreset(HaulPreset.Balanced);
            }
            GUI.color = oldColor;

            if (settings.ActivePreset == HaulPreset.Performance) GUI.color = Color.green;
            if (Widgets.ButtonText(new Rect(presetRect.x + btnWidth + 5f, presetRect.y, btnWidth, 32f), "SadHaul_PresetPerformance".Translate()))
            {
                settings.ApplyPreset(HaulPreset.Performance);
            }
            GUI.color = oldColor;

            if (settings.ActivePreset == HaulPreset.MaximumThroughput) GUI.color = Color.green;
            if (Widgets.ButtonText(new Rect(presetRect.x + (btnWidth + 5f) * 2f, presetRect.y, btnWidth, 32f), "SadHaul_PresetMaximum".Translate()))
            {
                settings.ApplyPreset(HaulPreset.MaximumThroughput);
            }
            GUI.color = oldColor;

            if (Widgets.ButtonText(new Rect(presetRect.x + (btnWidth + 5f) * 3f, presetRect.y, btnWidth, 32f), "SadHaul_ResetDefaults".Translate()))
            {
                settings.ResetToDefaults();
            }

            listing.Gap(14f);

            // Toggles
            Text.Font = GameFont.Medium;
            listing.Label("SadHaul_SectionGeneral".Translate());
            Text.Font = GameFont.Small;
            listing.Gap(4f);

            bool oldSmart = settings.EnableSmartHaul;
            listing.CheckboxLabeled("SadHaul_Enable".Translate(), ref settings.EnableSmartHaul, "SadHaul_Enable_Desc".Translate());
            if (oldSmart != settings.EnableSmartHaul) settings.ActivePreset = HaulPreset.Custom;

            bool oldDraft = settings.DropOnDraft;
            listing.CheckboxLabeled("SadHaul_DropOnDraft".Translate(), ref settings.DropOnDraft, "SadHaul_DropOnDraft_Desc".Translate());
            if (oldDraft != settings.DropOnDraft) settings.ActivePreset = HaulPreset.Custom;

            bool oldCluster = settings.StrictClustering;
            listing.CheckboxLabeled("SadHaul_StrictClustering".Translate(), ref settings.StrictClustering, "SadHaul_StrictClustering_Desc".Translate());
            if (oldCluster != settings.StrictClustering) settings.ActivePreset = HaulPreset.Custom;

            bool oldAnimals = settings.AnimalsCanSmartHaul;
            listing.CheckboxLabeled("SadHaul_Animals".Translate(), ref settings.AnimalsCanSmartHaul, "SadHaul_Animals_Desc".Translate());
            if (oldAnimals != settings.AnimalsCanSmartHaul) settings.ActivePreset = HaulPreset.Custom;

            listing.Gap(14f);

            // Sliders Section
            Text.Font = GameFont.Medium;
            listing.Label("SadHaul_SectionLimits".Translate());
            Text.Font = GameFont.Small;
            listing.Gap(4f);

            int oldRadius = settings.SearchRadius;
            settings.SearchRadius = (int)listing.SliderLabeled(
                "SadHaul_Radius".Translate() + $": {settings.SearchRadius} c",
                settings.SearchRadius, 4f, 20f, 0.5f,
                "SadHaul_Radius_Desc".Translate());
            if (oldRadius != settings.SearchRadius) settings.ActivePreset = HaulPreset.Custom;

            int oldMaxItems = settings.MaxItemsPerTrip;
            settings.MaxItemsPerTrip = (int)listing.SliderLabeled(
                "SadHaul_MaxItems".Translate() + $": {settings.MaxItemsPerTrip}",
                settings.MaxItemsPerTrip, 2f, 16f, 0.5f,
                "SadHaul_MaxItems_Desc".Translate());
            if (oldMaxItems != settings.MaxItemsPerTrip) settings.ActivePreset = HaulPreset.Custom;

            float oldMass = settings.MassLimitPercent;
            settings.MassLimitPercent = listing.SliderLabeled(
                "SadHaul_MassLimit".Translate() + $": {settings.MassLimitPercent:P0}",
                settings.MassLimitPercent, 0.50f, 1.00f, 0.5f,
                "SadHaul_MassLimit_Desc".Translate());
            if (Math.Abs(oldMass - settings.MassLimitPercent) > 0.005f) settings.ActivePreset = HaulPreset.Custom;

            listing.End();
            Widgets.EndScrollView();
        }
    }
}
