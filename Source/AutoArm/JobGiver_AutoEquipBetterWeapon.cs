using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.AutoArm
{
    public class JobGiver_AutoEquipBetterWeapon : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (MegaMod.Settings?.Modules != null && !MegaMod.Settings.Modules.enableAutoArm) return null;

            AutoArmSettings settings = MegaMod.Settings?.AutoArm;
            if (settings != null && !settings.enableAutoArm) return null;
            if (settings == null) settings = new AutoArmSettings();

            if (pawn == null || !pawn.Spawned || pawn.Dead || pawn.Downed) return null;
            if (pawn.Faction != Faction.OfPlayer || !pawn.RaceProps.Humanlike || !pawn.IsColonist) return null;
            if (pawn.Drafted || pawn.InMentalState) return null;
            if (pawn.IsBurning() || (pawn.health?.hediffSet?.BleedRateTotal ?? 0f) > 0.05f) return null;
            if (pawn.mindState?.enemyTarget != null) return null;
            if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation)) return null;
            if (pawn.WorkTagIsDisabled(WorkTags.Violent)) return null;

            CompAutoArm comp = pawn.TryGetComp<CompAutoArm>();
            if (comp != null && !comp.enabled) return null;

            int currentTick = Find.TickManager.TicksGame;
            if (comp != null)
            {
                if (currentTick < comp.nextCheckTick) return null;
                int intervalTicks = Mathf.Max(600, settings.checkIntervalSeconds * 60);
                comp.nextCheckTick = currentTick + intervalTicks + Rand.Range(-60, 60);
            }

            WeaponCategory targetCategory = WeaponCategory.None;
            if (comp != null && comp.isCategoryLocked && comp.preferredCategory != WeaponCategory.None)
            {
                targetCategory = comp.preferredCategory;
            }
            else if (pawn.equipment?.Primary != null)
            {
                targetCategory = WeaponClassifier.GetCategory(pawn.equipment.Primary);
            }

            if (targetCategory == WeaponCategory.None && settings.autoAssignBySkill)
            {
                bool prefersMelee = (pawn.story?.traits?.HasTrait(TraitDefOf.Brawler) == true)
                    || ((pawn.skills?.GetSkill(SkillDefOf.Melee)?.Level ?? 0) > (pawn.skills?.GetSkill(SkillDefOf.Shooting)?.Level ?? 0) + 2);

                if (prefersMelee)
                {
                    targetCategory = WeaponCategory.MeleeSharp;
                }
                else if ((pawn.skills?.GetSkill(SkillDefOf.Shooting)?.Level ?? 0) >= 10)
                {
                    targetCategory = WeaponCategory.RangedSingleShot;
                }
                else
                {
                    targetCategory = WeaponCategory.RangedBurst;
                }
            }
            else if (targetCategory == WeaponCategory.None)
            {
                targetCategory = WeaponCategory.RangedBurst;
            }

            float currentScore = 0f;
            if (pawn.equipment?.Primary != null)
            {
                if (WeaponClassifier.GetCategory(pawn.equipment.Primary) == targetCategory)
                {
                    currentScore = WeaponEvaluator.Evaluate(pawn.equipment.Primary, pawn, settings);
                }
            }

            Map map = pawn.Map;
            if (map == null) return null;

            List<Thing> weapons = map.listerThings.ThingsInGroup(ThingRequestGroup.Weapon);
            if (weapons == null || weapons.Count == 0) return null;

            Thing bestCandidate = null;
            float bestAdjustedScore = currentScore;
            float minImprovementFactor = pawn.equipment?.Primary != null ? (1f + settings.minScoreDifference) : 1.0f;

            for (int i = 0; i < weapons.Count; i++)
            {
                Thing w = weapons[i];
                if (w == null || !w.Spawned || w.Position.Fogged(map)) continue;
                if (ForbidUtility.IsForbidden(w, pawn)) continue;
                if (w.IsBurning()) continue;
                if (!ForbidUtility.InAllowedArea(w.Position, pawn)) continue;

                if (settings.onlyFromStockpile)
                {
                    bool inStorage = w.IsInValidStorage() || w.Position.GetSlotGroup(map) != null || w.ParentHolder is Building_Storage;
                    if (!inStorage) continue;
                }

                if (WeaponClassifier.GetCategory(w) != targetCategory) continue;

                float candScore = WeaponEvaluator.Evaluate(w, pawn, settings);
                if (candScore <= 0f) continue;
                if (candScore < currentScore * minImprovementFactor) continue;

                if (!pawn.CanReach(w, PathEndMode.ClosestTouch, Danger.Some)) continue;
                if (!pawn.CanReserve(w, 1, -1, null, false)) continue;

                float dist = pawn.Position.DistanceTo(w.Position);
                float adjustedScore = candScore / (1f + dist * 0.002f);

                if (adjustedScore > bestAdjustedScore && candScore > currentScore * minImprovementFactor)
                {
                    bestCandidate = w;
                    bestAdjustedScore = adjustedScore;
                }
            }

            if (bestCandidate != null)
            {
                return JobMaker.MakeJob(JobDefOf.Equip, bestCandidate);
            }

            return null;
        }
    }
}
