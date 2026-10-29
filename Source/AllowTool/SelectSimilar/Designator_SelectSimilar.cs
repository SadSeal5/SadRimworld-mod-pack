using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.AllowTool.SelectSimilar
{
    public class Designator_SelectSimilar : Designator_Cells
    {
        public static bool SelectEntireMap = false;

        public Designator_SelectSimilar()
        {
            defaultLabel = "MegaMod_Designator_SelectSimilar".Translate();
            defaultDesc = "MegaMod_Designator_SelectSimilar_Desc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/Commands/SelectSimilar", false) ?? ContentFinder<Texture2D>.Get("UI/Designators/SelectSimilar", false) ?? BaseContent.WhiteTex;
            useMouseIcon = true;
            soundSucceeded = SoundDefOf.Click;
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
                    break;
                }
            }
        }

        public override void DesignateThing(Thing t)
        {
            if (t == null) return;
            SelectAllSimilar(t, SelectEntireMap);
        }

        public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions
        {
            get
            {
                yield return new FloatMenuOption("MegaMod_SelectSimilar_Screen".Translate(), () => SelectEntireMap = false);
                yield return new FloatMenuOption("MegaMod_SelectSimilar_Map".Translate(), () => SelectEntireMap = true);
            }
        }

        public static void SelectAllSimilar(Thing target, bool entireMap)
        {
            if (target == null || target.Map == null) return;

            Map map = target.Map;
            CellRect viewRect = Find.CameraDriver.CurrentViewRect;
            List<Thing> allThings = map.listerThings.AllThings;
            int count = 0;

            for (int i = 0; i < allThings.Count; i++)
            {
                Thing th = allThings[i];
                if (!th.Spawned || th.Destroyed) continue;

                if (!entireMap && !viewRect.Contains(th.Position)) continue;

                if (th.Position.Fogged(map)) continue;

                if (th.def == target.def && th.Stuff == target.Stuff)
                {
                    Find.Selector.Select(th, playSound: false, forceDesignatorDeselect: false);
                    count++;
                }
            }

            Messages.Message("MegaMod_SelectedSimilarCount".Translate(count, target.LabelNoCount), MessageTypeDefOf.PositiveEvent, false);
        }
    }
}
