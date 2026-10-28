using UnityEngine;
using Verse;
using SadSeal.Common.UI;

namespace SadSeal.MegaMod.TimeFlow.UI
{
    public static class TimeFlowSettingsDrawer
    {
        private static Vector2 scrollPosition = Vector2.zero;

        public static void DrawSettings(Rect inRect, TimeFlowSettings settings)
        {
            if (settings == null) return;

            Rect viewRect = new Rect(0f, 0f, inRect.width - 24f, 600f);
            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            SadUI.DrawHeader(listing, "MegaMod_TimeFlow_Title", "MegaMod_TimeFlow_Desc");

            listing.CheckboxLabeled("MegaMod_TimeFlow_UnlockSuperSpeed".Translate(), ref settings.UnlockSuperSpeed, "MegaMod_TimeFlow_UnlockSuperSpeed_Desc".Translate());
            listing.Gap(8f);

            listing.CheckboxLabeled("MegaMod_TimeFlow_DisableForcedSlowdown".Translate(), ref settings.DisableForcedSlowdown, "MegaMod_TimeFlow_DisableForcedSlowdown_Desc".Translate());
            listing.Gap(8f);

            listing.CheckboxLabeled("MegaMod_TimeFlow_SmartSleep".Translate(), ref settings.EnableSmartSleepFastForward, "MegaMod_TimeFlow_SmartSleep_Desc".Translate());

            if (settings.EnableSmartSleepFastForward)
            {
                Rect row = listing.GetRect(30f);
                Widgets.Label(new Rect(row.x, row.y + 4f, 260f, 24f), "MegaMod_TimeFlow_SleepSpeedTarget".Translate());

                string speedLabel = settings.SleepFastForwardSpeed == 4 ? "4x (Ultrafast)" : "3x (Superfast)";
                if (Widgets.ButtonText(new Rect(row.x + 270f, row.y, 160f, 28f), speedLabel))
                {
                    settings.SleepFastForwardSpeed = (settings.SleepFastForwardSpeed == 4) ? 3 : 4;
                }
            }

            listing.End();
            Widgets.EndScrollView();
        }
    }
}
