using System.Collections.Generic;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.AllowTool.Area
{
    public static class StripUtility
    {
        public static bool CanStrip(Thing t)
        {
            if (t == null || !t.Spawned) return false;
            if (t.Position.Fogged(t.Map)) return false;

            if (t is Corpse corpse)
            {
                if (corpse.Map.designationManager.DesignationOn(corpse, DesignationDefOf.Strip) != null) return false;
                return StrippableUtility.CanBeStrippedByColony(corpse);
            }

            if (t is Pawn pawn)
            {
                if (!pawn.Downed || pawn.Dead) return false;
                if (pawn.Faction == Faction.OfPlayer) return false;
                if (pawn.Map.designationManager.DesignationOn(pawn, DesignationDefOf.Strip) != null) return false;
                return StrippableUtility.CanBeStrippedByColony(pawn);
            }

            return false;
        }

        public static void Strip(Thing t)
        {
            if (t == null || !t.Spawned) return;
            if (t.Map.designationManager.DesignationOn(t, DesignationDefOf.Strip) == null)
            {
                t.Map.designationManager.AddDesignation(new Designation(t, DesignationDefOf.Strip));
            }
        }
    }
}
