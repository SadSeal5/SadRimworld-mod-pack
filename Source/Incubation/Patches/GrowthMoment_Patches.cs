using HarmonyLib;
using RimWorld;
using SadSeal.MegaMod.Incubation.Core;
using Verse;

namespace SadSeal.MegaMod.Incubation.Patches;

[HarmonyPatch(typeof(ChoiceLetter_GrowthMoment), nameof(ChoiceLetter_GrowthMoment.CanDismissWithRightClick), MethodType.Getter)]
public static class ChoiceLetter_GrowthMoment_CanDismissWithRightClick_Patch
{
	[HarmonyPrepare]
	public static bool Prepare() => ModsConfig.BiotechActive;

	[HarmonyPostfix]
	public static void Postfix(ref bool __result)
	{
		if (IncubationModule.Instance.IsEnabled && IncubationModule.Settings.enableRightClickAutoGrowth)
		{
			__result = true;
		}
	}
}

[HarmonyPatch(typeof(ChoiceLetter_GrowthMoment), "GetMouseoverText")]
public static class ChoiceLetter_GrowthMoment_GetMouseoverText_Patch
{
	[HarmonyPrepare]
	public static bool Prepare() => ModsConfig.BiotechActive;

	[HarmonyPostfix]
	public static void Postfix(ChoiceLetter_GrowthMoment __instance, ref string __result)
	{
		if (IncubationModule.Instance.IsEnabled && !__instance.ArchiveView && IncubationModule.Settings.enableRightClickAutoGrowth)
		{
			__result += "\n\n" + "MegaMod_Incubation_RightClickHint".Translate().Colorize(ColoredText.SubtleGrayColor);
		}
	}
}

[HarmonyPatch(typeof(LetterStack), nameof(LetterStack.RemoveLetter))]
public static class LetterStack_RemoveLetter_Patch
{
	[HarmonyPrepare]
	public static bool Prepare() => ModsConfig.BiotechActive;

	[HarmonyPrefix]
	public static void Prefix(Letter let)
	{
		if (IncubationModule.Instance.IsEnabled
			&& IncubationModule.Settings.enableRightClickAutoGrowth
			&& let is ChoiceLetter_GrowthMoment gm
			&& !gm.choiceMade
			&& !gm.ArchiveView)
		{
			AutoGrowthResolver.TryResolve(gm);
		}
	}
}

[HarmonyPatch(typeof(LetterStack), nameof(LetterStack.ReceiveLetter), typeof(Letter), typeof(string), typeof(int), typeof(bool))]
public static class LetterStack_ReceiveLetter_Patch
{
	[HarmonyPrepare]
	public static bool Prepare() => ModsConfig.BiotechActive;

	[HarmonyPrefix]
	public static bool Prefix(Letter let, string debugInfo, int delayTicks, bool playSound)
	{
		if (!IncubationModule.Instance.IsEnabled || delayTicks > 0)
		{
			return true;
		}

		if (let is ChoiceLetter_GrowthMoment gm && !gm.choiceMade && !gm.ArchiveView)
		{
			GrowthAutoMode mode = IncubationModule.Settings.autoResolveMode;
			bool shouldAutoResolve = mode == GrowthAutoMode.AutoAllChildren
				|| (mode == GrowthAutoMode.AutoVatChildrenOnly && gm.pawn?.ParentHolder is Building_GrowthVat);

			if (shouldAutoResolve)
			{
				AutoGrowthResolver.TryResolve(gm);
				Find.Archive?.Add(gm);
				return false;
			}
		}

		return true;
	}
}
