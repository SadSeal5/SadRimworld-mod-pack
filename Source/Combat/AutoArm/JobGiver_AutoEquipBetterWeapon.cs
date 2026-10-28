using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.Combat.AutoArm
{
    public class JobGiver_AutoEquipBetterWeapon : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn == null || !pawn.Spawned || pawn.Dead || pawn.Downed) return null;
            if (pawn.Faction != Faction.OfPlayer || !pawn.RaceProps.Humanlike) return null;
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
                comp.nextCheckTick = currentTick + Rand.Range(2000, 3500);
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
            else
            {
                bool prefersMelee = (pawn.story?.traits?.HasTrait(TraitDefOf.Brawler) == true)
                    || ((pawn.skills?.GetSkill(SkillDefOf.Melee)?.Level ?? 0) > (pawn.skills?.GetSkill(SkillDefOf.Shooting)?.Level ?? 0) + 2);

                targetCategory = prefersMelee ? WeaponCategory.MeleeSharp : WeaponCategory.RangedBurst;
            }

            if (targetCategory == WeaponCategory.None) return null;

            float currentScore = 0f;
            if (pawn.equipment?.Primary != null)
            {
                if (WeaponClassifier.GetCategory(pawn.equipment.Primary) == targetCategory)
                {
                    currentScore = WeaponEvaluator.Evaluate(pawn.equipment.Primary, pawn);
                }
            }

            Map map = pawn.Map;
            if (map == null) return null;

            List<Thing> weapons = map.listerThings.ThingsInGroup(ThingRequestGroup.Weapon);
            if (weapons == null || weapons.Count == 0) return null;

            Thing bestCandidate = null;
            float bestScore = currentScore;
            float minImprovementFactor = pawn.equipment?.Primary != null ? 1.15f : 1.0f;

            for (int i = 0; i < weapons.Count; i++)
            {
                Thing w = weapons[i];
                if (w == null || !w.Spawned || w.Position.Fogged(map)) continue;
                if (ForbidUtility.IsForbidden(w, pawn)) continue;
                if (w.IsBurning()) continue;
                if (!ForbidUtility.InAllowedArea(w.Position, pawn)) continue;

                if (WeaponClassifier.GetCategory(w) != targetCategory) continue;

                float candScore = WeaponEvaluator.Evaluate(w, pawn);
                if (candScore <= 0f) continue;
                if (candScore < bestScore * minImprovementFactor) continue;

                if (!pawn.CanReach(w, PathEndMode.ClosestTouch, Danger.Some)) continue;
                if (!pawn.CanReserve(w, 1, -1, null, false)) continue;

                float dist = pawn.Position.DistanceTo(w.Position);
                float adjustedScore = candScore / (1f + dist * 0.003f);

                if (adjustedScore > bestScore * minImprovementFactor)
                {
                    bestCandidate = w;
                    bestScore = candScore;
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
