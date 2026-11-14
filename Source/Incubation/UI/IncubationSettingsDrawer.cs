using System;
using SadSeal.Common.UI;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Incubation.UI;

public static class IncubationSettingsDrawer
{
	private static Vector2 scrollPosition = Vector2.zero;

	private static readonly int[] SupportedAges = { 3, 7, 10, 13, 18 };

	private static readonly float[] SpeedPresets = { 1f, 2f, 5f, 10f, 20f, 50f };

	public static void DrawSettings(Rect inRect, IncubationSettings settings)
	{
		if (settings == null)
		{
			return;
		}

		Rect viewRect = new Rect(0f, 0f, inRect.width - 24f, 1150f);
		Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

		Listing_Standard listing = new Listing_Standard();
		listing.Begin(viewRect);

		SadUI.DrawHeader(listing, "MegaMod_Incubation_Title", "MegaMod_Incubation_Desc");

		if (!ModsConfig.BiotechActive)
		{
			GUI.color = Color.yellow;
			listing.Label("MegaMod_Incubation_BiotechRequired".Translate());
			GUI.color = Color.white;
			listing.Gap(12f);
		}

		Text.Font = GameFont.Medium;
		listing.Label("MegaMod_Incubation_SpeedSection".Translate());
		Text.Font = GameFont.Small;
		listing.Gap(4f);

		settings.embryoSpeedMultiplier = listing.SliderLabeled(
			"MegaMod_Incubation_EmbryoSpeed".Translate() + $": {settings.embryoSpeedMultiplier:0.#}x",
			settings.embryoSpeedMultiplier,
			0.1f,
			100f,
			0.5f,
			"MegaMod_Incubation_EmbryoSpeed_Desc".Translate());
		listing.Gap(4f);

		settings.childSpeedMultiplier = listing.SliderLabeled(
			"MegaMod_Incubation_ChildSpeed".Translate() + $": {settings.childSpeedMultiplier:0.#}x (Total: {Mathf.RoundToInt(20f * settings.childSpeedMultiplier)}x)",
			settings.childSpeedMultiplier,
			0.1f,
			100f,
			0.5f,
			"MegaMod_Incubation_ChildSpeed_Desc".Translate());
		listing.Gap(4f);

		Rect presetRect = listing.GetRect(28f);
		Widgets.Label(new Rect(presetRect.x, presetRect.y + 4f, 160f, 24f), "MegaMod_Incubation_Presets".Translate() + ":");
		float presetStartX = presetRect.x + 165f;
		const float presetBtnWidth = 46f;
		for (int i = 0; i < SpeedPresets.Length; i++)
		{
			float preset = SpeedPresets[i];
			if (Widgets.ButtonText(new Rect(presetStartX + i * (presetBtnWidth + 4f), presetRect.y, presetBtnWidth, 24f), $"{preset:0}x"))
			{
				settings.embryoSpeedMultiplier = preset;
				settings.childSpeedMultiplier = preset;
			}
		}

		listing.Gap(14f);
		Text.Font = GameFont.Medium;
		listing.Label("MegaMod_Incubation_ResourcesSection".Translate());
		Text.Font = GameFont.Small;
		listing.Gap(4f);

		settings.nutritionUsageMultiplier = listing.SliderLabeled(
			"MegaMod_Incubation_NutritionMultiplier".Translate() + $": {settings.nutritionUsageMultiplier:P0}",
			settings.nutritionUsageMultiplier,
			0f,
			5f,
			0.5f,
			"MegaMod_Incubation_NutritionMultiplier_Desc".Translate());
		listing.Gap(4f);

		settings.powerUsageMultiplier = listing.SliderLabeled(
			"MegaMod_Incubation_PowerMultiplier".Translate() + $": {settings.powerUsageMultiplier:P0}",
			settings.powerUsageMultiplier,
			0f,
			5f,
			0.5f,
			"MegaMod_Incubation_PowerMultiplier_Desc".Translate());

		listing.Gap(14f);
		Text.Font = GameFont.Medium;
		listing.Label("MegaMod_Incubation_SafetySection".Translate());
		Text.Font = GameFont.Small;
		listing.Gap(4f);

		listing.CheckboxLabeled(
			"MegaMod_Incubation_AutoEject".Translate(),
			ref settings.autoEjectEnabled,
			"MegaMod_Incubation_AutoEject_Desc".Translate());
		listing.Gap(4f);

		if (settings.autoEjectEnabled)
		{
			Rect ageRect = listing.GetRect(28f);
			Widgets.Label(new Rect(ageRect.x + 16f, ageRect.y + 4f, 220f, 24f), "MegaMod_Incubation_TargetAge".Translate() + $": {settings.targetEjectAge}");
			float ageStartX = ageRect.x + 240f;
			const float ageBtnWidth = 42f;
			Color prevColor = GUI.color;
			for (int i = 0; i < SupportedAges.Length; i++)
			{
				int age = SupportedAges[i];
				GUI.color = settings.targetEjectAge == age ? Color.green : prevColor;
				if (Widgets.ButtonText(new Rect(ageStartX + i * (ageBtnWidth + 4f), ageRect.y, ageBtnWidth, 24f), age.ToString()))
				{
					settings.targetEjectAge = age;
				}
			}
			GUI.color = prevColor;
			listing.Gap(6f);
		}

		listing.CheckboxLabeled(
			"MegaMod_Incubation_PreserveGrowthTier".Translate(),
			ref settings.preserveGrowthTier,
			"MegaMod_Incubation_PreserveGrowthTier_Desc".Translate());
		listing.Gap(4f);

		listing.CheckboxLabeled(
			"MegaMod_Incubation_StasisProtection".Translate(),
			ref settings.stasisProtection,
			"MegaMod_Incubation_StasisProtection_Desc".Translate());
		listing.Gap(4f);

		listing.CheckboxLabeled(
			"MegaMod_Incubation_DisableParentGuilt".Translate(),
			ref settings.disableParentGuilt,
			"MegaMod_Incubation_DisableParentGuilt_Desc".Translate());

		listing.Gap(14f);
		Text.Font = GameFont.Medium;
		listing.Label("MegaMod_Incubation_GrowthSection".Translate());
		Text.Font = GameFont.Small;
		listing.Gap(4f);

		listing.CheckboxLabeled(
			"MegaMod_Incubation_RightClickAutoGrowth".Translate(),
			ref settings.enableRightClickAutoGrowth,
			"MegaMod_Incubation_RightClickAutoGrowth_Desc".Translate());
		listing.Gap(4f);

		DrawCycleButton(
			listing,
			"MegaMod_Incubation_AutoResolveMode".Translate(),
			GetAutoModeLabel(settings.autoResolveMode),
			"MegaMod_Incubation_AutoResolveMode_Desc".Translate(),
			() => settings.autoResolveMode = NextEnum(settings.autoResolveMode));
		listing.Gap(4f);

		DrawCycleButton(
			listing,
			"MegaMod_Incubation_PassionStrategy".Translate(),
			GetStrategyLabel(settings.passionStrategy),
			"MegaMod_Incubation_PassionStrategy_Desc".Translate(),
			() => settings.passionStrategy = NextEnum(settings.passionStrategy));
		listing.Gap(4f);

		listing.CheckboxLabeled(
			"MegaMod_Incubation_AvoidNegativeTraits".Translate(),
			ref settings.avoidNegativeTraitsWithNoTrait,
			"MegaMod_Incubation_AvoidNegativeTraits_Desc".Translate());
		listing.Gap(4f);

		listing.CheckboxLabeled(
			"MegaMod_Incubation_ShowGrowthToast".Translate(),
			ref settings.showGrowthSelectionToast,
			"MegaMod_Incubation_ShowGrowthToast_Desc".Translate());

		listing.Gap(16f);
		Rect resetRect = listing.GetRect(30f);
		if (Widgets.ButtonText(new Rect(resetRect.x, resetRect.y, 200f, 30f), "MegaMod_Incubation_ResetDefaults".Translate()))
		{
			settings.ResetToDefaults();
		}

		listing.End();
		Widgets.EndScrollView();
	}

