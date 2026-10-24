using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Skills
{
    public enum SkillPreset
    {
        Custom,
        X1,
        X2,
        X5,
        X10,
        X25,
        X50,
        X100
    }

    public class SkillSettings : IExposable
    {
        public bool onlyColonists = true;
        public bool useGlobalMultiplier = true;
        public float globalMultiplier = 1.0f;
        public bool disableDailyCap = false;
        public bool disableSkillDecay = false;
        public bool muteMasterworkLegendaryNotifications = false;
        public SkillPreset activePreset = SkillPreset.X1;

        public Dictionary<string, float> skillMultipliers = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);

        public bool ShouldApplyTo(Pawn pawn)
        {
            if (!onlyColonists) return true;
            if (pawn == null) return false;
            return pawn.IsColonist || (pawn.Faction != null && pawn.Faction.IsPlayer);
        }

        public float GetMultiplierForSkill(SkillDef def)
        {
            if (useGlobalMultiplier)
            {
                return globalMultiplier;
            }

            if (def != null && skillMultipliers != null && skillMultipliers.TryGetValue(def.defName, out float mult))
            {
                return mult;
            }

            return 1.0f;
        }

        public float GetSkillMultiplier(string defName)
        {
            if (skillMultipliers != null && skillMultipliers.TryGetValue(defName, out float mult))
            {
                return mult;
            }
            return 1.0f;
        }

        public void SetSkillMultiplier(string defName, float value)
        {
            if (skillMultipliers == null)
            {
                skillMultipliers = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
            }
            skillMultipliers[defName] = Mathf.Clamp(value, 0f, 100f);
        }

        public void ResetToDefaults()
        {
            onlyColonists = true;
            useGlobalMultiplier = true;
            globalMultiplier = 1.0f;
            disableDailyCap = false;
            disableSkillDecay = false;
            muteMasterworkLegendaryNotifications = false;
            activePreset = SkillPreset.X1;
            if (skillMultipliers == null)
            {
                skillMultipliers = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                skillMultipliers.Clear();
            }
        }

        public void ApplyPreset(SkillPreset preset)
        {
            activePreset = preset;
            switch (preset)
            {
                case SkillPreset.X1: SetAllMultipliers(1f); break;
                case SkillPreset.X2: SetAllMultipliers(2f); break;
                case SkillPreset.X5: SetAllMultipliers(5f); break;
                case SkillPreset.X10: SetAllMultipliers(10f); break;
                case SkillPreset.X25: SetAllMultipliers(25f); break;
                case SkillPreset.X50: SetAllMultipliers(50f); break;
                case SkillPreset.X100: SetAllMultipliers(100f); break;
            }
        }

        public void SetAllMultipliers(float value)
        {
            float clamped = Mathf.Clamp(value, 0f, 100f);
            globalMultiplier = clamped;
            if (skillMultipliers == null)
            {
                skillMultipliers = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
            }

            if (DefDatabase<SkillDef>.AllDefsListForReading != null)
            {
                foreach (SkillDef def in DefDatabase<SkillDef>.AllDefsListForReading)
                {
                    if (def != null && !string.IsNullOrEmpty(def.defName))
                    {
                        skillMultipliers[def.defName] = clamped;
                    }
                }
            }
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref onlyColonists, "onlyColonists", true);
            Scribe_Values.Look(ref useGlobalMultiplier, "useGlobalMultiplier", true);
            Scribe_Values.Look(ref globalMultiplier, "globalMultiplier", 1.0f);
            Scribe_Values.Look(ref disableDailyCap, "disableDailyCap", false);
            Scribe_Values.Look(ref disableSkillDecay, "disableSkillDecay", false);
            Scribe_Values.Look(ref muteMasterworkLegendaryNotifications, "muteMasterworkLegendaryNotifications", false);
            Scribe_Values.Look(ref activePreset, "activePreset", SkillPreset.X1);
            Scribe_Collections.Look(ref skillMultipliers, "skillMultipliers", LookMode.Value, LookMode.Value);

            if (skillMultipliers == null)
            {
                skillMultipliers = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
