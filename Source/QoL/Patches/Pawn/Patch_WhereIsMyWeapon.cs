using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.QoL.Patches.Pawn
{
    public static class Patch_WhereIsMyWeapon
    {
        private static readonly Dictionary<Verse.Pawn, ThingWithComps> droppedWeapons = new Dictionary<Verse.Pawn, ThingWithComps>();

        [HarmonyPatch(typeof(Pawn_HealthTracker), "MakeDowned")]
        public static class Patch_MakeDowned
        {
            public static void Prefix(Pawn_HealthTracker __instance)
            {
                if (MegaMod.Settings?.QoL?.whereIsMyWeapon != true) return;

                Verse.Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Verse.Pawn>();
                if (pawn != null && pawn.IsColonist && pawn.equipment?.Primary != null)
                {
                    droppedWeapons[pawn] = pawn.equipment.Primary;
                }
            }
        }

        [HarmonyPatch(typeof(Pawn_HealthTracker), "MakeUndowned")]
        public static class Patch_MakeUndowned
        {
            public static void Postfix(Pawn_HealthTracker __instance)
            {
                if (MegaMod.Settings?.QoL?.whereIsMyWeapon != true) return;

                Verse.Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Verse.Pawn>();
                if (pawn == null || !pawn.IsColonist || !pawn.Spawned || pawn.Dead) return;

                if (pawn.health?.capacities == null || !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
                {
                    droppedWeapons.Remove(pawn);
                    return;
                }

                if (droppedWeapons.TryGetValue(pawn, out ThingWithComps weapon))
                {
                    droppedWeapons.Remove(pawn);

                    if (weapon != null && !weapon.Destroyed && weapon.Spawned && weapon.Map == pawn.Map && pawn.equipment?.Primary == null)
                    {
                        if (pawn.CanReach(weapon, PathEndMode.ClosestTouch, Danger.Some))
                        {
                            Job job = JobMaker.MakeJob(JobDefOf.Equip, weapon);
                            pawn.jobs?.jobQueue?.EnqueueFirst(job);
                        }
                    }
                }
            }
        }
    }
}