	private static void DrawCycleButton(Listing_Standard listing, string label, string valueLabel, string tooltip, Action onClick)
	{
		Rect rowRect = listing.GetRect(28f);
		if (!tooltip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rowRect, tooltip);
		}
		Rect labelRect = new Rect(rowRect.x, rowRect.y + 3f, rowRect.width * 0.55f, 24f);
		Rect buttonRect = new Rect(rowRect.x + rowRect.width * 0.57f, rowRect.y, rowRect.width * 0.43f, 26f);
		Widgets.Label(labelRect, label);
		if (Widgets.ButtonText(buttonRect, valueLabel))
		{
			onClick();
		}
	}

	private static T NextEnum<T>(T current) where T : struct, Enum
	{
		T[] values = (T[])Enum.GetValues(typeof(T));
		int index = Array.IndexOf(values, current);
		return values[(index + 1) % values.Length];
	}

	private static string GetAutoModeLabel(GrowthAutoMode mode) => mode switch
	{
		GrowthAutoMode.RightClickOnly => "MegaMod_Incubation_AutoMode_RightClickOnly".Translate(),
		GrowthAutoMode.AutoVatChildrenOnly => "MegaMod_Incubation_AutoMode_AutoVatChildrenOnly".Translate(),
		GrowthAutoMode.AutoAllChildren => "MegaMod_Incubation_AutoMode_AutoAllChildren".Translate(),
		_ => mode.ToString()
	};

	private static string GetStrategyLabel(PassionStrategy strategy) => strategy switch
	{
		PassionStrategy.BalancedColonyNeed => "MegaMod_Incubation_Strategy_BalancedColonyNeed".Translate(),
		PassionStrategy.CombatFocused => "MegaMod_Incubation_Strategy_CombatFocused".Translate(),
		PassionStrategy.ExistingSynergy => "MegaMod_Incubation_Strategy_ExistingSynergy".Translate(),
		PassionStrategy.BroadCoverage => "MegaMod_Incubation_Strategy_BroadCoverage".Translate(),
		_ => strategy.ToString()
	};
}
