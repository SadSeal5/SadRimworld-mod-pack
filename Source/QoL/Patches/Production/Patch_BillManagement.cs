using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.QoL.Patches.Production
{
    public static class BillClipboard
    {
        private static readonly List<Bill> clipboard = new List<Bill>();
        private static ThingDef sourceTableDef;

        public static bool HasCopiedBills => clipboard.Count > 0;

        public static void CopyBillsFrom(Building_WorkTable table)
        {
            clipboard.Clear();
            if (table == null || table.BillStack == null) return;
            sourceTableDef = table.def;

            foreach (Bill bill in table.BillStack.Bills)
            {
                clipboard.Add(bill.Clone());
            }
        }

        public static bool CanPasteTo(Building_WorkTable table)
        {
            if (!HasCopiedBills || table == null || table.def?.AllRecipes == null) return false;
            foreach (Bill bill in clipboard)
            {
                if (table.def.AllRecipes.Contains(bill.recipe))
                {
                    return true;
                }
            }
            return false;
        }

        public static int PasteBillsTo(Building_WorkTable table)
        {
            if (table?.BillStack == null || !HasCopiedBills) return 0;

            int count = 0;
            foreach (Bill bill in clipboard)
            {
                if (table.def.AllRecipes != null && table.def.AllRecipes.Contains(bill.recipe))
                {
                    Bill newBill = bill.Clone();
                    newBill.billStack = table.BillStack;
                    table.BillStack.AddBill(newBill);
                    count++;
                }
            }
            return count;
        }
    }

    // Kept for backward compatibility with existing save games that have this MapComponent serialized.
    // Automatically un-suspends previously auto-suspended bills on load and runs no background ticks.
    public class BillAutoSuspendManager : MapComponent
    {
        private List<Bill_Production> autoSuspendedBills = new List<Bill_Production>();

        public BillAutoSuspendManager(Map map) : base(map)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref autoSuspendedBills, "autoSuspendedBills", LookMode.Reference);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (autoSuspendedBills != null)
                {
                    for (int i = 0; i < autoSuspendedBills.Count; i++)
                    {
                        if (autoSuspendedBills[i] != null)
                        {
                            autoSuspendedBills[i].suspended = false;
                        }
                    }
                    autoSuspendedBills.Clear();
                }
            }
        }
    }

    [HarmonyPatch(typeof(Building), nameof(Building.GetGizmos))]
    public static class Patch_Building_WorkTable_BillClipboard
    {
        public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Building __instance)
        {
            foreach (Gizmo g in __result) yield return g;
            if (QoLModule.ModSettings?.enableBillQoL != true) yield break;
            if (!(__instance is Building_WorkTable workTable)) yield break;
            if (!workTable.Spawned || workTable.Faction != Faction.OfPlayer) yield break;

            // Gizmo: Copy bills from this workbench
            if (workTable.BillStack != null && workTable.BillStack.Count > 0)
            {
                yield return new Command_Action
                {
                    defaultLabel = "MegaMod_CopyBills".Translate(),
                    defaultDesc = "MegaMod_CopyBills_Desc".Translate(),
                    icon = TexButton.Copy ?? BaseContent.WhiteTex,
                    action = () =>
                    {
                        BillClipboard.CopyBillsFrom(workTable);
                        Messages.Message("MegaMod_BillsCopied".Translate(workTable.BillStack.Count, workTable.LabelShort),
                            MessageTypeDefOf.PositiveEvent, false);
                    }
                };
            }

            // Gizmo: Paste bills to this workbench
            if (BillClipboard.HasCopiedBills)
            {
                Command_Action pasteCommand = new Command_Action
                {
                    defaultLabel = "MegaMod_PasteBills".Translate(),
                    defaultDesc = "MegaMod_PasteBills_Desc".Translate(),
                    icon = TexButton.Paste ?? BaseContent.WhiteTex,
                    action = () =>
                    {
                        int pasted = BillClipboard.PasteBillsTo(workTable);
                        Messages.Message("MegaMod_BillsPasted".Translate(pasted, workTable.LabelShort),
                            MessageTypeDefOf.PositiveEvent, false);
                    }
                };

                if (!BillClipboard.CanPasteTo(workTable))
                {
                    pasteCommand.Disable("MegaMod_CannotPasteBills".Translate());
                }

                yield return pasteCommand;
            }
        }
    }
}
