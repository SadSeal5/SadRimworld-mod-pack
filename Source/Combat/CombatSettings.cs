using Verse;

namespace SadSeal.MegaMod.Combat
{
    public class CombatSettings : IExposable
    {
        public bool enableAccuracyOverhaul = true;
        public AccuracyFormulaMode accuracyMode = AccuracyFormulaMode.SoftBallistic;
        public float distanceFalloffMultiplier = 1.0f;
        public float damageMultiplier = 1.35f;
        public float armorPenetrationMultiplier = 1.25f;
        public float meleeDamageMultiplier = 1.25f;
        public float meleeArmorPenetrationMultiplier = 1.20f;
        public float projectileSpeedMultiplier = 1.50f;
        public bool enableStaggerEnhancement = true;
        public float staggerMoveSpeedFactor = 0.25f;
        public bool preventOneHpPreservation = true;
        public bool affectTurrets = true;
        public LethalityPreset activePreset = LethalityPreset.TacticalLethal;

        public void ExposeData()
        {
            Scribe_Values.Look(ref enableAccuracyOverhaul, "enableAccuracyOverhaul", true);
            Scribe_Values.Look(ref accuracyMode, "accuracyMode", AccuracyFormulaMode.SoftBallistic);
            Scribe_Values.Look(ref distanceFalloffMultiplier, "distanceFalloffMultiplier", 1.0f);
            Scribe_Values.Look(ref damageMultiplier, "damageMultiplier", 1.35f);
            Scribe_Values.Look(ref armorPenetrationMultiplier, "armorPenetrationMultiplier", 1.25f);
            Scribe_Values.Look(ref meleeDamageMultiplier, "meleeDamageMultiplier", 1.25f);
            Scribe_Values.Look(ref meleeArmorPenetrationMultiplier, "meleeArmorPenetrationMultiplier", 1.20f);
            Scribe_Values.Look(ref projectileSpeedMultiplier, "projectileSpeedMultiplier", 1.50f);
            Scribe_Values.Look(ref enableStaggerEnhancement, "enableStaggerEnhancement", true);
            Scribe_Values.Look(ref staggerMoveSpeedFactor, "staggerMoveSpeedFactor", 0.25f);
            Scribe_Values.Look(ref preventOneHpPreservation, "preventOneHpPreservation", true);
            Scribe_Values.Look(ref affectTurrets, "affectTurrets", true);
            Scribe_Values.Look(ref activePreset, "activePreset", LethalityPreset.TacticalLethal);
        }

        public void ApplyPreset(LethalityPreset preset)
        {
            activePreset = preset;
            switch (preset)
            {
                case LethalityPreset.VanillaPlus:
                    enableAccuracyOverhaul = true;
                    accuracyMode = AccuracyFormulaMode.SoftBallistic;
                    distanceFalloffMultiplier = 0.85f;
                    damageMultiplier = 1.15f;
                    armorPenetrationMultiplier = 1.10f;
                    meleeDamageMultiplier = 1.10f;
                    meleeArmorPenetrationMultiplier = 1.10f;
                    projectileSpeedMultiplier = 1.25f;
                    enableStaggerEnhancement = false;
                    staggerMoveSpeedFactor = 0.50f;
                    preventOneHpPreservation = false;
                    affectTurrets = true;
                    break;

                case LethalityPreset.TacticalLethal:
                    enableAccuracyOverhaul = true;
                    accuracyMode = AccuracyFormulaMode.SoftBallistic;
                    distanceFalloffMultiplier = 1.0f;
                    damageMultiplier = 1.35f;
                    armorPenetrationMultiplier = 1.25f;
                    meleeDamageMultiplier = 1.25f;
                    meleeArmorPenetrationMultiplier = 1.20f;
                    projectileSpeedMultiplier = 1.50f;
                    enableStaggerEnhancement = true;
                    staggerMoveSpeedFactor = 0.25f;
                    preventOneHpPreservation = true;
                    affectTurrets = true;
                    break;

                case LethalityPreset.HarshRealism:
                    enableAccuracyOverhaul = true;
                    accuracyMode = AccuracyFormulaMode.SkillFocused;
                    distanceFalloffMultiplier = 1.2f;
                    damageMultiplier = 1.80f;
                    armorPenetrationMultiplier = 1.50f;
                    meleeDamageMultiplier = 1.50f;
                    meleeArmorPenetrationMultiplier = 1.40f;
                    projectileSpeedMultiplier = 2.00f;
                    enableStaggerEnhancement = true;
                    staggerMoveSpeedFactor = 0.15f;
                    preventOneHpPreservation = true;
                    affectTurrets = true;
                    break;

                case LethalityPreset.Custom:
                    break;
            }
        }
    }
}
