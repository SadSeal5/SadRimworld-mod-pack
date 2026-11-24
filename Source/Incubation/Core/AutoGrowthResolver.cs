using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Incubation.Core;

public static class AutoGrowthResolver
{
	private static readonly MethodInfo TrySetChoicesMethod = AccessTools.Method(typeof(ChoiceLetter_GrowthMoment), "TrySetChoices");

	public static bool TryResolve(ChoiceLetter_GrowthMoment letter)
	{
		if (letter == null
			|| letter.ArchiveView
			|| letter.choiceMade
			|| letter.pawn == null
			|| letter.pawn.Destroyed
			|| letter.pawn.Dead
			|| letter.pawn.skills == null
			|| letter.pawn.story?.traits == null)
		{
			return false;
		}

		if (TrySetChoicesMethod == null
			&& ((letter.traitChoices == null && letter.traitChoiceCount > 0)
				|| (letter.passionChoices == null && letter.passionGainsCount > 0)))
		{
			Log.ErrorOnce("[SadSeal.Incubation] ChoiceLetter_GrowthMoment.TrySetChoices method not found; aborting auto-resolve.", 84920144);
			return false;
		}

		TrySetChoicesMethod?.Invoke(letter, null);

		IncubationSettings settings = IncubationModule.Settings;

		Trait chosenTrait = letter.traitChoices != null && letter.traitChoices.Count > 0
			? GrowthTraitScorer.SelectBestTrait(letter.pawn, letter.traitChoices, letter.noTraitOptionShown, settings.avoidNegativeTraitsWithNoTrait)
			: null;

		List<SkillDef> chosenPassions = letter.passionChoices != null && letter.passionGainsCount > 0
			? GrowthPassionScorer.SelectBestPassions(letter.pawn, letter.passionChoices, letter.passionGainsCount, chosenTrait, settings.passionStrategy)
			: new List<SkillDef>();

		letter.MakeChoices(chosenPassions, chosenTrait);

		if (settings.showGrowthSelectionToast)
		{
			string traitText = chosenTrait != null && chosenTrait != ChoiceLetter_GrowthMoment.NoTrait
				? chosenTrait.LabelCap
				: "MegaMod_Incubation_NoTraitChosen".Translate().ToString();

			string passionsText = chosenPassions.Count > 0
				? string.Join(", ", chosenPassions.ConvertAll(s => s.LabelCap.ToString()))
				: "MegaMod_Incubation_NoPassionChosen".Translate().ToString();

			Messages.Message(
				"MegaMod_Incubation_GrowthToast".Translate(letter.pawn.Named("PAWN"), traitText.Named("TRAIT"), passionsText.Named("PASSIONS")),
				new LookTargets(letter.pawn),
				MessageTypeDefOf.NeutralEvent);
		}

		return true;
	}
}
