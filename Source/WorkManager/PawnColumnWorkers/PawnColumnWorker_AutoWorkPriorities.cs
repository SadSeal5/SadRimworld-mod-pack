using RimWorld;
using SadSeal.MegaMod.WorkManager.UI;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.WorkManager.PawnColumnWorkers
{
    public class PawnColumnWorker_AutoWorkPriorities : PawnColumnWorker
    {
        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            if (pawn == null || pawn.Dead || pawn.workSettings == null || !pawn.workSettings.EverWork)
            {
                return;
            }

            var comp = WorkManagerGameComponent.Instance;
            if (comp == null) return;

            bool enabled = comp.IsPawnPriorityEnabled(pawn);
            Texture2D tex = enabled ? WorkManagerTextures.PawnOn : WorkManagerTextures.PawnOff;

            Rect btnRect = new Rect(rect.x + (rect.width - 18f) * 0.5f, rect.y + (rect.height - 18f) * 0.5f, 18f, 18f);

            if (Widgets.ButtonImage(btnRect, tex))
            {
                comp.TogglePawnPriority(pawn);
            }

            string tip = enabled
                ? "WorkManager_PawnPriorityEnabledTip".Translate(pawn.LabelShort)
                : "WorkManager_PawnPriorityDisabledTip".Translate(pawn.LabelShort);
            TooltipHandler.TipRegion(btnRect, tip);
        }

        public override int GetMinWidth(PawnTable table) => 24;
        public override int GetOptimalWidth(PawnTable table) => 24;

        public override void DoHeader(Rect rect, PawnTable table)
        {
            base.DoHeader(rect, table);
            GUI.color = Color.white;
            Rect iconRect = new Rect(rect.x + (rect.width - 16f) * 0.5f, rect.y + (rect.height - 16f) * 0.5f, 16f, 16f);
            GUI.DrawTexture(iconRect, WorkManagerTextures.GlobalOn);
            TooltipHandler.TipRegion(rect, "WorkManager_ColumnPriorityHeaderTip".Translate());
        }
    }
}
