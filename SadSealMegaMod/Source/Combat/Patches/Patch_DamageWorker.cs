using HarmonyLib;
using Verse;

namespace SadSeal.MegaMod.Combat.Patches
{
    [HarmonyPatch(typeof(DamageWorker_AddInjury), nameof(DamageWorker_AddInjury.ShouldReduceDamageToPreservePart))]
    public static class Patch_DamageWorker_ShouldReduceDamage
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result)
        {
            if (MegaMod.Settings?.Combat != null && MegaMod.Settings.Combat.preventOneHpPreservation)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
