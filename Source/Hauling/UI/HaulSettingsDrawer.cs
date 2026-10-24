using System;
using UnityEngine;
using Verse;
using SadSeal.Common.UI;

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

            SadUI.DrawHeader(listing, "SadHaul_ModTitle", "SadHaul_ModDesc");

            // Preset Buttons Section
            listing.Label("SadHaul_PresetsHeader".Translate());
            SadUI.DrawPresetRow(listing, settings.ActivePreset, new (string, HaulPreset)[]
            {
                ("SadHaul_PresetBalanced", HaulPreset.Balanced),
                ("SadHaul_PresetPerformance", HaulPreset.Performance),
                ("SadHaul_PresetMaximum", HaulPreset.MaximumThroughput)
            }, preset => settings.ApplyPreset(preset), () => settings.ResetToDefaults(), "SadHaul_ResetDefaults");

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
