using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.QoL.Patches.Pawn
{
    public static class SnapOutUtility
    {
        public static readonly Dictionary<int, int> CalmDownCooldowns = new Dictionary<int, int>();

        public static bool CanCalmDown(Verse.Pawn calmer, Verse.Pawn target)
        {
            if (QoLModule.ModSettings?.enableSnapOut != true) return false;
            if (calmer == null || target == null || calmer == target) return false;
            if (!calmer.Spawned || !target.Spawned) return false;
            if (target.Faction != Faction.OfPlayer) return false;
            if (!target.InMentalState) return false;

            // Only calm non-violent mental breaks
            if (target.MentalState == null || target.MentalState.def == null || target.MentalState.def.IsAggro) return false;

            // Check cooldown
            if (CalmDownCooldowns.TryGetValue(target.thingIDNumber, out int expireTick))
            {
                if (Find.TickManager.TicksGame < expireTick) return false;
            }

            return true;
        }

        public static void SetCooldown(Verse.Pawn target)
        {
            if (target == null) return;
            CalmDownCooldowns[target.thingIDNumber] = Find.TickManager.TicksGame + 2500; // 1 hour cooldown between attempts
        }
    }

    public class JobDriver_CalmDown : JobDriver
    {
        private const int CalmingDuration = 180;

        protected Verse.Pawn TargetPawn => job.targetA.Thing as Verse.Pawn;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            this.FailOn(() =>
            {
                Verse.Pawn target = TargetPawn;
                return target == null || !target.InMentalState || (target.MentalState?.def != null && target.MentalState.def.IsAggro);
            });

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            Toil talkToil = ToilMaker.MakeToil("CalmDownToil");
            talkToil.defaultCompleteMode = ToilCompleteMode.Delay;
            talkToil.defaultDuration = CalmingDuration;
            talkToil.handlingFacing = true;

            talkToil.tickAction = () =>
            {
                Verse.Pawn target = TargetPawn;
                if (target == null) return;

                if (pawn.Position.DistanceToSquared(target.Position) > 25)
                {
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                pawn.rotationTracker.FaceCell(target.Position);

                // Show occasional speech bubbles
                if (pawn.IsHashIntervalTick(45))
                {
                    MoteMaker.MakeInteractionBubble(pawn, target, InteractionDefOf.Chitchat.interactionMote, InteractionDefOf.Chitchat.GetSymbol(null, null));
                }
            };

            talkToil.AddFinishAction(() =>
            {
                Verse.Pawn target = TargetPawn;
                if (target == null || !target.InMentalState) return;

                // Success chance based on Social skill and opinion
                int socialLevel = pawn.skills?.GetSkill(SkillDefOf.Social)?.Level ?? 5;
                float opinion = target.relations?.OpinionOf(pawn) ?? 0f;
                float chance = Mathf.Clamp(0.40f + (socialLevel * 0.035f) + (opinion * 0.0025f), 0.20f, 0.95f);

                SnapOutUtility.SetCooldown(target);

                if (Rand.Value < chance)
                {
                    target.MentalState?.RecoverFromState();
                    Messages.Message("MegaMod_SnapOut_Success".Translate(pawn.LabelShort, target.LabelShort), target, MessageTypeDefOf.PositiveEvent);
                    target.needs?.mood?.thoughts?.memories?.TryGainMemory(ThoughtDefOf.Catharsis);
                }
                else
                {
                    Messages.Message("MegaMod_SnapOut_Failed".Translate(pawn.LabelShort, target.LabelShort), target, MessageTypeDefOf.NeutralEvent);
                }
            });

            yield return talkToil;
        }
    }

    public class WorkGiver_CalmDown : WorkGiver_Scanner
    {
        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Verse.Pawn pawn)
        {
            if (QoLModule.ModSettings?.enableSnapOut != true) yield break;
            if (pawn == null || pawn.Map == null) yield break;
            foreach (Verse.Pawn p in pawn.Map.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
            {
                if (p != pawn && p.InMentalState)
                {
                    yield return p;
                }
            }
        }

        public override bool HasJobOnThing(Verse.Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t is Verse.Pawn targetPawn)) return false;
            if (!SnapOutUtility.CanCalmDown(pawn, targetPawn)) return false;
            if (!pawn.CanReserve(targetPawn, 1, -1, null, forced)) return false;
            if (!pawn.CanReach(targetPawn, PathEndMode.Touch, Danger.Some)) return false;

            return true;
        }

        public override Job JobOnThing(Verse.Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t is Verse.Pawn targetPawn)) return null;
            return JobMaker.MakeJob(SadQoLDefOf.SadQoL_CalmDown, targetPawn);
        }
    }

    [HarmonyPatch]
    public static class Patch_FloatMenuMakerMap_SnapOut
    {
        public static bool Prepare() => AccessTools.Method(typeof(FloatMenuMakerMap), "AddHumanlikeOrders") != null;
        public static MethodBase TargetMethod() => AccessTools.Method(typeof(FloatMenuMakerMap), "AddHumanlikeOrders");

        public static void Postfix(Vector3 clickPos, Verse.Pawn pawn, List<FloatMenuOption> opts)
        {
            if (QoLModule.ModSettings?.enableSnapOut != true) return;
            if (pawn == null || !pawn.IsColonistPlayerControlled) return;

            IntVec3 cell = IntVec3.FromVector3(clickPos);
            Map map = pawn.Map;
            if (map == null || !cell.InBounds(map)) return;

            List<Thing> thingList = cell.GetThingList(map);
            for (int i = 0; i < thingList.Count; i++)
            {
                if (thingList[i] is Verse.Pawn targetPawn && targetPawn != pawn)
                {
                    if (SnapOutUtility.CanCalmDown(pawn, targetPawn))
                    {
                        opts.Add(new FloatMenuOption("MegaMod_SnapOut_Option".Translate(targetPawn.LabelShort), () =>
                        {
                            Job job = JobMaker.MakeJob(SadQoLDefOf.SadQoL_CalmDown, targetPawn);
                            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                        }));
                    }
                }
            }
        }
    }
}
