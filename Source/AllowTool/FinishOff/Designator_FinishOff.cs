using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.AllowTool.FinishOff
{
    public class Designator_FinishOff : Designator_Cells
    {
        private static DesignationDef cachedDesignationDef;

        public static DesignationDef FinishOffDef =>
            cachedDesignationDef ??= DefDatabase<DesignationDef>.GetNamedSilentFail("Sad_Designation_FinishOff");

        public Designator_FinishOff()
        {
            defaultLabel = "MegaMod_Designator_FinishOff".Translate();
            defaultDesc = "MegaMod_Designator_FinishOff_Desc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/Designators/FinishOff", false) ?? ContentFinder<Texture2D>.Get("UI/Commands/FinishOff", false) ?? BaseContent.WhiteTex;
            useMouseIcon = true;
            soundDragSustain = SoundDefOf.Designate_DragStandard;
            soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            soundSucceeded = SoundDefOf.Designate_Hunt;
        }

        public static bool CanFinishOffPawn(Pawn p)
        {
            if (p == null || !p.Spawned || p.Dead || !p.Downed)
            {
                return false;
            }

            if (p.Faction == Faction.OfPlayer || p.IsSlaveOfColony)
            {
                return false;
            }

            if (p.Faction != null)
            {
                if (!p.Faction.HostileTo(Faction.OfPlayer))
                {
                    return false;
                }
            }
            else if (p.RaceProps?.Humanlike == true)
            {
                if (!p.HostileTo(Faction.OfPlayer) && !p.InAggroMentalState)
                {
                    return false;
                }
            }

            return true;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 loc)
        {
            if (Map == null || !loc.InBounds(Map) || loc.Fogged(Map)) return false;

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
            if (Map == null || !(t is Pawn p) || p.Map != Map || p.Position.Fogged(Map))
            {
                return false;
            }

            if (!CanFinishOffPawn(p))
            {
                return false;
            }

            if (FinishOffDef != null && Map.designationManager.DesignationOn(p, FinishOffDef) != null)
            {
                return false;
            }

            return true;
        }

        public override void DesignateSingleCell(IntVec3 loc)
        {
            if (Map == null || !loc.InBounds(Map) || loc.Fogged(Map)) return;

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
            if (FinishOffDef == null || t == null || Map?.designationManager == null) return;
            Map.designationManager.AddDesignation(new Designation(t, FinishOffDef));
        }
    }
}
