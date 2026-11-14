using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Incubation.Core;

public static class GrowthTraitScorer
{
	private static readonly Dictionary<(string defName, int degree), float> CuratedScores = new Dictionary<(string, int), float>
	{
		// S-Tier & A-Tier
		{ ("Tough", 0), 115f },
		{ ("Industriousness", 2), 105f },
		{ ("Industriousness", 1), 70f },
		{ ("QuickSleeper", 0), 90f },
		{ ("SpeedOffset", 2), 95f },
		{ ("SpeedOffset", 1), 55f },
		{ ("FastLearner", 0), 100f },
		{ ("NaturalMood", 2), 95f },
		{ ("NaturalMood", 1), 60f },
		{ ("Nerves", 2), 85f },
		{ ("Nerves", 1), 55f },
		{ ("TriggerHappy", 0), 80f },
		{ ("ShootingAccuracy", -1), 65f },
		{ ("GreatMemory", 0), 80f },
		{ ("Nimble", 0), 75f },
		{ ("Immunity", 1), 70f },
		{ ("Brawler", 0), 55f },
		{ ("Kind", 0), 55f },
		{ ("Ascetic", 0), 45f },
		{ ("TooSmart", 0), 45f },
		{ ("BodyModder", 0), 45f },
		{ ("Transhumanist", 0), 45f },
		{ ("NightOwl", 0), 40f },
		{ ("Masochist", 0), 35f },
		{ ("PsychicSensitivity", 1), 25f },
		{ ("PsychicSensitivity", 2), 40f },

		// F-Tier
		{ ("Pyromaniac", 0), -300f },
		{ ("Gourmand", 0), -220f },
		{ ("ChemicalInterest", 1), -130f },
		{ ("ChemicalInterest", 2), -220f },
		{ ("DrugDesire", 1), -130f },
		{ ("DrugDesire", 2), -220f },
		{ ("Wimp", 0), -180f },
		{ ("Delicate", 0), -160f },
		{ ("SlowLearner", 0), -160f },
		{ ("Sickly", 0), -140f },
		{ ("SpeedOffset", -1), -120f },
		{ ("Industriousness", -1), -100f },
		{ ("Industriousness", -2), -180f },
		{ ("NaturalMood", -1), -90f },
		{ ("NaturalMood", -2), -170f },
		{ ("Nerves", -1), -90f },
		{ ("Nerves", -2), -180f },
		{ ("Jealous", 0), -110f },
		{ ("Greedy", 0), -110f },
		{ ("Abrasive", 0), -90f },
		{ ("TorturedArtist", 0), -100f },
		{ ("BodyPurist", 0), -90f },
		{ ("AnnoyingVoice", 0), -75f },
		{ ("CreepyBreathing", 0), -75f },
		{ ("Recluse", 0), -60f }
	};

	public static Trait SelectBestTrait(Pawn pawn, List<Trait> traitChoices, bool noTraitOptionShown, bool avoidNegativeTraits)
	{
		if (traitChoices == null || traitChoices.Count == 0)
		{
			return null;
		}

		Trait bestTrait = null;
		float bestScore = float.MinValue;

		for (int i = 0; i < traitChoices.Count; i++)
		{
			Trait candidate = traitChoices[i];
			if (candidate?.def == null)
			{
				continue;
			}

			float score = ScoreTrait(pawn, candidate);
			if (score > bestScore)
			{
				bestScore = score;
				bestTrait = candidate;
			}
		}

		if (bestTrait == null)
		{
			return noTraitOptionShown ? ChoiceLetter_GrowthMoment.NoTrait : null;
		}

		if (bestScore < 0f && noTraitOptionShown && avoidNegativeTraits)
		{
			return ChoiceLetter_GrowthMoment.NoTrait;
		}

		return bestTrait;
	}

	public static float ScoreTrait(Pawn pawn, Trait trait)
	{
		if (trait?.def == null)
		{
			return -1000f;
		}

		TraitDef def = trait.def;
		TraitDegreeData data = trait.CurrentData;
		float score = 0f;

		bool hasCuratedScore = CuratedScores.TryGetValue((def.defName, trait.Degree), out float curatedScore);
		if (hasCuratedScore)
		{
			score += curatedScore;
		}

		score += EvaluateContextualAdjustments(pawn, def, trait.Degree);
		score += EvaluateDynamicData(pawn, def, data, hasCuratedScore ? 0.25f : 1f);

		return score;
	}

	private static float EvaluateContextualAdjustments(Pawn pawn, TraitDef def, int degree)
	{
		if (pawn == null)
		{
			return 0f;
		}

		float adj = 0f;

		if (def.defName == "Brawler")
		{
			SkillRecord melee = pawn.skills?.GetSkill(SkillDefOf.Melee);
			SkillRecord shooting = pawn.skills?.GetSkill(SkillDefOf.Shooting);

			if (melee != null && !melee.PermanentlyDisabled)
			{
				if (melee.passion == Passion.Major)
				{
					adj += 35f;
				}
				else if (melee.passion == Passion.Minor)
				{
					adj += 20f;
				}
			}

			if (pawn.genes != null)
			{
				int meleeAptitude = GetGeneAptitudeForSkill(pawn, SkillDefOf.Melee);
				if (meleeAptitude > 0)
				{
					adj += meleeAptitude * 6f;
				}
			}

			if (shooting != null)
			{
				if (shooting.passion == Passion.Major)
				{
					adj -= 60f;
				}
				else if (shooting.passion == Passion.Minor)
				{
					adj -= 30f;
				}
			}
		}
		else if (def.defName == "PsychicSensitivity" && degree > 0)
		{
			if (ModsConfig.RoyaltyActive)
			{
				adj += 20f;
			}
			if (pawn.HasPsylink)
			{
				adj += 20f;
			}
		}

		return adj;
	}

