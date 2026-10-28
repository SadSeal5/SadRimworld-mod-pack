using System;
using System.Linq;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Combat.AutoArm
{
    public static class WeaponClassifier
    {
        public static WeaponCategory GetCategory(Thing thing)
        {
            if (thing == null) return WeaponCategory.None;
            return GetCategory(thing.def);
        }

        public static WeaponCategory GetCategory(ThingDef def)
        {
            if (def == null || !def.IsWeapon) return WeaponCategory.None;
            if (IsExcludedWeapon(def)) return WeaponCategory.None;

            if (def.IsRangedWeapon)
            {
                VerbProperties verb = def.Verbs?.FirstOrDefault(v => v.isPrimary) ?? def.Verbs?.FirstOrDefault();
                int burst = verb?.burstShotCount ?? 1;
                float range = verb?.range ?? 25f;

                if (burst > 1) return WeaponCategory.RangedBurst;

                if (def.weaponTags != null && def.weaponTags.Any(t => t.IndexOf("Autopistol", StringComparison.OrdinalIgnoreCase) >= 0 || t.IndexOf("Automatic", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    return WeaponCategory.RangedBurst;
                }

                if (range > 28f || (def.weaponClasses != null && def.weaponClasses.Any(wc => wc.defName == "LongShots")))
                {
                    return WeaponCategory.RangedPrecision;
                }

                return WeaponCategory.RangedPrecision;
            }

            if (def.IsMeleeWeapon)
            {
                float sharpPower = 0f;
                float bluntPower = 0f;

                if (def.tools != null)
                {
                    foreach (Tool tool in def.tools)
                    {
                        if (tool.capacities != null)
                        {
                            foreach (ToolCapacityDef cap in tool.capacities)
                            {
                                if (cap == null) continue;
                                string capName = cap.defName.ToLowerInvariant();
                                if (capName.Contains("cut") || capName.Contains("stab") || capName.Contains("scratch") || capName.Contains("bite") || capName.Contains("pierce"))
                                {
                                    sharpPower += tool.power;
                                }
                                else
                                {
                                    bluntPower += tool.power;
                                }
                            }
                        }
                        else
                        {
                            bluntPower += tool.power;
                        }
                    }
                }

                return sharpPower >= bluntPower ? WeaponCategory.MeleeSharp : WeaponCategory.MeleeBlunt;
            }

            return WeaponCategory.None;
        }

        public static bool IsExcludedWeapon(ThingDef def)
        {
            if (def == null || !def.IsWeapon) return true;
            if (def.destroyOnDrop) return true;

            string lower = def.defName.ToLowerInvariant();
            if (lower.Contains("grenade") || lower.Contains("molotov") ||
                lower.Contains("doomsday") || lower.Contains("rocket") ||
                lower.Contains("orbital") || lower.Contains("tornado") ||
                lower.Contains("emp"))
            {
                return true;
            }

            if (def.Verbs != null)
            {
                foreach (VerbProperties verb in def.Verbs)
                {
                    if (verb.defaultProjectile != null && verb.defaultProjectile.projectile != null)
                    {
                        ProjectileProperties proj = verb.defaultProjectile.projectile;
                        if (proj.explosionRadius > 0f) return true;
                        DamageDef d = proj.damageDef;
                        if (d == DamageDefOf.Bomb || d == DamageDefOf.Flame || d == DamageDefOf.Extinguish || d == DamageDefOf.EMP)
                            return true;
                    }
                }
            }

            return false;
        }
    }
}
