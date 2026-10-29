using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.AllowTool.Area
{
    public class Designator_CancelArea : Verse.Designator
    {
        public override DrawStyleCategoryDef DrawStyleCategory => DrawStyleCategoryDefOf.FilledRectangle;

        public Designator_CancelArea()
        {
            defaultLabel = "MegaMod_Designator_CancelArea".Translate();
            defaultDesc = "MegaMod_Designator_CancelArea_Desc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/Designators/CancelArea", false)
                ?? ContentFinder<Texture2D>.Get("UI/Designators/Cancel", false)
                ?? BaseContent.WhiteTex;
            useMouseIcon = true;
            soundDragSustain = SoundDefOf.Designate_DragStandard;
            soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            soundSucceeded = SoundDefOf.Designate_Cancel;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 loc)
        {
            if (!loc.InBounds(Map)) return false;

            if (Map.designationManager.AllDesignationsAt(loc).Any())
            {
                return true;
            }

            List<Thing> thingList = loc.GetThingList(Map);
            for (int i = 0; i < thingList.Count; i++)
            {
                Thing t = thingList[i];
                if (CanDesignateThing(t).Accepted)
                {
                    return true;
                }
            }

            return false;
        }

        public override void DesignateSingleCell(IntVec3 loc)
        {
            if (!loc.InBounds(Map)) return;

            var cellDesList = Map.designationManager.AllDesignationsAt(loc).ToList();
            for (int i = 0; i < cellDesList.Count; i++)
            {
                cellDesList[i].Delete();
            }

            List<Thing> thingList = loc.GetThingList(Map).ToList();
            for (int i = 0; i < thingList.Count; i++)
            {
                Thing t = thingList[i];
                if (CanDesignateThing(t).Accepted)
                {
                    DesignateThing(t);
                }
            }
        }

        public override AcceptanceReport CanDesignateThing(Thing t)
        {
            if (t == null || !t.Spawned || t.Map != Map) return false;

            if (Map.designationManager.AllDesignationsOn(t).Any())
            {
                return true;
            }

            if (t.Faction == Faction.OfPlayer && (t is Frame || t is Blueprint))
            {
                return true;
            }

            return false;
        }

        public override void DesignateThing(Thing t)
        {
            if (t == null || !t.Spawned) return;

            var thingDesList = Map.designationManager.AllDesignationsOn(t).ToList();
            for (int i = 0; i < thingDesList.Count; i++)
            {
                thingDesList[i].Delete();
            }

            if (t.Faction == Faction.OfPlayer)
            {
                if (t is Frame frame)
                {
                    frame.Destroy(DestroyMode.Cancel);
                }
                else if (t is Blueprint bp)
                {
                    bp.Destroy(DestroyMode.Cancel);
                }
            }
        }
    }
}
