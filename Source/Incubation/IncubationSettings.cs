using Verse;

namespace SadSeal.MegaMod.Incubation;

public enum GrowthAutoMode
{
	RightClickOnly,
	AutoVatChildrenOnly,
	AutoAllChildren
}

public enum PassionStrategy
{
	BalancedColonyNeed,
	CombatFocused,
	ExistingSynergy,
	BroadCoverage
}

public class IncubationSettings : IExposable
{
	public float embryoSpeedMultiplier = 1f;

	public float childSpeedMultiplier = 1f;

	public float nutritionUsageMultiplier = 1f;

	public float powerUsageMultiplier = 1f;

	public bool autoEjectEnabled = true;

	public int targetEjectAge = 18;

	public bool preserveGrowthTier = true;

	public bool stasisProtection = true;

	public bool disableParentGuilt = true;

	public bool enableRightClickAutoGrowth = true;

	public GrowthAutoMode autoResolveMode = GrowthAutoMode.RightClickOnly;

	public PassionStrategy passionStrategy = PassionStrategy.BalancedColonyNeed;

	public bool avoidNegativeTraitsWithNoTrait = true;

	public bool showGrowthSelectionToast = true;

	public void ExposeData()
	{
		Scribe_Values.Look(ref embryoSpeedMultiplier, "embryoSpeedMultiplier", 1f);
		Scribe_Values.Look(ref childSpeedMultiplier, "childSpeedMultiplier", 1f);
		Scribe_Values.Look(ref nutritionUsageMultiplier, "nutritionUsageMultiplier", 1f);
		Scribe_Values.Look(ref powerUsageMultiplier, "powerUsageMultiplier", 1f);
		Scribe_Values.Look(ref autoEjectEnabled, "autoEjectEnabled", true);
		Scribe_Values.Look(ref targetEjectAge, "targetEjectAge", 18);
		Scribe_Values.Look(ref preserveGrowthTier, "preserveGrowthTier", true);
		Scribe_Values.Look(ref stasisProtection, "stasisProtection", true);
		Scribe_Values.Look(ref disableParentGuilt, "disableParentGuilt", true);
		Scribe_Values.Look(ref enableRightClickAutoGrowth, "enableRightClickAutoGrowth", true);
		Scribe_Values.Look(ref autoResolveMode, "autoResolveMode", GrowthAutoMode.RightClickOnly);
		Scribe_Values.Look(ref passionStrategy, "passionStrategy", PassionStrategy.BalancedColonyNeed);
		Scribe_Values.Look(ref avoidNegativeTraitsWithNoTrait, "avoidNegativeTraitsWithNoTrait", true);
		Scribe_Values.Look(ref showGrowthSelectionToast, "showGrowthSelectionToast", true);
	}

	public void ResetToDefaults()
	{
		embryoSpeedMultiplier = 1f;
		childSpeedMultiplier = 1f;
		nutritionUsageMultiplier = 1f;
		powerUsageMultiplier = 1f;
		autoEjectEnabled = true;
		targetEjectAge = 18;
		preserveGrowthTier = true;
		stasisProtection = true;
		disableParentGuilt = true;
		enableRightClickAutoGrowth = true;
		autoResolveMode = GrowthAutoMode.RightClickOnly;
		passionStrategy = PassionStrategy.BalancedColonyNeed;
		avoidNegativeTraitsWithNoTrait = true;
		showGrowthSelectionToast = true;
	}
}
