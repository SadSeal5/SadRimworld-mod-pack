using System;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Combat.UI
{
    public static class CombatSettingsDrawer
    {
        private static Vector2 scrollPosition = Vector2.zero;

        public static void DrawSettings(Rect inRect, CombatSettings settings)
        {
            Rect viewRect = new Rect(0f, 0f, inRect.width - 24f, 780f);
            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            Text.Font = GameFont.Medium;
            listing.Label("LethalRanged_SettingsTitle".Translate());
            Text.Font = GameFont.Small;
            listing.Gap(8f);

            // Preset Buttons
            listing.Label("LethalRanged_PresetHeader".Translate());
            Rect presetRect = listing.GetRect(32f);
            float btnWidth = (presetRect.width - 15f) / 4f;

            Color oldColor = GUI.color;
            if (settings.activePreset == LethalityPreset.VanillaPlus) GUI.color = Color.green;
            if (Widgets.ButtonText(new Rect(presetRect.x, presetRect.y, btnWidth, 32f), "LethalRanged_PresetVanillaPlus".Translate()))
            {
                settings.ApplyPreset(LethalityPreset.VanillaPlus);
            }
            GUI.color = oldColor;

            if (settings.activePreset == LethalityPreset.TacticalLethal) GUI.color = Color.green;
            if (Widgets.ButtonText(new Rect(presetRect.x + btnWidth + 5f, presetRect.y, btnWidth, 32f), "LethalRanged_PresetTactical".Translate()))
            {
                settings.ApplyPreset(LethalityPreset.TacticalLethal);
            }
            GUI.color = oldColor;

            if (settings.activePreset == LethalityPreset.HarshRealism) GUI.color = Color.green;
            if (Widgets.ButtonText(new Rect(presetRect.x + (btnWidth + 5f) * 2f, presetRect.y, btnWidth, 32f), "LethalRanged_PresetRealism".Translate()))
            {
                settings.ApplyPreset(LethalityPreset.HarshRealism);
            }
            GUI.color = oldColor;

            if (Widgets.ButtonText(new Rect(presetRect.x + (btnWidth + 5f) * 3f, presetRect.y, btnWidth, 32f), "LethalRanged_ResetDefault".Translate()))
            {
                settings.ApplyPreset(LethalityPreset.TacticalLethal);
            }

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
