using System;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.AllowTool.HaulUrgently
{
    [HarmonyPatch(typeof(Toils_Haul), nameof(Toils_Haul.PlaceHauledThingInCell))]
    public static class Patch_HaulCompleteDesignation
    {
        public static void Postfix(TargetIndex carrierInd, TargetIndex cellInd, bool flag, Toil __result)
        {
            if (__result == null) return;

            Action originalAction = __result.initAction;
            __result.initAction = () =>
            {
                originalAction?.Invoke();

                Pawn actor = __result.actor;
                if (actor?.Map?.designationManager == null) return;

                Thing carried = actor.carryTracker?.CarriedThing;
                DesignationDef def = Designator_HaulUrgently.HaulUrgentlyDef;

                if (carried != null && def != null)
                {
                    Designation des = actor.Map.designationManager.DesignationOn(carried, def);
                    if (des != null)
                    {
                        actor.Map.designationManager.RemoveDesignation(des);
                    }
                }
            };
        }
    }
}
