using Verse;

namespace SadSeal.MegaMod.Incubation
{
    public class IncubationSettings : IExposable
    {
        public float embryoSpeedMultiplier = 1.0f;
        public float childSpeedMultiplier = 1.0f;
        public float nutritionUsageMultiplier = 1.0f;
        public float powerUsageMultiplier = 1.0f;
        public bool autoEjectEnabled = true;
        public int targetEjectAge = 18;
        public bool preserveGrowthTier = true;
        public bool stasisProtection = true;
        public bool disableParentGuilt = true;

        public void ExposeData()
        {
            Scribe_Values.Look(ref embryoSpeedMultiplier, "embryoSpeedMultiplier", 1.0f);
            Scribe_Values.Look(ref childSpeedMultiplier, "childSpeedMultiplier", 1.0f);
            Scribe_Values.Look(ref nutritionUsageMultiplier, "nutritionUsageMultiplier", 1.0f);
            Scribe_Values.Look(ref powerUsageMultiplier, "powerUsageMultiplier", 1.0f);
            Scribe_Values.Look(ref autoEjectEnabled, "autoEjectEnabled", true);
            Scribe_Values.Look(ref targetEjectAge, "targetEjectAge", 18);
            Scribe_Values.Look(ref preserveGrowthTier, "preserveGrowthTier", true);
            Scribe_Values.Look(ref stasisProtection, "stasisProtection", true);
            Scribe_Values.Look(ref disableParentGuilt, "disableParentGuilt", true);
        }

        public void ResetToDefaults()
        {
            embryoSpeedMultiplier = 1.0f;
            childSpeedMultiplier = 1.0f;
            nutritionUsageMultiplier = 1.0f;
            powerUsageMultiplier = 1.0f;
            autoEjectEnabled = true;
            targetEjectAge = 18;
            preserveGrowthTier = true;
            stasisProtection = true;
            disableParentGuilt = true;
        }
    }
}
