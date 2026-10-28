using System;
using System.Collections.Generic;
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
                Pawn actor = __result.actor;
                // Capture carried thing BEFORE originalAction drops it and nulls CarriedThing
                Thing carried = actor?.carryTracker?.CarriedThing;
                DesignationDef def = Designator_HaulUrgently.HaulUrgentlyDef;

                originalAction?.Invoke();

                if (actor?.Map?.designationManager == null || def == null) return;

                if (carried != null)
                {
                    Designation des = actor.Map.designationManager.DesignationOn(carried, def);
                    if (des != null)
                    {
                        actor.Map.designationManager.RemoveDesignation(des);
                    }
                }

                // Also check target cell for any item with urgent haul designation that was just deposited into storage
                if (cellInd != TargetIndex.None && actor.CurJob != null)
                {
                    IntVec3 cell = actor.CurJob.GetTarget(cellInd).Cell;
                    if (cell.IsValid && cell.InBounds(actor.Map))
                    {
                        List<Thing> thingsInCell = cell.GetThingList(actor.Map);
                        for (int i = 0; i < thingsInCell.Count; i++)
                        {
                            Thing t = thingsInCell[i];
                            if (t.IsInValidStorage())
                            {
                                Designation des = actor.Map.designationManager.DesignationOn(t, def);
                                if (des != null)
                                {
                                    actor.Map.designationManager.RemoveDesignation(des);
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
