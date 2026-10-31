using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.WorkManager.Cache
{
    public static class PawnSuitabilityScorer
    {
        public static float CalculateScore(Pawn pawn, WorkTypeDef workType)
        {
            if (pawn == null || pawn.Dead || workType == null)
            {
                return -9999f;
            }

            // Incapable check
            if (pawn.WorkTypeIsDisabled(workType) || pawn.WorkTagIsDisabled(workType.workTags))
            {
                return -9999f;
            }

            var skills = pawn.skills;
            if (skills == null)
            {
                return -9999f;
            }

            // Base score: Highest relevant skill level * 10f
            float score = 0f;
            var relSkills = workType.relevantSkills;
            if (relSkills != null && relSkills.Count > 0)
            {
                int maxSkill = 0;
                for (int i = 0; i < relSkills.Count; i++)
                {
                    var skillRecord = skills.GetSkill(relSkills[i]);
                    if (skillRecord != null && skillRecord.Level > maxSkill)
                    {
                        maxSkill = skillRecord.Level;
                    }
                }
                score = maxSkill * 10f;
            }
            else
            {
                // Skill-less work types (Hauling, Cleaning, Firefighting, Basic)
                score = 50f;
            }

            // Passion bonus: Major passion +30f, Minor passion +15f
            if (WorkManagerModule.ModSettings.UsePassionPriorities)
            {
                Passion passion = skills.MaxPassionOfRelevantSkillsFor(workType);
                if (passion == Passion.Major)
                {
                    score += 30f;
                }
                else if (passion == Passion.Minor)
                {
                    score += 15f;
                }
            }

            // Health capacity multipliers (Consciousness, Manipulation, Moving, Sight)
            var capacities = pawn.health?.capacities;
            if (capacities != null)
            {
                float consciousness = capacities.GetLevel(PawnCapacityDefOf.Consciousness);
                float manipulation = capacities.GetLevel(PawnCapacityDefOf.Manipulation);
                float moving = capacities.GetLevel(PawnCapacityDefOf.Moving);
                float sight = capacities.GetLevel(PawnCapacityDefOf.Sight);

                score *= Mathf.Clamp(consciousness, 0.1f, 1.5f);

                string defName = workType.defName;
                if (defName == "Doctor" || defName == "Cooking" || defName == "Crafting" ||
                    defName == "Construction" || defName == "Art" || defName == "Smithing" ||
                    defName == "Tailoring" || defName == "Growing" || defName == "PlantCutting")
                {
                    score *= Mathf.Clamp(manipulation, 0.2f, 1.4f);
                }

                if (defName == "Hauling" || defName == "Cleaning" || defName == "Hunting" || defName == "Firefighter")
                {
                    score *= Mathf.Clamp(moving, 0.2f, 1.4f);
                }

                if (defName == "Doctor" || defName == "Hunting" || defName == "Research")
                {
                    score *= Mathf.Clamp(sight, 0.2f, 1.4f);
                }
            }

            // Trait adjustments
            var traitList = pawn.story?.traits?.allTraits;
            if (traitList != null)
            {
                string defName = workType.defName;
                for (int i = 0; i < traitList.Count; i++)
                {
                    var trait = traitList[i];
                    string tDef = trait.def.defName;

                    // Brawler penalized for Hunting
                    if (defName == "Hunting" && tDef == "Brawler")
                    {
                        score -= 100f;
                    }

                    // Nimble bonus for Hauling and Cleaning
                    if ((defName == "Hauling" || defName == "Cleaning") && tDef == "Nimble")
                    {
                        score += 15f;
                    }

                    // TooSmart / FastLearner / GreatMemory bonus for Research
                    if (defName == "Research" && (tDef == "TooSmart" || tDef == "FastLearner" || tDef == "GreatMemory"))
                    {
                        score += 20f;
                    }

                    // Gourmand bonus for Cooking
                    if (defName == "Cooking" && tDef == "Gourmand")
                    {
                        score += 15f;
                    }

                    // Industriousness trait impact on overall work
                    if (tDef == "Industriousness")
                    {
                        score += trait.Degree * 8f;
                    }
                }
            }

            // Ideology penalties if work is considered dangerous
            if (ModsConfig.IdeologyActive && pawn.Ideo != null)
            {
                if (pawn.Ideo.IsWorkTypeConsideredDangerous(workType))
                {
                    score -= 50f;
                }
            }

            return score;
        }
    }
}
