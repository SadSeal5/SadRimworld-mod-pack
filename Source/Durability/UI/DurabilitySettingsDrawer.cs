using System;
using SadSeal.Common.UI;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Durability.UI
{
    public static class DurabilitySettingsDrawer
    {
        private static Vector2 scrollPosition = Vector2.zero;

        public static void DrawSettings(Rect inRect, DurabilitySettings settings)
        {
            if (settings == null) return;

            Rect viewRect = new Rect(0f, 0f, inRect.width - 24f, 520f);
            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            SadUI.DrawHeader(listing, "MegaMod_DurabilityTitle", "MegaMod_DurabilityDesc");

            bool oldApparel = settings.removeApparelDurability;
            listing.CheckboxLabeled(
                "MegaMod_RemoveApparelDurability".Translate(),
                ref settings.removeApparelDurability,
                "MegaMod_RemoveApparelDurability_Desc".Translate());
            if (oldApparel != settings.removeApparelDurability)
            {
                DurabilityApplier.ApplyDurabilitySettings();
            }

            bool oldWeapon = settings.removeWeaponDurability;
            listing.CheckboxLabeled(
                "MegaMod_RemoveWeaponDurability".Translate(),
                ref settings.removeWeaponDurability,
                "MegaMod_RemoveWeaponDurability_Desc".Translate());
            if (oldWeapon != settings.removeWeaponDurability)
            {
                DurabilityApplier.ApplyDurabilitySettings();
            }

            listing.Gap(6f);

            listing.CheckboxLabeled(
                "MegaMod_RestoreExistingOnLoad".Translate(),
                ref settings.restoreExistingOnLoad,
                "MegaMod_RestoreExistingOnLoad_Desc".Translate());

            listing.CheckboxLabeled(
                "MegaMod_PreventTaintedApparel".Translate(),
                ref settings.preventTaintedApparel,
                "MegaMod_PreventTaintedApparel_Desc".Translate());

            listing.Gap(14f);

            // Button: Restore All In-Game Gear Now
            Rect btnRect = listing.GetRect(34f);
            btnRect.width = Mathf.Min(btnRect.width, 360f);
            bool isPlaying = Current.ProgramState == ProgramState.Playing;
            Color prevColor = GUI.color;
            if (!isPlaying)
            {
                GUI.color = new Color(0.7f, 0.7f, 0.7f, 0.6f);
            }

            if (Widgets.ButtonText(btnRect, "MegaMod_RestoreGearNow".Translate()))
            {
                if (isPlaying)
                {
                    DurabilityApplier.RestoreAllDamagedItemsInWorld();
                }
            }
            GUI.color = prevColor;

            if (!isPlaying)
            {
                TooltipHandler.TipRegion(btnRect, "MegaMod_RestoreGearOnlyInGame".Translate());
            }

            listing.Gap(18f);

            // Informational box about 0-tick performance
            Rect boxRect = listing.GetRect(75f);
            Widgets.DrawMenuSection(boxRect);
            Rect innerBox = boxRect.ContractedBy(8f);
            Color textPrev = GUI.color;
            GUI.color = new Color(0.4f, 0.85f, 1f, 1f);
            Widgets.Label(innerBox, "MegaMod_DurabilityPerfNote".Translate());
            GUI.color = textPrev;

            listing.End();
            Widgets.EndScrollView();
        }
    }
}
