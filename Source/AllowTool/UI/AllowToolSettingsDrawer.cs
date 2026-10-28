using UnityEngine;
using Verse;
using SadSeal.Common.UI;

namespace SadSeal.MegaMod.AllowTool.UI
{
    public static class AllowToolSettingsDrawer
    {
        private static Vector2 scrollPosition = Vector2.zero;

        public static void DrawSettings(Rect inRect, AllowToolSettings settings)
        {
            if (settings == null) return;

            Rect viewRect = new Rect(0f, 0f, inRect.width - 24f, 600f);
            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            SadUI.DrawHeader(listing, "MegaMod_AllowTool_Title", "MegaMod_AllowTool_Desc");

            listing.CheckboxLabeled("MegaMod_AllowTool_EnableUnforbid".Translate(), ref settings.EnableUnforbidAll, "MegaMod_AllowTool_EnableUnforbid_Desc".Translate());
            listing.Gap(8f);

            listing.CheckboxLabeled("MegaMod_AllowTool_EnableHaulUrgently".Translate(), ref settings.EnableHaulUrgently, "MegaMod_AllowTool_EnableHaulUrgently_Desc".Translate());
            listing.Gap(8f);

            listing.CheckboxLabeled("MegaMod_AllowTool_EnableSelectSimilar".Translate(), ref settings.EnableSelectSimilar, "MegaMod_AllowTool_EnableSelectSimilar_Desc".Translate());
            listing.Gap(8f);

            listing.CheckboxLabeled("MegaMod_AllowTool_EnableFinishOff".Translate(), ref settings.EnableFinishOff, "MegaMod_AllowTool_EnableFinishOff_Desc".Translate());

            listing.End();
            Widgets.EndScrollView();
        }
    }
}
