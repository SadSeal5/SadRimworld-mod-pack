using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.QoL.Patches.Pawn
{
    public class WhereIsMyWeaponGameComponent : GameComponent
    {
        public static WhereIsMyWeaponGameComponent Instance { get; private set; }

        private Dictionary<Verse.Pawn, ThingWithComps> droppedWeapons = new Dictionary<Verse.Pawn, ThingWithComps>();

        public Dictionary<Verse.Pawn, ThingWithComps> DroppedWeapons => droppedWeapons;

        public WhereIsMyWeaponGameComponent(Game game)
        {
            Instance = this;
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            Instance = this;
        }

        public static WhereIsMyWeaponGameComponent GetInstance()
        {
            return Instance ?? Current.Game?.GetComponent<WhereIsMyWeaponGameComponent>();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref droppedWeapons, "droppedWeapons", LookMode.Reference, LookMode.Reference);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (droppedWeapons == null)
                {
                    droppedWeapons = new Dictionary<Verse.Pawn, ThingWithComps>();
                }
                else
                {
                    var keysToRemove = new List<Verse.Pawn>();
                    foreach (var kvp in droppedWeapons)
                    {
                        if (kvp.Key == null || kvp.Value == null || kvp.Value.Destroyed)
                        {
                            keysToRemove.Add(kvp.Key);
                        }
                    }
                    for (int i = 0; i < keysToRemove.Count; i++)
                    {
                        droppedWeapons.Remove(keysToRemove[i]);
                    }
                }
            }
        }
    }

    public static class Patch_WhereIsMyWeapon
    {
        [HarmonyPatch(typeof(Pawn_HealthTracker), "MakeDowned")]
        public static class Patch_MakeDowned
        {
            public static void Prefix(Pawn_HealthTracker __instance)
            {
                if (MegaMod.Settings?.QoL?.whereIsMyWeapon != true) return;

                Verse.Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Verse.Pawn>();
                if (pawn != null && pawn.IsColonist && pawn.equipment?.Primary != null)
                {
                    var comp = WhereIsMyWeaponGameComponent.GetInstance();
                    if (comp != null)
                    {
                        comp.DroppedWeapons[pawn] = pawn.equipment.Primary;
                    }
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

                var comp = WhereIsMyWeaponGameComponent.GetInstance();
                if (comp == null) return;

                if (pawn.health?.capacities == null || !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
                {
                    comp.DroppedWeapons.Remove(pawn);
                    return;
                }

                if (comp.DroppedWeapons.TryGetValue(pawn, out ThingWithComps weapon))
                {
                    comp.DroppedWeapons.Remove(pawn);

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
