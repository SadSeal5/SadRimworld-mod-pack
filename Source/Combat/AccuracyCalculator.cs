using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Combat
{
    public static class AccuracyCalculator
    {
        public static float CalculateShooterFactor(Thing caster, float distance, float? acc = null)
        {
            var settings = CombatModule.ModSettings;
            if (settings == null || !settings.enableAccuracyOverhaul || settings.accuracyMode == AccuracyFormulaMode.Vanilla)
            {
                float baseVanillaAcc = acc ?? ((caster is Pawn) ? caster.GetStatValue(StatDefOf.ShootingAccuracyPawn) : (caster?.GetStatValue(StatDefOf.ShootingAccuracyTurret) ?? 1f));
                float vanillaNum = Mathf.Pow(baseVanillaAcc, distance);
                return ApplyDistanceFactors(caster, distance, vanillaNum);
            }

            if (caster != null && !settings.affectTurrets && !(caster is Pawn))
            {
                float baseVanillaAcc = acc ?? (caster.GetStatValue(StatDefOf.ShootingAccuracyTurret));
                float vanillaNum = Mathf.Pow(baseVanillaAcc, distance);
                return ApplyDistanceFactors(caster, distance, vanillaNum);
            }

            float baseAcc;
            if (acc.HasValue)
            {
                baseAcc = acc.Value;
            }
            else if (caster is Pawn pawn)
            {
                baseAcc = pawn.GetStatValue(StatDefOf.ShootingAccuracyPawn);
            }
            else if (caster != null)
            {
                baseAcc = caster.GetStatValue(StatDefOf.ShootingAccuracyTurret);
            }
            else
            {
                baseAcc = 0.96f;
            }

            float dist = Mathf.Max(1f, distance);
            float falloff = Mathf.Max(0.1f, settings.distanceFalloffMultiplier);
            float factor;

            switch (settings.accuracyMode)
            {
                case AccuracyFormulaMode.SoftBallistic:
                    float exponent = Mathf.Sqrt(dist) * falloff;
                    factor = Mathf.Pow(baseAcc, exponent);
                    break;

                case AccuracyFormulaMode.Hyperbolic:
                    factor = 1f - (1f - baseAcc) * (1f + dist * 0.12f * falloff);
                    break;

                case AccuracyFormulaMode.SkillFocused:
                    float baseSkillFactor = Mathf.Pow(baseAcc, Mathf.Sqrt(dist) * 0.75f * falloff);
                    float baseline = baseAcc >= 0.95f ? 0.70f : (baseAcc >= 0.90f ? 0.50f : 0.20f);
                    factor = Mathf.Max(baseSkillFactor, baseline * baseAcc);
                    break;

                default:
                    factor = Mathf.Pow(baseAcc, dist);
                    break;
            }

            factor = ApplyDistanceFactors(caster, distance, factor);
            return Mathf.Clamp(factor, 0.0201f, 0.999f);
        }

        private static float ApplyDistanceFactors(Thing caster, float distance, float factor)
        {
            if (caster is Pawn)
            {
                if (distance <= 3f)
                {
                    factor *= caster.GetStatValue(StatDefOf.ShootingAccuracyFactor_Touch);
                }
                else if (distance <= 12f)
                {
                    factor *= Mathf.Lerp(caster.GetStatValue(StatDefOf.ShootingAccuracyFactor_Touch), caster.GetStatValue(StatDefOf.ShootingAccuracyFactor_Short), (distance - 3f) / 9f);
                }
                else if (distance <= 25f)
                {
                    factor *= Mathf.Lerp(caster.GetStatValue(StatDefOf.ShootingAccuracyFactor_Short), caster.GetStatValue(StatDefOf.ShootingAccuracyFactor_Medium), (distance - 12f) / 13f);
                }
                else
                {
                    factor = ((!(distance <= 40f)) ? (factor * caster.GetStatValue(StatDefOf.ShootingAccuracyFactor_Long)) : (factor * Mathf.Lerp(caster.GetStatValue(StatDefOf.ShootingAccuracyFactor_Medium), caster.GetStatValue(StatDefOf.ShootingAccuracyFactor_Long), (distance - 25f) / 15f)));
                }
            }
            return factor;
        }
    }
}
