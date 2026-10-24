using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using SadSeal.Common.UI;

namespace SadSeal.MegaMod.Skills.UI
{
    public static class SkillSettingsDrawer
    {
        private static Vector2 scrollPosition = Vector2.zero;
        private static readonly Dictionary<string, string> textBuffers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static string globalBuffer = null;

        public static void ClearBuffers()
        {
            textBuffers.Clear();
            globalBuffer = null;
        }

        public static void DrawSettings(Rect inRect, SkillSettings settings)
        {
            var skills = DefDatabase<SkillDef>.AllDefsListForReading;
            int skillCount = skills != null ? skills.Count : 0;
            float totalHeight = 415f + (skillCount * 38f);

            Rect outRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            Rect viewRect = new Rect(0f, 0f, inRect.width - 24f, totalHeight);

            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
            var listing = new Listing_Standard();
            listing.Begin(viewRect);

            // General & QoL Section
            Text.Font = GameFont.Medium;
            listing.Label("SLM_GeneralSettings".Translate());
            Text.Font = GameFont.Small;
            listing.Gap(4f);

            listing.CheckboxLabeled(
                "SLM_OnlyColonists".Translate(),
                ref settings.onlyColonists,
                "SLM_OnlyColonists_Desc".Translate()
            );

            listing.CheckboxLabeled(
                "SLM_DisableDailyCap".Translate(),
                ref settings.disableDailyCap,
                "SLM_DisableDailyCap_Desc".Translate()
            );

            listing.CheckboxLabeled(
                "SLM_DisableSkillDecay".Translate(),
                ref settings.disableSkillDecay,
                "SLM_DisableSkillDecay_Desc".Translate()
            );

            listing.CheckboxLabeled(
                "SLM_MuteMasterworkLegendary".Translate(),
                ref settings.muteMasterworkLegendaryNotifications,
                "SLM_MuteMasterworkLegendary_Desc".Translate()
            );

            listing.CheckboxLabeled(
                "SLM_UseGlobalMultiplier".Translate(),
                ref settings.useGlobalMultiplier,
                "SLM_UseGlobalMultiplier_Desc".Translate()
            );

            listing.Gap(10f);

            // Preset Buttons Section
            Text.Font = GameFont.Medium;
            listing.Label("SLM_Presets".Translate());
            Text.Font = GameFont.Small;
            listing.Gap(4f);

            SadUI.DrawPresetRow(listing, settings.activePreset, new (string, SkillPreset)[]
            {
                ("1x", SkillPreset.X1),
                ("2x", SkillPreset.X2),
                ("5x", SkillPreset.X5),
                ("10x", SkillPreset.X10),
                ("25x", SkillPreset.X25),
                ("50x", SkillPreset.X50),
                ("100x", SkillPreset.X100)
            }, preset =>
            {
                settings.ApplyPreset(preset);
                ClearBuffers();
            }, () =>
            {
                settings.ResetToDefaults();
                ClearBuffers();
            }, "SLM_ResetDefaults", height: 30f, spacing: 6f);

            listing.Gap(14f);

            // Global Multiplier Row
            Text.Font = GameFont.Medium;
            listing.Label("SLM_GlobalMultiplier".Translate());
            Text.Font = GameFont.Small;
            listing.Gap(4f);

            Rect globalRowRect = listing.GetRect(32f);
            SadUI.DrawMultiplierRow(globalRowRect, "SLM_GlobalMultiplier".Translate(), settings.globalMultiplier, ref globalBuffer, newVal =>
            {
                settings.globalMultiplier = newVal;
                settings.activePreset = SkillPreset.Custom;
            }, labelWidth: 180f);

            listing.Gap(14f);

            // Individual Multipliers Section
            Text.Font = GameFont.Medium;
            listing.Label("SLM_IndividualMultipliers".Translate());
            Text.Font = GameFont.Small;

            if (settings.useGlobalMultiplier)
            {
                GUI.color = Color.yellow;
                listing.Label("SLM_IndividualNotice".Translate());
                GUI.color = Color.white;
            }

            listing.Gap(6f);

            if (skills != null)
            {
                var sortedSkills = skills.OrderBy(s => s.listOrder).ToList();
                foreach (SkillDef skill in sortedSkills)
                {
                    if (skill == null) continue;

                    string key = skill.defName;
                    float currentVal = settings.GetSkillMultiplier(key);
                    string buffer = textBuffers.TryGetValue(key, out string b) ? b : null;

                    Rect rowRect = listing.GetRect(32f);
                    string label = !string.IsNullOrEmpty(skill.label) ? skill.LabelCap.Resolve() : skill.defName;

                    SadUI.DrawMultiplierRow(rowRect, label, currentVal, ref buffer, newVal =>
                    {
                        settings.SetSkillMultiplier(key, newVal);
                        settings.activePreset = SkillPreset.Custom;
                    }, labelWidth: 180f);

                    textBuffers[key] = buffer;
                    listing.Gap(4f);
                }
            }

            listing.End();
            Widgets.EndScrollView();
        }
    }
}
