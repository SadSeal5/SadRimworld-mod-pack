using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Incubation.Core;

public static class GrowthPassionScorer
{
	private static readonly SkillDef[] ColonySpecialistSkills =
	{
		SkillDefOf.Medicine,
		SkillDefOf.Cooking,
		SkillDefOf.Construction,
		SkillDefOf.Crafting,
		SkillDefOf.Plants,
		SkillDefOf.Intellectual
	};

	public static List<SkillDef> SelectBestPassions(
		Pawn pawn,
		List<SkillDef> passionChoices,
		int gainsCount,
		Trait chosenTrait,
		PassionStrategy strategy)
	{
		var result = new List<SkillDef>();
		if (pawn == null || passionChoices == null || passionChoices.Count == 0 || gainsCount <= 0)
		{
			return result;
		}

		Dictionary<SkillDef, float> colonyDeficits = ComputeColonyDeficits(pawn);

		var candidates = new List<(SkillDef skill, bool requiredByTrait, float score)>();
		var seen = new HashSet<SkillDef>();

		for (int i = 0; i < passionChoices.Count; i++)
		{
			SkillDef skill = passionChoices[i];
			if (skill == null || !seen.Add(skill))
			{
				continue;
			}

			if (!IsEligibleSkill(pawn, skill, chosenTrait))
			{
				continue;
			}

			bool required = chosenTrait?.def != null && chosenTrait.def.RequiresPassion(skill);
			float score = ScoreSkill(pawn, skill, chosenTrait, strategy, colonyDeficits);
			candidates.Add((skill, required, score));
		}

		candidates.Sort((a, b) =>
		{
			int reqCmp = b.requiredByTrait.CompareTo(a.requiredByTrait);
			if (reqCmp != 0)
			{
				return reqCmp;
			}
			return b.score.CompareTo(a.score);
		});

		int pickCount = Math.Min(gainsCount, candidates.Count);
		for (int i = 0; i < pickCount; i++)
		{
			result.Add(candidates[i].skill);
		}

		return result;
	}

	private static bool IsEligibleSkill(Pawn pawn, SkillDef skill, Trait chosenTrait)
	{
		SkillRecord record = pawn.skills?.GetSkill(skill);
		if (record == null || record.PermanentlyDisabled || record.TotallyDisabled || record.passion == Passion.Major)
		{
			return false;
		}

		if (pawn.WorkTagIsDisabled(skill.disablingWorkTags))
		{
			return false;
		}

		if (chosenTrait?.def != null)
		{
			if ((chosenTrait.def.disabledWorkTags & skill.disablingWorkTags) != WorkTags.None)
			{
				return false;
			}
			if (chosenTrait.def.ConflictsWithPassion(skill))
			{
				return false;
			}
			if (IsSkillDisabledByTraitWorkTypes(skill, chosenTrait.def))
			{
				return false;
			}
		}

		if (HasDropAllPassionGene(pawn, skill))
		{
			return false;
		}

		return true;
	}

	private static bool IsSkillDisabledByTraitWorkTypes(SkillDef skill, TraitDef traitDef)
	{
		if (traitDef.disabledWorkTypes == null || traitDef.disabledWorkTypes.Count == 0)
		{
			return false;
		}

		List<WorkTypeDef> allWorkTypes = DefDatabase<WorkTypeDef>.AllDefsListForReading;
		bool anyMatchingWorkType = false;
		bool allDisabled = true;

		for (int i = 0; i < allWorkTypes.Count; i++)
		{
			WorkTypeDef wt = allWorkTypes[i];
			if (wt.relevantSkills != null && wt.relevantSkills.Contains(skill))
			{
				anyMatchingWorkType = true;
				if (!traitDef.disabledWorkTypes.Contains(wt))
				{
					allDisabled = false;
					break;
				}
			}
		}

		return anyMatchingWorkType && allDisabled;
	}

	private static bool HasDropAllPassionGene(Pawn pawn, SkillDef skill)
	{
		if (pawn.genes == null)
		{
			return false;
		}

		List<Gene> genes = pawn.genes.GenesListForReading;
		for (int i = 0; i < genes.Count; i++)
		{
			Gene gene = genes[i];
			if (gene.Active && gene.def.passionMod != null && gene.def.passionMod.modType == PassionMod.PassionModType.DropAll && gene.def.passionMod.skill == skill)
			{
				return true;
			}
		}
		return false;
	}

