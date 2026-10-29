using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.AllowTool.Area
{
    public class Designator_StripArea : Verse.Designator
    {
        public override DrawStyleCategoryDef DrawStyleCategory => DrawStyleCategoryDefOf.FilledRectangle;

        public Designator_StripArea()
        {
            defaultLabel = "MegaMod_Designator_StripArea".Translate();
            defaultDesc = "MegaMod_Designator_StripArea_Desc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/Designators/StripArea", false)
                ?? ContentFinder<Texture2D>.Get("UI/Designators/Strip", false)
                ?? BaseContent.WhiteTex;
            useMouseIcon = true;
            soundDragSustain = SoundDefOf.Designate_DragStandard;
            soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            soundSucceeded = SoundDefOf.Designate_Claim;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 loc)
        {
            if (!loc.InBounds(Map) || loc.Fogged(Map)) return false;

            List<Thing> thingList = loc.GetThingList(Map);
            for (int i = 0; i < thingList.Count; i++)
            {
                if (StripUtility.CanStrip(thingList[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public override void DesignateSingleCell(IntVec3 loc)
        {
            if (!loc.InBounds(Map) || loc.Fogged(Map)) return;

            List<Thing> thingList = loc.GetThingList(Map);
            for (int i = 0; i < thingList.Count; i++)
            {
                Thing t = thingList[i];
                if (StripUtility.CanStrip(t))
                {
                    StripUtility.Strip(t);
                }
            }
        }

        public override AcceptanceReport CanDesignateThing(Thing t)
        {
            return StripUtility.CanStrip(t);
        }

        public override void DesignateThing(Thing t)
        {
            if (StripUtility.CanStrip(t))
            {
                StripUtility.Strip(t);
            }
        }
    }
}
