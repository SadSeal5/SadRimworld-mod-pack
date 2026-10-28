using UnityEngine;
using Verse;
using SadSeal.Common.UI;

namespace SadSeal.MegaMod.Quarry.UI
{
    public static class QuarrySettingsDrawer
    {
        private static Vector2 scrollPosition = Vector2.zero;

        public static void DrawSettings(Rect inRect, QuarrySettings settings)
        {
            if (settings == null) return;

            Rect viewRect = new Rect(0f, 0f, inRect.width - 24f, 750f);
            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            SadUI.DrawHeader(listing, "MegaMod_Quarry_Title", "MegaMod_Quarry_Desc");

            listing.CheckboxLabeled("MegaMod_Quarry_InfiniteQuarry".Translate(), ref settings.InfiniteQuarry, "MegaMod_Quarry_InfiniteQuarry_Desc".Translate());
            listing.Gap(8f);

            settings.MiningSpeedMultiplier = listing.SliderLabeled(
                "MegaMod_Quarry_MiningSpeed".Translate() + $": {settings.MiningSpeedMultiplier:P0}",
                settings.MiningSpeedMultiplier, 0.25f, 3.0f, 0.5f,
                "MegaMod_Quarry_MiningSpeed_Desc".Translate());
            listing.Gap(12f);

            listing.Label("MegaMod_Quarry_ResourceWeightsHeader".Translate());

            settings.WeightSteel = listing.SliderLabeled(
                "MegaMod_Quarry_WeightSteel".Translate() + $": {settings.WeightSteel:F0}",
                settings.WeightSteel, 0f, 100f, 0.5f);

            settings.WeightPlasteel = listing.SliderLabeled(
                "MegaMod_Quarry_WeightPlasteel".Translate() + $": {settings.WeightPlasteel:F0}",
                settings.WeightPlasteel, 0f, 100f, 0.5f);

            settings.WeightUranium = listing.SliderLabeled(
                "MegaMod_Quarry_WeightUranium".Translate() + $": {settings.WeightUranium:F0}",
                settings.WeightUranium, 0f, 100f, 0.5f);

            settings.WeightComponents = listing.SliderLabeled(
                "MegaMod_Quarry_WeightComponents".Translate() + $": {settings.WeightComponents:F0}",
                settings.WeightComponents, 0f, 100f, 0.5f);

            settings.WeightGold = listing.SliderLabeled(
                "MegaMod_Quarry_WeightGold".Translate() + $": {settings.WeightGold:F0}",
                settings.WeightGold, 0f, 100f, 0.5f);

            settings.WeightSilver = listing.SliderLabeled(
                "MegaMod_Quarry_WeightSilver".Translate() + $": {settings.WeightSilver:F0}",
                settings.WeightSilver, 0f, 100f, 0.5f);

            listing.End();
            Widgets.EndScrollView();
        }
    }
}