	private static float ScoreSkill(
		Pawn pawn,
		SkillDef skill,
		Trait chosenTrait,
		PassionStrategy strategy,
		Dictionary<SkillDef, float> colonyDeficits)
	{
		float baseUtility = GetBaseUtility(skill);
		float pawnAndTraitSynergy = GetPawnAndTraitSynergy(pawn, skill, chosenTrait);
		float geneAptitudeSynergy = GetGeneAptitudeSynergy(pawn, skill);
		float colonyDeficitNeed = colonyDeficits.TryGetValue(skill, out float defScore) ? defScore : 0f;
		float upgradeValue = GetUpgradeValue(pawn, skill, strategy);

		float strategyBonus = 0f;
		switch (strategy)
		{
			case PassionStrategy.BalancedColonyNeed:
				return baseUtility + pawnAndTraitSynergy + geneAptitudeSynergy + (colonyDeficitNeed * 1.35f) + upgradeValue;

			case PassionStrategy.CombatFocused:
				if (skill == SkillDefOf.Shooting || skill == SkillDefOf.Melee)
				{
					strategyBonus += 55f;
				}
				else if (skill == SkillDefOf.Medicine)
				{
					strategyBonus += 15f;
				}
				return baseUtility + (pawnAndTraitSynergy * 1.25f) + geneAptitudeSynergy + (colonyDeficitNeed * 0.5f) + upgradeValue + strategyBonus;

			case PassionStrategy.ExistingSynergy:
				return (baseUtility * 0.8f) + (pawnAndTraitSynergy * 1.8f) + (geneAptitudeSynergy * 1.5f) + (colonyDeficitNeed * 0.6f) + upgradeValue;

			case PassionStrategy.BroadCoverage:
				return (baseUtility * 1.1f) + (pawnAndTraitSynergy * 0.7f) + geneAptitudeSynergy + (colonyDeficitNeed * 1.3f) + upgradeValue;

			default:
				return baseUtility + pawnAndTraitSynergy + geneAptitudeSynergy + colonyDeficitNeed + upgradeValue;
		}
	}

	private static float GetBaseUtility(SkillDef skill)
	{
		if (skill == SkillDefOf.Shooting) return 24f;
		if (skill == SkillDefOf.Medicine) return 22f;
		if (skill == SkillDefOf.Construction) return 20f;
		if (skill == SkillDefOf.Crafting) return 20f;
		if (skill == SkillDefOf.Plants) return 18f;
		if (skill == SkillDefOf.Cooking) return 18f;
		if (skill == SkillDefOf.Intellectual) return 17f;
		if (skill == SkillDefOf.Melee) return 16f;
		if (skill == SkillDefOf.Social) return 14f;
		if (skill == SkillDefOf.Mining) return 12f;
		if (skill == SkillDefOf.Animals) return 11f;
		if (skill == SkillDefOf.Artistic) return 8f;
		return 10f;
	}

	private static float GetPawnAndTraitSynergy(Pawn pawn, SkillDef skill, Trait chosenTrait)
	{
		float synergy = 0f;
		SkillRecord record = pawn.skills?.GetSkill(skill);
		if (record != null)
		{
			synergy += Mathf.Min(record.Level * 1.6f, 25f);
		}

		// Trait synergies across existing traits and the newly selected trait
		synergy += ScoreTraitListSynergy(pawn.story?.traits?.allTraits, skill);
		if (chosenTrait != null)
		{
			synergy += ScoreSingleTraitSynergy(chosenTrait, skill) * 1.25f;
		}

		return synergy;
	}

	private static float ScoreTraitListSynergy(List<Trait> traits, SkillDef skill)
	{
		if (traits == null)
		{
			return 0f;
		}
		float sum = 0f;
		for (int i = 0; i < traits.Count; i++)
		{
			sum += ScoreSingleTraitSynergy(traits[i], skill);
		}
		return sum;
	}

	private static float ScoreSingleTraitSynergy(Trait trait, SkillDef skill)
	{
		if (trait?.def == null)
		{
			return 0f;
		}

		float s = 0f;
		string defName = trait.def.defName;

		if (defName == "Brawler")
		{
			if (skill == SkillDefOf.Melee) s += 35f;
			else if (skill == SkillDefOf.Shooting) s -= 40f;
		}
		else if (defName == "TriggerHappy" || defName == "ShootingAccuracy")
		{
			if (skill == SkillDefOf.Shooting) s += 28f;
		}
		else if (defName == "Nimble")
		{
			if (skill == SkillDefOf.Melee) s += 22f;
		}
		else if (defName == "TorturedArtist")
		{
			if (skill == SkillDefOf.Artistic || skill == SkillDefOf.Crafting) s += 15f;
		}
		else if (defName == "Kind")
		{
			if (skill == SkillDefOf.Social) s += 12f;
		}

		TraitDegreeData data = trait.CurrentData;
		if (data != null)
		{
			if (data.skillGains != null)
			{
				foreach (SkillGain gain in data.skillGains)
				{
					if (gain != null && gain.skill == skill)
					{
						s += gain.amount * 4f;
					}
				}
			}

			if (data.aptitudes != null)
			{
				for (int i = 0; i < data.aptitudes.Count; i++)
				{
					Aptitude apt = data.aptitudes[i];
					if (apt != null && apt.skill == skill)
					{
						s += apt.level * 5f;
					}
				}
			}
		}

		return s;
	}

