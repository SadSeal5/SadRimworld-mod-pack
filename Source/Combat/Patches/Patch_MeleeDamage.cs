using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Combat.Patches
{
    [HarmonyPatch(typeof(Verb_MeleeAttackDamage), "DamageInfosToHit")]
    public static class Patch_Verb_MeleeAttackDamage_DamageInfosToHit
    {
        [HarmonyPostfix]
        public static void Postfix(ref IEnumerable<DamageInfo> __result)
        {
            var settings = MegaMod.Settings?.Combat;
            if (settings == null || __result == null) return;

            float dmgMult = settings.meleeDamageMultiplier;
            float apMult = settings.meleeArmorPenetrationMultiplier;

            if (UnityEngine.Mathf.Approximately(dmgMult, 1f) && UnityEngine.Mathf.Approximately(apMult, 1f))
            {
                return;
            }

            __result = ModifyDamageInfos(__result, dmgMult, apMult);
        }

        private static IEnumerable<DamageInfo> ModifyDamageInfos(IEnumerable<DamageInfo> original, float dmgMult, float apMult)
        {
            foreach (var dinfo in original)
            {
                float newAmount = dinfo.Amount * dmgMult;
                float newAp = dinfo.ArmorPenetrationInt * apMult;

                yield return new DamageInfo(
                    dinfo.Def,
                    newAmount,
                    newAp,
                    dinfo.Angle,
                    dinfo.Instigator,
                    dinfo.HitPart,
                    dinfo.Weapon,
                    dinfo.Category,
                    dinfo.IntendedTarget,
                    dinfo.InstigatorGuilty,
                    dinfo.SpawnFilth
                );
            }
        }
    }
}
