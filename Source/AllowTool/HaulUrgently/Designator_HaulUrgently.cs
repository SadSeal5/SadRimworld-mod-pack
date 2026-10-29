using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.AllowTool.HaulUrgently
{
    public class Designator_HaulUrgently : Verse.Designator
    {
        private static DesignationDef cachedDesignationDef;

        public static DesignationDef HaulUrgentlyDef =>
            cachedDesignationDef ??= DefDatabase<DesignationDef>.GetNamedSilentFail("SadHaul_HaulUrgently");

        protected override DesignationDef Designation => HaulUrgentlyDef;

        public override DrawStyleCategoryDef DrawStyleCategory => DrawStyleCategoryDefOf.FilledRectangle;

        public Designator_HaulUrgently()
        {
            defaultLabel = "MegaMod_Designator_HaulUrgently".Translate();
            defaultDesc = "MegaMod_Designator_HaulUrgently_Desc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/Designators/HaulUrgently", false) ?? ContentFinder<Texture2D>.Get("UI/Commands/HaulUrgently", false) ?? BaseContent.WhiteTex;
            useMouseIcon = true;
            soundDragSustain = SoundDefOf.Designate_DragStandard;
            soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            soundSucceeded = SoundDefOf.Designate_Haul;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 loc)
        {
            if (!loc.InBounds(Map)) return false;

            if (FirstHaulableInCell(loc) == null)
            {
                return "MessageMustDesignateHaulable".Translate();
            }

            return true;
        }

        public override void DesignateSingleCell(IntVec3 loc)
        {
            Thing t = FirstHaulableInCell(loc);
            if (t != null)
            {
                DesignateThing(t);
            }
        }

        public override AcceptanceReport CanDesignateThing(Thing t)
        {
            if (t == null || !t.Spawned || t.Map != Map) return false;
            if (t.Faction != null && t.Faction != Faction.OfPlayer) return false;
            if (!t.def.EverHaulable && !t.def.designateHaulable) return false;
            if (t.IsInValidStorage()) return false;
            if (HaulUrgentlyDef != null && Map.designationManager.DesignationOn(t, HaulUrgentlyDef) != null) return false;

            return true;
        }

        public override void DesignateThing(Thing t)
        {
            if (HaulUrgentlyDef == null || t == null) return;
            Map.designationManager.AddDesignation(new Designation(t, HaulUrgentlyDef));
        }

        public override void SelectedUpdate()
        {
            GenUI.RenderMouseoverBracket();
        }

        private Thing FirstHaulableInCell(IntVec3 loc)
        {
            if (loc.Fogged(Map)) return null;

            Thing firstHaulable = loc.GetFirstHaulable(Map);
            if (firstHaulable != null && CanDesignateThing(firstHaulable).Accepted)
            {
                return firstHaulable;
            }

            List<Thing> thingList = loc.GetThingList(Map);
            for (int i = 0; i < thingList.Count; i++)
            {
                if (CanDesignateThing(thingList[i]).Accepted)
                {
                    return thingList[i];
                }
            }

            return null;
        }
    }
}
