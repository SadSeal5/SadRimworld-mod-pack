using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.QoL.Patches.Pawn
{
    [HarmonyPatch(typeof(MentalState_SocialFighting), nameof(MentalState_SocialFighting.MentalStateTick))]
    public static class Patch_SocialFighting
    {
        public static void Postfix(MentalState_SocialFighting __instance)
        {
            if (MegaMod.Settings?.QoL?.ughYouGotMe != true) return;

            Verse.Pawn pawn = __instance.pawn;
            Verse.Pawn other = __instance.otherPawn;
            if (pawn == null || other == null) return;

            // Stop social fighting if either participant experiences severe pain (> 0.50f) or is incapacitated
            if (pawn.health?.hediffSet == null || other.health?.hediffSet == null) return;

            float pain1 = pawn.health.hediffSet.PainTotal;
            float pain2 = other.health.hediffSet.PainTotal;

            if (pain1 >= 0.50f || pain2 >= 0.50f || pawn.Downed || other.Downed || pawn.Dead || other.Dead)
            {
                __instance.RecoverFromState();

                if (PawnUtility.ShouldSendNotificationAbout(pawn) || PawnUtility.ShouldSendNotificationAbout(other))
                {
                    Messages.Message("MegaMod_QoLUghYouGotMeStopped".Translate(pawn.LabelShort, other.LabelShort), MessageTypeDefOf.NeutralEvent);
                }
            }
        }
    }
}
