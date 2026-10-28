using System;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.QoL.Patches.Work
{
    [HarmonyPatch(typeof(WorkGiver_DoBill), "ClosestUnfinishedThingForBill")]
    public static class Patch_NoJobAuthors
    {
        public static bool Prefix(Verse.Pawn pawn, Bill_ProductionWithUft bill, ref UnfinishedThing __result)
        {
            if (MegaMod.Settings?.QoL?.noJobAuthors != true) return true;
            if (pawn == null || bill == null || pawn.Map == null) return true;

            Predicate<Thing> validator = (Thing t) =>
            {
                if (t.IsForbidden(pawn)) return false;
                if (!pawn.CanReserve(t)) return false;
                if (!(t is UnfinishedThing uft)) return false;
                if (uft.Recipe != bill.recipe) return false;
                if (uft.BoundBill != bill) return false;

                // Any qualified colonist can continue the craft without author restriction
                return true;
            };

            Thing found = GenClosest.ClosestThingReachable(
                pawn.Position,
                pawn.Map,
                ThingRequest.ForGroup(ThingRequestGroup.HaulableAlways),
                PathEndMode.InteractionCell,
                TraverseParms.For(pawn, Danger.Some),
                9999f,
                validator);

            if (found != null)
            {
                __result = (UnfinishedThing)found;
                return false;
            }

            return true;
        }
    }
}