	private static float GetGeneAptitudeSynergy(Pawn pawn, SkillDef skill)
	{
		if (pawn.genes == null)
		{
			return 0f;
		}

		float total = 0f;
		List<Gene> genes = pawn.genes.GenesListForReading;
		for (int i = 0; i < genes.Count; i++)
		{
			Gene gene = genes[i];
			if (!gene.Active)
			{
				continue;
			}

			if (gene.def.aptitudes != null)
			{
				for (int j = 0; j < gene.def.aptitudes.Count; j++)
				{
					Aptitude apt = gene.def.aptitudes[j];
					if (apt != null && apt.skill == skill)
					{
						total += apt.level * 7f;
					}
				}
			}

			if (gene.def.passionMod != null && gene.def.passionMod.skill == skill)
			{
				if (gene.def.passionMod.modType == PassionMod.PassionModType.AddOneLevel)
				{
					total += 10f;
				}
				else if (gene.def.passionMod.modType == PassionMod.PassionModType.DropAll)
				{
					total -= 100f;
				}
			}
		}

		return total;
	}

	private static float GetUpgradeValue(Pawn pawn, SkillDef skill, PassionStrategy strategy)
	{
		SkillRecord record = pawn.skills?.GetSkill(skill);
		if (record == null)
		{
			return 0f;
		}

		if (strategy == PassionStrategy.BroadCoverage)
		{
			return record.passion == Passion.None ? 24f : -8f;
		}

		if (strategy == PassionStrategy.ExistingSynergy)
		{
			return record.passion == Passion.Minor ? 24f : 6f;
		}

		return record.passion == Passion.Minor ? 14f : 10f;
	}

	private static Dictionary<SkillDef, float> ComputeColonyDeficits(Pawn subjectPawn)
	{
		var deficits = new Dictionary<SkillDef, float>();
		List<Pawn> colonists = GetActiveColonists(subjectPawn);
		int colonistCount = colonists.Count;
		if (colonistCount == 0)
		{
			return deficits;
		}

		double targetPerSpecialist = Math.Max(1.0, Math.Ceiling(colonistCount / 5.0));

		for (int s = 0; s < ColonySpecialistSkills.Length; s++)
		{
			SkillDef skill = ColonySpecialistSkills[s];
			int specialists = 0;

			for (int p = 0; p < colonistCount; p++)
			{
				Pawn colonist = colonists[p];
				if (colonist == subjectPawn || colonist.skills == null)
				{
					continue;
				}

				SkillRecord rec = colonist.skills.GetSkill(skill);
				if (rec != null && !rec.PermanentlyDisabled && !rec.TotallyDisabled && (rec.passion >= Passion.Minor || rec.Level >= 8))
				{
					specialists++;
				}
			}

			if (specialists < targetPerSpecialist)
			{
				float ratio = (float)((targetPerSpecialist - specialists) / targetPerSpecialist);
				float bonus = ratio * 35f + (specialists == 0 ? 20f : 0f);
				deficits[skill] = bonus;
			}
		}

		return deficits;
	}

	private static List<Pawn> GetActiveColonists(Pawn pawn)
	{
		var result = new List<Pawn>();
		Map map = pawn?.MapHeld;
		if (map?.mapPawns != null)
		{
			List<Pawn> mapColonists = map.mapPawns.FreeColonistsSpawned;
			for (int i = 0; i < mapColonists.Count; i++)
			{
				Pawn p = mapColonists[i];
				if (p != null && !p.Dead && !p.Downed && p.Faction == Faction.OfPlayer)
				{
					result.Add(p);
				}
			}
			if (result.Count > 0)
			{
				return result;
			}
		}

		List<Pawn> allColonists = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists;
		if (allColonists != null)
		{
			for (int i = 0; i < allColonists.Count; i++)
			{
				Pawn p = allColonists[i];
				if (p != null && !p.Dead && p.Faction == Faction.OfPlayer)
				{
					result.Add(p);
				}
			}
		}

		return result;
	}
}
