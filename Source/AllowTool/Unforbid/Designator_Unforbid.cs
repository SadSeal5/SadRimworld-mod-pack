using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.AllowTool.Unforbid
{
    public class Designator_Unforbid : Designator_Cells
    {
        public Designator_Unforbid()
        {
            defaultLabel = "MegaMod_Designator_Unforbid".Translate();
            defaultDesc = "MegaMod_Designator_Unforbid_Desc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/Designators/ForbidOff", false) ?? BaseContent.WhiteTex;
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
                Thing t = thingList[i];
                if (t.def.category == ThingCategory.Item && t.IsForbidden(Faction.OfPlayer))
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
                if (t.def.category == ThingCategory.Item && t.IsForbidden(Faction.OfPlayer))
                {
                    ForbidUtility.SetForbidden(t, false);
                }
            }
        }

        public override AcceptanceReport CanDesignateThing(Thing t)
        {
            if (t != null && t.Spawned && t.Map == Map && !t.Position.Fogged(Map) &&
                t.def.category == ThingCategory.Item && t.IsForbidden(Faction.OfPlayer))
            {
                return true;
            }
            return false;
        }

        public override void DesignateThing(Thing t)
        {
            if (t != null)
            {
                ForbidUtility.SetForbidden(t, false);
            }
        }

        public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions
        {
            get
            {
                yield return new FloatMenuOption("MegaMod_UnforbidAllOnMap".Translate(), () => UnforbidAllOnMap(Map));
            }
        }

        public static void UnforbidAllOnMap(Map map)
        {
            if (map == null) return;

            int count = 0;
            List<Thing> allThings = map.listerThings.AllThings;
            for (int i = 0; i < allThings.Count; i++)
            {
                Thing t = allThings[i];
                // Exclude fogged items to prevent revealing unrevealed ancient dangers
                if (t.Spawned && !t.Position.Fogged(map) && t.def.category == ThingCategory.Item && t.IsForbidden(Faction.OfPlayer))
                {
                    ForbidUtility.SetForbidden(t, false);
                    count++;
                }
            }

            Messages.Message("MegaMod_UnforbiddenMapCount".Translate(count), MessageTypeDefOf.PositiveEvent, false);
        }
    }
}
