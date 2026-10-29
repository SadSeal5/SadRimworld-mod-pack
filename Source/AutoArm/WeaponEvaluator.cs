using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.AutoArm
{
    public static class WeaponEvaluator
    {
        public static float Evaluate(Thing weapon, Pawn pawn, AutoArmSettings settings = null)
        {
            if (weapon == null || !weapon.def.IsWeapon) return 0f;
            if (WeaponClassifier.IsExcludedWeapon(weapon.def)) return -1f;

            CompBiocodable bio = weapon.TryGetComp<CompBiocodable>();
            if (bio != null && bio.Biocoded && bio.CodedPawn != pawn) return -1f;

            CompBladelinkWeapon blade = weapon.TryGetComp<CompBladelinkWeapon>();
            if (blade != null && blade.Biocoded && blade.CodedPawn != pawn) return -1f;

            if (settings == null)
            {
                settings = MegaMod.Settings?.AutoArm;
            }

            WeaponCategory cat = WeaponClassifier.GetCategory(weapon);
            if (cat == WeaponCategory.None) return 0f;

            float score = 0f;

            if (cat == WeaponCategory.RangedSingleShot)
            {
                VerbProperties verb = weapon.def.Verbs?.FirstOrDefault(v => v.isPrimary) ?? weapon.def.Verbs?.FirstOrDefault();
                if (verb == null) return 0f;

                float ap = 0.15f;
                if (verb.defaultProjectile != null && verb.defaultProjectile.projectile != null)
                {
                    ap = verb.defaultProjectile.projectile.GetArmorPenetration(weapon);
                }

                float accTouch = weapon.GetStatValue(StatDefOf.AccuracyTouch);
                float accShort = weapon.GetStatValue(StatDefOf.AccuracyShort);
                float accMed = weapon.GetStatValue(StatDefOf.AccuracyMedium);
                float accLong = weapon.GetStatValue(StatDefOf.AccuracyLong);
                float avgAcc = Mathf.Clamp((accTouch + accShort * 2f + accMed * 2f + accLong) / 6f, 0.1f, 1.0f);

                string name = weapon.def.defName.ToLowerInvariant();
                float range = verb.range;
                bool isLongShot = weapon.def.weaponClasses != null && weapon.def.weaponClasses.Any(wc => wc.defName == "LongShots");

                float baseTierScore = 15f;
                if (range > 35f || isLongShot || name.Contains("sniper") || name.Contains("lance") ||
                    name.Contains("antimateriel") || name.Contains("anti-materiel"))
                {
                    baseTierScore = 50f;
                }
                else if (range >= 26f || name.Contains("rifle") || name.Contains("bow") ||
                         name.Contains("bolt") || name.Contains("musket"))
                {
                    baseTierScore = 30f;
                }
                else
                {
                    baseTierScore = 15f;
                }

                score = baseTierScore * (1f + ap * 0.5f) * avgAcc;
            }
            else if (cat == WeaponCategory.RangedBurst)
            {
                VerbProperties verb = weapon.def.Verbs?.FirstOrDefault(v => v.isPrimary) ?? weapon.def.Verbs?.FirstOrDefault();
                if (verb == null) return 0f;

                float burst = Mathf.Max(1f, (float)verb.burstShotCount);
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

                float accTouch = weapon.GetStatValue(StatDefOf.AccuracyTouch);
                float accShort = weapon.GetStatValue(StatDefOf.AccuracyShort);
                float accMed = weapon.GetStatValue(StatDefOf.AccuracyMedium);
                float accLong = weapon.GetStatValue(StatDefOf.AccuracyLong);
                float avgAcc = Mathf.Clamp((accTouch + accShort * 2f + accMed * 2f + accLong) / 6f, 0.1f, 1.0f);

                score = (dmg * burst) / cycleTime * avgAcc * (1f + ap);
            }
            else if (cat == WeaponCategory.MeleeSharp || cat == WeaponCategory.MeleeBlunt)
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

                if (cat == WeaponCategory.MeleeSharp)
                {
                    if (weapon.Stuff == ThingDefOf.Plasteel)
                    {
                        score *= 1.20f;
                    }
                }
                else
                {
                    if (weapon.Stuff == ThingDefOf.Uranium)
                    {
                        score *= 1.25f;
                    }
                }
            }

            if (settings == null || settings.considerQuality)
            {
                if (weapon.TryGetQuality(out QualityCategory qc))
                {
                    switch (qc)
                    {
                        case QualityCategory.Awful: score *= 0.60f; break;
                        case QualityCategory.Poor: score *= 0.80f; break;
                        case QualityCategory.Normal: score *= 1.00f; break;
                        case QualityCategory.Good: score *= 1.15f; break;
                        case QualityCategory.Excellent: score *= 1.30f; break;
                        case QualityCategory.Masterwork: score *= 1.50f; break;
                        case QualityCategory.Legendary: score *= 1.75f; break;
                    }
                }
            }

            if (settings == null || settings.considerHitPoints)
            {
                if (weapon.MaxHitPoints > 0)
                {
                    float hpPercent = (float)weapon.HitPoints / weapon.MaxHitPoints;
                    if (hpPercent < 0.50f)
                    {
                        score *= 0.60f;
                    }
                }
            }

            if (blade != null && blade.Biocoded && blade.CodedPawn == pawn)
            {
                score = score * 5.0f + 500f;
            }

            return Mathf.Max(0.1f, score);
        }
    }
}
