using System;
using System.Linq;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.AutoArm
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
                string name = def.defName.ToLowerInvariant();

                bool burstKeyword = name.Contains("machine")
                    || name.Contains("auto")
                    || name.Contains("assault")
                    || name.Contains("minigun")
                    || name.Contains("lmg")
                    || name.Contains("burst");

                if (def.weaponTags != null && def.weaponTags.Any(t =>
                    t.IndexOf("autopistol", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    t.IndexOf("automatic", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    burstKeyword = true;
                }

                if (burst > 1 || burstKeyword)
                {
                    return WeaponCategory.RangedBurst;
                }

                return WeaponCategory.RangedSingleShot;
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
                                if (capName.Contains("cut") || capName.Contains("stab") ||
                                    capName.Contains("scratch") || capName.Contains("bite") ||
                                    capName.Contains("pierce") || capName.Contains("poke"))
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
            if (MegaMod.Settings?.AutoArm?.IsBlacklisted(def) == true) return true;

            string lower = def.defName.ToLowerInvariant();
            if (lower.Contains("grenade") || lower.Contains("molotov") ||
                lower.Contains("doomsday") || lower.Contains("rocket") ||
                lower.Contains("orbital") || lower.Contains("tornado") ||
                lower.Contains("emp") || lower.Contains("smoke") ||
                lower.Contains("firefoam") || lower.Contains("triple"))
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
                        if (d == DamageDefOf.Bomb || d == DamageDefOf.Flame ||
                            d == DamageDefOf.Extinguish || d == DamageDefOf.EMP ||
                            d == DamageDefOf.Smoke || d == DamageDefOf.Stun)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
