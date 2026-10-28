using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.AllowTool.HaulUrgently
{
    public class Designator_HaulUrgently : Designator_Cells
    {
        private static DesignationDef cachedDesignationDef;

        public static DesignationDef HaulUrgentlyDef =>
            cachedDesignationDef ??= DefDatabase<DesignationDef>.GetNamedSilentFail("SadHaul_HaulUrgently");

        public Designator_HaulUrgently()
        {
            defaultLabel = "MegaMod_Designator_HaulUrgently".Translate();
            defaultDesc = "MegaMod_Designator_HaulUrgently_Desc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/Designators/Haul", false) ?? BaseContent.WhiteTex;
            useMouseIcon = true;
            soundDragSustain = SoundDefOf.Designate_DragStandard;
            soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            soundSucceeded = SoundDefOf.Designate_Haul;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 loc)
        {
            if (!loc.InBounds(Map)) return false;

            List<Thing> thingList = loc.GetThingList(Map);
            for (int i = 0; i < thingList.Count; i++)
            {
                if (CanDesignateThing(thingList[i]).Accepted)
                {
                    return true;
                }
            }

            return false;
        }

        public override AcceptanceReport CanDesignateThing(Thing t)
        {
            if (t == null || !t.Spawned || t.Map != Map) return false;
            if (!t.def.EverHaulable) return false;
            if (t.IsInValidStorage()) return false;
            if (HaulUrgentlyDef != null && Map.designationManager.DesignationOn(t, HaulUrgentlyDef) != null) return false;

            return true;
        }

        public override void DesignateSingleCell(IntVec3 loc)
        {
            List<Thing> thingList = loc.GetThingList(Map);
            for (int i = 0; i < thingList.Count; i++)
            {
                Thing t = thingList[i];
                if (CanDesignateThing(t).Accepted)
                {
                    DesignateThing(t);
                }
            }
        }

        public override void DesignateThing(Thing t)
        {
            if (HaulUrgentlyDef == null || t == null) return;
            Map.designationManager.AddDesignation(new Designation(t, HaulUrgentlyDef));
        }
    }
}
