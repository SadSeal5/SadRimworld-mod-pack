using System;
using SadSeal.Common.UI;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.QoL.UI
{
    public static class QoLSettingsDrawer
    {
        private static Vector2 scrollPosition = Vector2.zero;
        private static string chunkStackBuffer;

        public static void DrawSettings(Rect inRect, QoLSettings settings)
        {
            if (settings == null) return;

            Rect viewRect = new Rect(0f, 0f, inRect.width - 24f, 1300f);
            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            SadUI.DrawHeader(listing, "MegaMod_QoLTitle", "MegaMod_QoLDesc");

            // Preset Buttons Row
            listing.Label("MegaMod_QoLPresetsHeader".Translate());
            Rect presetRect = listing.GetRect(32f);
            float btnSpacing = 6f;
            float btnWidth = (presetRect.width - (btnSpacing * 3f)) / 4f;

            Rect btnBalanced = new Rect(presetRect.x, presetRect.y, btnWidth, 32f);
            Rect btnComfort = new Rect(btnBalanced.xMax + btnSpacing, presetRect.y, btnWidth, 32f);
            Rect btnVanilla = new Rect(btnComfort.xMax + btnSpacing, presetRect.y, btnWidth, 32f);
            Rect btnReset = new Rect(btnVanilla.xMax + btnSpacing, presetRect.y, btnWidth, 32f);

            if (Widgets.ButtonText(btnBalanced, "MegaMod_QoLPresetBalanced".Translate()))
            {
                settings.SetPresetBalanced();
                chunkStackBuffer = null;
            }

            if (Widgets.ButtonText(btnComfort, "MegaMod_QoLPresetMaxComfort".Translate()))
            {
                settings.SetPresetMaxComfort();
                chunkStackBuffer = null;
            }

            if (Widgets.ButtonText(btnVanilla, "MegaMod_QoLPresetVanilla".Translate()))
            {
                settings.SetPresetVanilla();
                chunkStackBuffer = null;
            }

            if (Widgets.ButtonText(btnReset, "MegaMod_ResetDefault".Translate()))
            {
                settings.ResetToDefaults();
                chunkStackBuffer = null;
            }

            listing.Gap(14f);

            // ----------------------------------------------------
            // Section 1: Pawn Behavior
            // ----------------------------------------------------
            Text.Font = GameFont.Medium;
            listing.Label("MegaMod_QoLSectionPawn".Translate());
            Text.Font = GameFont.Small;
            listing.GapLine();

            listing.CheckboxLabeled(
                "MegaMod_QoLUghYouGotMe".Translate(),
                ref settings.ughYouGotMe,
                "MegaMod_QoLUghYouGotMe_Desc".Translate());

            listing.CheckboxLabeled(
                "MegaMod_QoLWhereIsMyWeapon".Translate(),
                ref settings.whereIsMyWeapon,
                "MegaMod_QoLWhereIsMyWeapon_Desc".Translate());

            listing.CheckboxLabeled(
                "MegaMod_QoLPrioritizeSleep".Translate(),
                ref settings.prioritizeSleep,
                "MegaMod_QoLPrioritizeSleep_Desc".Translate());

            listing.CheckboxLabeled(
                "MegaMod_QoLDontBlockDoors".Translate(),
                ref settings.dontBlockDoors,
                "MegaMod_QoLDontBlockDoors_Desc".Translate());

            listing.CheckboxLabeled(
                "MegaMod_QoLEnableCleanBeforeWork".Translate(),
                ref settings.enableCleanBeforeWork,
                "MegaMod_QoLEnableCleanBeforeWork_Desc".Translate());

            listing.CheckboxLabeled(
                "MegaMod_QoLEnableAutoCutBlight".Translate(),
                ref settings.enableAutoCutBlight,
                "MegaMod_QoLEnableAutoCutBlight_Desc".Translate());

            listing.CheckboxLabeled(
                "MegaMod_QoLEnableSnapOut".Translate(),
                ref settings.enableSnapOut,
                "MegaMod_QoLEnableSnapOut_Desc".Translate());

            listing.Gap(14f);

            // ----------------------------------------------------
            // Section 2: Base & Map Management
            // ----------------------------------------------------
            Text.Font = GameFont.Medium;
            listing.Label("MegaMod_QoLSectionBase".Translate());
            Text.Font = GameFont.Small;
            listing.GapLine();

            bool oldMetal = settings.metalDoesntBurn;
            listing.CheckboxLabeled(
                "MegaMod_QoLMetalDoesntBurn".Translate(),
                ref settings.metalDoesntBurn,
                "MegaMod_QoLMetalDoesntBurn_Desc".Translate());
            if (oldMetal != settings.metalDoesntBurn)
            {
                QoLApplier.ApplyMetalFlammability(settings.metalDoesntBurn);
            }

            listing.CheckboxLabeled(
                "MegaMod_QoLFilthVanishes".Translate(),
                ref settings.filthVanishes,
                "MegaMod_QoLFilthVanishes_Desc".Translate());

            bool oldStack = settings.stackableChunks;
            listing.CheckboxLabeled(
                "MegaMod_QoLStackableChunks".Translate(),
                ref settings.stackableChunks,
                "MegaMod_QoLStackableChunks_Desc".Translate());
            if (oldStack != settings.stackableChunks)
            {
                QoLApplier.ApplyChunkStackLimits(settings.stackableChunks, settings.chunkStackLimit);
            }

            if (settings.stackableChunks)
            {
                Rect sliderRect = listing.GetRect(28f);
                int oldLimit = settings.chunkStackLimit;
                SadUI.DrawIntRow(
                    sliderRect,
                    "MegaMod_QoLChunkStackLimit".Translate() + $": {settings.chunkStackLimit}",
                    settings.chunkStackLimit,
                    1,
                    20,
                    ref chunkStackBuffer,
                    newVal =>
                    {
                        settings.chunkStackLimit = newVal;
                        QoLApplier.ApplyChunkStackLimits(true, newVal);
                    });
            }

            bool oldTrees = settings.treesDontDie;
            listing.CheckboxLabeled(
                "MegaMod_QoLTreesDontDie".Translate(),
                ref settings.treesDontDie,
                "MegaMod_QoLTreesDontDie_Desc".Translate());
            if (oldTrees != settings.treesDontDie)
            {
                QoLApplier.ApplyTreeLifespan(settings.treesDontDie);
            }

            bool oldPrecious = settings.prettyPreciousMetals;
            listing.CheckboxLabeled(
                "MegaMod_QoLPrettyPreciousMetals".Translate(),
                ref settings.prettyPreciousMetals,
                "MegaMod_QoLPrettyPreciousMetals_Desc".Translate());
            if (oldPrecious != settings.prettyPreciousMetals)
            {
                QoLApplier.ApplyPreciousMetals(settings.prettyPreciousMetals);
            }

            listing.Gap(14f);

            // ----------------------------------------------------
            // Section 3: Workshop & Jobs
            // ----------------------------------------------------
            Text.Font = GameFont.Medium;
            listing.Label("MegaMod_QoLSectionWork".Translate());
            Text.Font = GameFont.Small;
            listing.GapLine();

            listing.CheckboxLabeled(
                "MegaMod_QoLNoJobAuthors".Translate(),
                ref settings.noJobAuthors,
                "MegaMod_QoLNoJobAuthors_Desc".Translate());

            bool oldSmooth = settings.smoothWallsBeforeFloors;
            listing.CheckboxLabeled(
                "MegaMod_QoLSmoothWallsBeforeFloors".Translate(),
                ref settings.smoothWallsBeforeFloors,
                "MegaMod_QoLSmoothWallsBeforeFloors_Desc".Translate());
            if (oldSmooth != settings.smoothWallsBeforeFloors)
            {
                QoLApplier.ApplySmoothPriorities(settings.smoothWallsBeforeFloors);
            }

            listing.CheckboxLabeled(
                "MegaMod_QoLRepairInZone".Translate(),
                ref settings.repairInZone,
                "MegaMod_QoLRepairInZone_Desc".Translate());

            listing.CheckboxLabeled(
                "MegaMod_QoLHaulAfterSlaughter".Translate(),
                ref settings.haulAfterSlaughter,
                "MegaMod_QoLHaulAfterSlaughter_Desc".Translate());

            listing.CheckboxLabeled(
                "MegaMod_QoLEnableBillQoL".Translate(),
                ref settings.enableBillQoL,
                "MegaMod_QoLEnableBillQoL_Desc".Translate());

            listing.Gap(14f);

            // ----------------------------------------------------
            // Section 4: Construction & Safety
            // ----------------------------------------------------
            Text.Font = GameFont.Medium;
            listing.Label("MegaMod_QoLSectionSafety".Translate());
            Text.Font = GameFont.Small;
            listing.GapLine();

            listing.CheckboxLabeled(
                "MegaMod_QoLEnableRoofSafety".Translate(),
                ref settings.enableRoofSafety,
                "MegaMod_QoLEnableRoofSafety_Desc".Translate());

            listing.CheckboxLabeled(
                "MegaMod_QoLEnableReplaceStuff".Translate(),
                ref settings.enableReplaceStuff,
                "MegaMod_QoLEnableReplaceStuff_Desc".Translate());

            listing.CheckboxLabeled(
                "MegaMod_QoLEnableShareTheLoad".Translate(),
                ref settings.enableShareTheLoad,
                "MegaMod_QoLEnableShareTheLoad_Desc".Translate());

            listing.Gap(14f);

            // ----------------------------------------------------
            // Section 5: Raids & Events
            // ----------------------------------------------------
            Text.Font = GameFont.Medium;
            listing.Label("MegaMod_QoLSectionRaids".Translate());
            Text.Font = GameFont.Small;
            listing.GapLine();

            listing.CheckboxLabeled(
                "MegaMod_QoLDisableBreachRaids".Translate(),
                ref settings.disableBreachRaids,
                "MegaMod_QoLDisableBreachRaids_Desc".Translate());

            listing.CheckboxLabeled(
                "MegaMod_QoLDisableDropPodRaids".Translate(),
                ref settings.disableDropPodRaids,
                "MegaMod_QoLDisableDropPodRaids_Desc".Translate());

            listing.CheckboxLabeled(
                "MegaMod_QoLNoDeathAcidifiers".Translate(),
                ref settings.noDeathAcidifiers,
                "MegaMod_QoLNoDeathAcidifiers_Desc".Translate());

            listing.CheckboxLabeled(
                "MegaMod_QoLTraderAlerts".Translate(),
                ref settings.traderAlerts,
                "MegaMod_QoLTraderAlerts_Desc".Translate());

            listing.End();
            Widgets.EndScrollView();
        }
    }
}
