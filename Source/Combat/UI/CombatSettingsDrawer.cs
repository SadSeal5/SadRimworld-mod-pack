using System;
using UnityEngine;
using Verse;
using SadSeal.Common.UI;

namespace SadSeal.MegaMod.Combat.UI
{
    public static class CombatSettingsDrawer
    {
        private static Vector2 scrollPosition = Vector2.zero;

        public static void DrawSettings(Rect inRect, CombatSettings settings)
        {
            Rect viewRect = new Rect(0f, 0f, inRect.width - 24f, 920f);
            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            SadUI.DrawHeader(listing, "LethalRanged_SettingsTitle");

            // Preset Buttons
            listing.Label("LethalRanged_PresetHeader".Translate());
            SadUI.DrawPresetRow(listing, settings.activePreset, new (string, LethalityPreset)[]
            {
                ("LethalRanged_PresetVanillaPlus", LethalityPreset.VanillaPlus),
                ("LethalRanged_PresetTactical", LethalityPreset.TacticalLethal),
                ("LethalRanged_PresetRealism", LethalityPreset.HarshRealism)
            }, preset => settings.ApplyPreset(preset), () => settings.ApplyPreset(LethalityPreset.TacticalLethal), "LethalRanged_ResetDefault");

            listing.Gap(14f);

            // Section 1: Accuracy Overhaul
            listing.Label("LethalRanged_SectionAccuracy".Translate());
            bool accEnabled = settings.enableAccuracyOverhaul;
            listing.CheckboxLabeled("LethalRanged_EnableAccuracy".Translate(), ref settings.enableAccuracyOverhaul, "LethalRanged_EnableAccuracy_Desc".Translate());
            if (accEnabled != settings.enableAccuracyOverhaul) settings.activePreset = LethalityPreset.Custom;

            if (settings.enableAccuracyOverhaul)
            {
                Rect formulaRect = listing.GetRect(30f);
                Widgets.Label(new Rect(formulaRect.x, formulaRect.y + 4f, 200f, 24f), "LethalRanged_FormulaMode".Translate());
                if (Widgets.ButtonText(new Rect(formulaRect.x + 210f, formulaRect.y, 240f, 30f), ("LethalRanged_Mode_" + settings.accuracyMode.ToString()).Translate()))
                {
                    int next = ((int)settings.accuracyMode + 1) % 4;
                    settings.accuracyMode = (AccuracyFormulaMode)next;
                    settings.activePreset = LethalityPreset.Custom;
                }

                float oldFalloff = settings.distanceFalloffMultiplier;
                settings.distanceFalloffMultiplier = listing.SliderLabeled(
                    "LethalRanged_FalloffMult".Translate() + $": {settings.distanceFalloffMultiplier:P0}",
                    settings.distanceFalloffMultiplier, 0.50f, 2.00f, 0.5f,
                    "LethalRanged_FalloffMult_Desc".Translate());
                if (Math.Abs(oldFalloff - settings.distanceFalloffMultiplier) > 0.005f) settings.activePreset = LethalityPreset.Custom;
            }

            listing.Gap(12f);

            // Section 2: Lethality & Damage
            listing.Label("LethalRanged_SectionLethality".Translate());

            float oldDmg = settings.damageMultiplier;
            settings.damageMultiplier = listing.SliderLabeled(
                "LethalRanged_DamageMult".Translate() + $": {settings.damageMultiplier:P0}",
                settings.damageMultiplier, 1.00f, 2.50f, 0.5f,
                "LethalRanged_DamageMult_Desc".Translate());
            if (Math.Abs(oldDmg - settings.damageMultiplier) > 0.005f) settings.activePreset = LethalityPreset.Custom;

            float oldAp = settings.armorPenetrationMultiplier;
            settings.armorPenetrationMultiplier = listing.SliderLabeled(
                "LethalRanged_ApMult".Translate() + $": {settings.armorPenetrationMultiplier:P0}",
                settings.armorPenetrationMultiplier, 1.00f, 2.00f, 0.5f,
                "LethalRanged_ApMult_Desc".Translate());
            if (Math.Abs(oldAp - settings.armorPenetrationMultiplier) > 0.005f) settings.activePreset = LethalityPreset.Custom;

            float oldSpeed = settings.projectileSpeedMultiplier;
            settings.projectileSpeedMultiplier = listing.SliderLabeled(
                "LethalRanged_SpeedMult".Translate() + $": {settings.projectileSpeedMultiplier:P0}",
                settings.projectileSpeedMultiplier, 1.00f, 2.50f, 0.5f,
                "LethalRanged_SpeedMult_Desc".Translate());
            if (Math.Abs(oldSpeed - settings.projectileSpeedMultiplier) > 0.005f) settings.activePreset = LethalityPreset.Custom;

            float oldMeleeDmg = settings.meleeDamageMultiplier;
            settings.meleeDamageMultiplier = listing.SliderLabeled(
                "LethalRanged_MeleeDamageMult".Translate() + $": {settings.meleeDamageMultiplier:P0}",
                settings.meleeDamageMultiplier, 0.50f, 3.00f, 0.5f,
                "LethalRanged_MeleeDamageMult_Desc".Translate());
            if (Math.Abs(oldMeleeDmg - settings.meleeDamageMultiplier) > 0.005f) settings.activePreset = LethalityPreset.Custom;

            float oldMeleeAp = settings.meleeArmorPenetrationMultiplier;
            settings.meleeArmorPenetrationMultiplier = listing.SliderLabeled(
                "LethalRanged_MeleeApMult".Translate() + $": {settings.meleeArmorPenetrationMultiplier:P0}",
                settings.meleeArmorPenetrationMultiplier, 0.50f, 3.00f, 0.5f,
                "LethalRanged_MeleeApMult_Desc".Translate());
            if (Math.Abs(oldMeleeAp - settings.meleeArmorPenetrationMultiplier) > 0.005f) settings.activePreset = LethalityPreset.Custom;

            listing.Gap(12f);

            // Section 3: Kinetic Stagger & Fatalities
            listing.Label("LethalRanged_SectionMechanics".Translate());

            bool oldStagger = settings.enableStaggerEnhancement;
            listing.CheckboxLabeled("LethalRanged_EnableStagger".Translate(), ref settings.enableStaggerEnhancement, "LethalRanged_EnableStagger_Desc".Translate());
            if (oldStagger != settings.enableStaggerEnhancement) settings.activePreset = LethalityPreset.Custom;

            if (settings.enableStaggerEnhancement)
            {
                float oldStaggerSpeed = settings.staggerMoveSpeedFactor;
                settings.staggerMoveSpeedFactor = listing.SliderLabeled(
                    "LethalRanged_StaggerSpeedFactor".Translate() + $": {settings.staggerMoveSpeedFactor:P0}",
                    settings.staggerMoveSpeedFactor, 0.10f, 0.50f, 0.5f,
                    "LethalRanged_StaggerSpeedFactor_Desc".Translate());
                if (Math.Abs(oldStaggerSpeed - settings.staggerMoveSpeedFactor) > 0.005f) settings.activePreset = LethalityPreset.Custom;
            }

            bool oldPreserve = settings.preventOneHpPreservation;
            listing.CheckboxLabeled("LethalRanged_Prevent1Hp".Translate(), ref settings.preventOneHpPreservation, "LethalRanged_Prevent1Hp_Desc".Translate());
            if (oldPreserve != settings.preventOneHpPreservation) settings.activePreset = LethalityPreset.Custom;

            bool oldTurrets = settings.affectTurrets;
            listing.CheckboxLabeled("LethalRanged_AffectTurrets".Translate(), ref settings.affectTurrets, "LethalRanged_AffectTurrets_Desc".Translate());
            if (oldTurrets != settings.affectTurrets) settings.activePreset = LethalityPreset.Custom;

            listing.End();
            Widgets.EndScrollView();
        }
    }
}
