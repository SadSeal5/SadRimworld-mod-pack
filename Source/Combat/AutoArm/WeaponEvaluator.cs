using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Combat.AutoArm
{
    public static class WeaponEvaluator
    {
        public static float Evaluate(Thing weapon, Pawn pawn)
        {
            if (weapon == null || !weapon.def.IsWeapon) return 0f;
            if (WeaponClassifier.IsExcludedWeapon(weapon.def)) return -1f;

            CompBiocodable bio = weapon.TryGetComp<CompBiocodable>();
            if (bio != null && bio.Biocoded && bio.CodedPawn != pawn) return -1f;

            CompBladelinkWeapon blade = weapon.TryGetComp<CompBladelinkWeapon>();
            if (blade != null && blade.Biocoded && blade.CodedPawn != pawn) return -1f;

            float score = 0f;

            if (weapon.def.IsRangedWeapon)
            {
                VerbProperties verb = weapon.def.Verbs?.FirstOrDefault(v => v.isPrimary) ?? weapon.def.Verbs?.FirstOrDefault();
                if (verb == null) return 0f;

                float burst = verb.burstShotCount;
                float warmup = verb.warmupTime;
                float cooldown = weapon.GetStatValue(StatDefOf.RangedWeapon_Cooldown);
                float cycleTime = Mathf.Max(0.2f, warmup + cooldown);

                float dmg = 10f;
                float ap = 0.15f;
                if (verb.defaultProjectile != null && verb.defaultProjectile.projectile != null)
                {
                    dmg = verb.defaultProjectile.projectile.GetDamageAmount(weapon);
                    ap = verb.defaultProjectile.projectile.GetArmorPenetration(weapon);
                }

                float rawDps = (dmg * burst) / cycleTime;

                float accTouch = weapon.GetStatValue(StatDefOf.AccuracyTouch);
                float accShort = weapon.GetStatValue(StatDefOf.AccuracyShort);
                float accMed = weapon.GetStatValue(StatDefOf.AccuracyMedium);
                float accLong = weapon.GetStatValue(StatDefOf.AccuracyLong);
                float avgAcc = Mathf.Clamp((accTouch + accShort * 2f + accMed * 2f + accLong) / 6f, 0.1f, 1.0f);

                score = rawDps * avgAcc * (1f + ap);
            }
            else if (weapon.def.IsMeleeWeapon)
            {
                float dps = weapon.GetStatValue(StatDefOf.MeleeWeapon_AverageDPS);

                float totalAp = 0f;
                int toolCount = 0;
                if (weapon.def.tools != null)
                {
                    foreach (Tool t in weapon.def.tools)
                    {
                        totalAp += t.armorPenetration >= 0f ? t.armorPenetration : (t.power * 0.015f);
                        toolCount++;
                    }
                }
                float avgAp = toolCount > 0 ? (totalAp / toolCount) : 0.15f;

                score = dps * (1f + avgAp);
            }

            if (weapon.Stuff != null)
            {
                if (weapon.Stuff == ThingDefOf.Plasteel) score *= 1.15f;
                else if (weapon.Stuff == ThingDefOf.Uranium) score *= 1.2f;
            }

            if (weapon.TryGetQuality(out QualityCategory qc))
            {
                score *= (1f + ((int)qc * 0.05f));
            }

            if (weapon.MaxHitPoints > 0)
            {
                float hpPercent = (float)weapon.HitPoints / weapon.MaxHitPoints;
                if (hpPercent < 0.5f)
                {
                    score *= 0.5f;
                }
            }

            return Mathf.Max(0.1f, score);
        }
    }
}
