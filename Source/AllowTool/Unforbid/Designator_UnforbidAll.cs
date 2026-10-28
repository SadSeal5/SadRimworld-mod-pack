using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SadSeal.MegaMod.AllowTool.Unforbid
{
    public class Designator_UnforbidAll : Designator
    {
        public Designator_UnforbidAll()
        {
            defaultLabel = "MegaMod_UnforbidAllOnMap".Translate();
            defaultDesc = "MegaMod_Designator_UnforbidAll_Desc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/Designators/ForbidOff", false) ?? BaseContent.WhiteTex;
            soundSucceeded = SoundDefOf.Designate_Claim;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 loc) => AcceptanceReport.WasAccepted;

        public override void ProcessInput(Event ev)
        {
            if (CheckCanInteract())
            {
                Designator_Unforbid.UnforbidAllOnMap(Map);
                SoundDefOf.Designate_Claim.PlayOneShotOnCamera(Map);
            }
        }
    }
}