	private static float EvaluateDynamicData(Pawn pawn, TraitDef def, TraitDegreeData data, float weight)
	{
		float dynamic = 0f;

		if (def.disabledWorkTags != WorkTags.None)
		{
			dynamic -= CountSetBits((int)def.disabledWorkTags) * 20f;
		}

		if (def.disabledWorkTypes != null && def.disabledWorkTypes.Count > 0)
		{
			dynamic -= def.disabledWorkTypes.Count * 15f;
		}

		if (def.conflictingPassions != null && pawn?.skills != null)
		{
			for (int i = 0; i < def.conflictingPassions.Count; i++)
			{
				SkillDef conflictSkill = def.conflictingPassions[i];
				SkillRecord rec = pawn.skills.GetSkill(conflictSkill);
				if (rec != null)
				{
					if (rec.passion == Passion.Major)
					{
						dynamic -= 50f;
					}
					else if (rec.passion == Passion.Minor)
					{
						dynamic -= 25f;
					}
				}
			}
		}

		if (data != null)
		{
			if (data.forcedMentalState != null)
			{
				dynamic -= 120f;
			}

			if (data.randomMentalState != null)
			{
				dynamic -= 85f;
			}

			if (data.hungerRateFactor > 1f)
			{
				dynamic -= (data.hungerRateFactor - 1f) * 60f;
			}
			else if (data.hungerRateFactor > 0f && data.hungerRateFactor < 1f)
			{
				dynamic += (1f - data.hungerRateFactor) * 40f;
			}

			if (data.painFactor > 1f)
			{
				dynamic -= (data.painFactor - 1f) * 60f;
			}
			else if (data.painFactor > 0f && data.painFactor < 1f)
			{
				dynamic += (1f - data.painFactor) * 40f;
			}

			if (data.socialFightChanceFactor > 1f)
			{
				dynamic -= (data.socialFightChanceFactor - 1f) * 40f;
			}

			if (data.disallowedMentalStates != null && data.disallowedMentalStates.Count > 0)
			{
				dynamic += Mathf.Min(data.disallowedMentalStates.Count * 10f, 40f);
			}

			if (data.statOffsets != null)
			{
				for (int i = 0; i < data.statOffsets.Count; i++)
				{
					StatModifier mod = data.statOffsets[i];
					if (mod?.stat == null)
					{
						continue;
					}
					float val = IsLowerBetterStat(mod.stat) ? -mod.value : mod.value;
					dynamic += Mathf.Clamp(val * 40f, -45f, 45f);
				}
			}

			if (data.statFactors != null)
			{
				for (int i = 0; i < data.statFactors.Count; i++)
				{
					StatModifier mod = data.statFactors[i];
					if (mod?.stat == null)
					{
						continue;
					}
					float delta = mod.value - 1f;
					if (IsLowerBetterStat(mod.stat))
					{
						delta = -delta;
					}
					dynamic += Mathf.Clamp(delta * 55f, -55f, 55f);
				}
			}

			if (data.skillGains != null)
			{
				foreach (SkillGain gain in data.skillGains)
				{
					if (gain != null)
					{
						dynamic += gain.amount * 3.5f;
					}
				}
			}

			if (data.aptitudes != null)
			{
				for (int i = 0; i < data.aptitudes.Count; i++)
				{
					Aptitude apt = data.aptitudes[i];
					if (apt != null)
					{
						dynamic += apt.level * 5f;
					}
				}
			}
		}

		return dynamic * weight;
	}

	private static bool IsLowerBetterStat(StatDef stat)
	{
		return stat == StatDefOf.MentalBreakThreshold
			|| stat == StatDefOf.IncomingDamageFactor
			|| stat == StatDefOf.AimingDelayFactor
			|| stat.defName == "HungerRateMultiplier"
			|| stat.defName == "RestFallRateFactor";
	}

	private static int GetGeneAptitudeForSkill(Pawn pawn, SkillDef skill)
	{
		if (pawn?.genes == null || skill == null)
		{
			return 0;
		}
		int total = 0;
		List<Gene> genes = pawn.genes.GenesListForReading;
		for (int i = 0; i < genes.Count; i++)
		{
			Gene gene = genes[i];
			if (gene.Active && gene.def.aptitudes != null)
			{
				for (int j = 0; j < gene.def.aptitudes.Count; j++)
				{
					Aptitude apt = gene.def.aptitudes[j];
					if (apt.skill == skill)
					{
						total += apt.level;
					}
				}
			}
		}
		return total;
	}

	private static int CountSetBits(int mask)
	{
		int count = 0;
		uint u = (uint)mask;
		while (u != 0)
		{
			u &= u - 1;
			count++;
		}
		return count;
	}
}
