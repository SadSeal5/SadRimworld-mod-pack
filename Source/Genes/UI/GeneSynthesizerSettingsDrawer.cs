using System;
using System.Globalization;
using UnityEngine;
using Verse;
using SadSeal.Common.UI;

namespace SadSeal.MegaMod.Genes.UI
{
    public static class GeneSynthesizerSettingsDrawer
    {
        private static Vector2 scrollPosition = Vector2.zero;
        private static string costBuffer = null;
        private static string workBuffer = null;

        public static void ClearBuffers()
        {
            costBuffer = null;
            workBuffer = null;
        }

        public static void DrawSettings(Rect inRect, GeneSynthesizerSettings settings)
        {
            Rect outRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            Rect viewRect = new Rect(0f, 0f, inRect.width - 24f, 520f);

            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            // Title & Description
            SadUI.DrawHeader(listing, "GeneSynthesis_SettingsHeader", "GeneSynthesis_SettingsDesc");

            // Presets Section
            Text.Font = GameFont.Medium;
            listing.Label("GeneSynthesis_Presets".Translate());
            Text.Font = GameFont.Small;
            listing.Gap(4f);

            SadUI.DrawPresetRow(listing, settings.ActivePreset, new (string, GeneSynthesizerPreset)[]
            {
                ("GeneSynthesis_PresetFree", GeneSynthesizerPreset.Free),
                ("GeneSynthesis_PresetCheap", GeneSynthesizerPreset.Cheap),
                ("GeneSynthesis_PresetStandard", GeneSynthesizerPreset.Standard),
                ("GeneSynthesis_PresetHardcore", GeneSynthesizerPreset.Hardcore)
            }, preset =>
            {
                settings.ApplyPreset(preset);
                ClearBuffers();
            }, () =>
            {
                settings.ResetToDefaults();
                ClearBuffers();
            }, "GeneSynthesis_ResetDefaults", spacing: 8f);

            listing.Gap(16f);

            // Cost & Work Config Section
            Text.Font = GameFont.Medium;
            listing.Label("GeneSynthesis_ResourceAndWork".Translate());
            Text.Font = GameFont.Small;
            listing.Gap(6f);

            // Neutroamine Cost Slider & Text
            Rect costRowRect = listing.GetRect(32f);
            SadUI.DrawIntRow(costRowRect, "GeneSynthesis_NeutroamineCostLabel".Translate(), settings.NeutroamineCost, 0, 1000, ref costBuffer, newVal =>
            {
                settings.NeutroamineCost = newVal;
                settings.ActivePreset = GeneSynthesizerPreset.Custom;
            });
            listing.Gap(4f);

            // Work Amount Slider & Text
            Rect workRowRect = listing.GetRect(32f);
            string workLabel = settings.InstantCraft || settings.WorkAmount <= 0f
                ? "GeneSynthesis_WorkAmountLabel".Translate() + " (" + "GeneSynthesis_WorkInstantShort".Translate() + ")"
                : "GeneSynthesis_WorkAmountLabel".Translate() + $" ({settings.WorkAmount:F0} ticks / ~{settings.WorkAmount / 60f:F0}s)";

            SadUI.DrawFloatRow(workRowRect, workLabel, settings.WorkAmount, 0f, 10000f, ref workBuffer, newVal =>
            {
                settings.WorkAmount = newVal;
                settings.ActivePreset = GeneSynthesizerPreset.Custom;
            });
            listing.Gap(12f);

            // Toggles
            Text.Font = GameFont.Medium;
            listing.Label("GeneSynthesis_RulesAndToggles".Translate());
            Text.Font = GameFont.Small;
            listing.Gap(6f);

            bool oldInstant = settings.InstantCraft;
            listing.CheckboxLabeled(
                "GeneSynthesis_InstantCraft".Translate(),
                ref settings.InstantCraft,
                "GeneSynthesis_InstantCraft_Desc".Translate()
            );
            if (oldInstant != settings.InstantCraft) settings.ActivePreset = GeneSynthesizerPreset.Custom;

            bool oldArchite = settings.AllowArchiteGenes;
            listing.CheckboxLabeled(
                "GeneSynthesis_AllowArchite".Translate(),
                ref settings.AllowArchiteGenes,
                "GeneSynthesis_AllowArchite_Desc".Translate()
            );
            if (oldArchite != settings.AllowArchiteGenes) settings.ActivePreset = GeneSynthesizerPreset.Custom;

            bool oldCapsules = settings.RequireArchiteCapsules;
            listing.CheckboxLabeled(
                "GeneSynthesis_RequireArchiteCapsules".Translate(),
                ref settings.RequireArchiteCapsules,
                "GeneSynthesis_RequireArchiteCapsules_Desc".Translate()
            );
            if (oldCapsules != settings.RequireArchiteCapsules) settings.ActivePreset = GeneSynthesizerPreset.Custom;

            listing.End();
            Widgets.EndScrollView();
        }
    }
}
