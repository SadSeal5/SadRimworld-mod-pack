using System.Collections.Generic;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Hauling
{
    public static class HauledCargoTracker
    {
        private static readonly Dictionary<int, HashSet<int>> trackedCargo = new Dictionary<int, HashSet<int>>();
        private static readonly object syncLock = new object();

        public static void Register(Pawn pawn, Thing thing)
        {
            if (pawn == null || thing == null) return;

            lock (syncLock)
            {
                if (!trackedCargo.TryGetValue(pawn.thingIDNumber, out var set))
                {
                    set = new HashSet<int>();
                    trackedCargo[pawn.thingIDNumber] = set;
                }
                set.Add(thing.thingIDNumber);
            }
        }

        public static void Unregister(Pawn pawn, Thing thing)
        {
            if (pawn == null || thing == null) return;

            lock (syncLock)
            {
                if (trackedCargo.TryGetValue(pawn.thingIDNumber, out var set))
                {
                    set.Remove(thing.thingIDNumber);
                    if (set.Count == 0)
                    {
                        trackedCargo.Remove(pawn.thingIDNumber);
                    }
                }
            }
        }

        public static bool IsHauled(Pawn pawn, Thing thing)
        {
            if (pawn == null || thing == null) return false;

            lock (syncLock)
            {
                if (trackedCargo.TryGetValue(pawn.thingIDNumber, out var set))
                {
                    return set.Contains(thing.thingIDNumber);
                }
                return false;
            }
        }

        public static List<Thing> GetHauledThings(Pawn pawn)
        {
            var results = new List<Thing>();
            if (pawn?.inventory?.innerContainer == null) return results;

            lock (syncLock)
            {
                if (trackedCargo.TryGetValue(pawn.thingIDNumber, out var set) && set.Count > 0)
                {
                    var container = pawn.inventory.innerContainer;
                    for (int i = 0; i < container.Count; i++)
                    {
                        Thing item = container[i];
                        if (item != null && set.Contains(item.thingIDNumber))
                        {
                            results.Add(item);
                        }
                    }
                }
            }

            return results;
        }

        public static void Clear(Pawn pawn)
        {
            if (pawn == null) return;

            lock (syncLock)
            {
                trackedCargo.Remove(pawn.thingIDNumber);
            }
        }

        public static void DropAllHauled(Pawn pawn, bool forbidden = false)
        {
            if (pawn == null) return;

            List<Thing> toDrop = GetHauledThings(pawn);
            if (toDrop.Count == 0)
            {
                Clear(pawn);
                return;
            }

            Map map = pawn.MapHeld ?? pawn.Map;
            IntVec3 dropPos = pawn.PositionHeld.IsValid ? pawn.PositionHeld : pawn.Position;

            if (pawn.inventory?.innerContainer != null && map != null && dropPos.IsValid)
            {
                for (int i = 0; i < toDrop.Count; i++)
                {
                    Thing item = toDrop[i];
                    if (item != null && !item.Destroyed && pawn.inventory.innerContainer.Contains(item))
                    {
                        if (pawn.inventory.innerContainer.TryDrop(item, dropPos, map, ThingPlaceMode.Near, out Thing dropped))
                        {
                            if (dropped != null)
                            {
                                dropped.SetForbidden(forbidden, false);
                            }
                        }
                    }
                }
            }

            Clear(pawn);
        }
    }
}
