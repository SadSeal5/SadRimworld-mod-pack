using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SadSeal.MegaMod.Genes.Jobs
{
    public class WorkGiver_SynthesizeGene : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                if (ThingDefOf.GeneAssembler != null)
                {
                    return ThingRequest.ForDef(ThingDefOf.GeneAssembler);
                }
                return ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial);
            }
        }

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!ModsConfig.BiotechActive || GenesModule.Instance?.IsEnabled != true) return false;
            if (t == null || pawn == null || t.IsForbidden(pawn)) return false;
            if (t.Faction != pawn.Faction && t.Faction != Faction.OfPlayer)
            {
                return false;
            }

            CompGeneSynthesizer comp = t.TryGetComp<CompGeneSynthesizer>();
            if (comp == null || !comp.IsSynthesizing || comp.CurrentOrder == null)
            {
                return false;
            }

            CompPowerTrader power = t.TryGetComp<CompPowerTrader>();
            if (power != null && !power.PowerOn)
            {
                JobFailReason.Is("NoPower".Translate());
                return false;
            }

            if (!pawn.CanReserve(t, 1, -1, null, forced))
            {
                return false;
            }

            if (comp.NeutroamineNeeded > 0)
            {
                Thing neutro = FindResource(pawn, CompGeneSynthesizer.NeutroamineDef, forced);
                if (neutro == null)
                {
                    JobFailReason.Is("GeneSynthesis_NoNeutroamine".Translate());
                    return false;
                }
                return true;
            }

            if (comp.ArchiteNeeded > 0)
            {
                Thing archite = FindResource(pawn, CompGeneSynthesizer.ArchiteDef, forced);
                if (archite == null)
                {
                    JobFailReason.Is("GeneSynthesis_RequiresArchiteCapsule".Translate(comp.ArchiteNeeded));
                    return false;
                }
                return true;
            }

            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!ModsConfig.BiotechActive || GenesModule.Instance?.IsEnabled != true) return null;
            if (t == null || pawn == null || t.IsForbidden(pawn)) return null;

            CompGeneSynthesizer comp = t.TryGetComp<CompGeneSynthesizer>();
            if (comp == null || !comp.IsSynthesizing || comp.CurrentOrder == null)
            {
                return null;
            }

            if (comp.NeutroamineNeeded > 0)
            {
                return CreateHaulJob(pawn, t, CompGeneSynthesizer.NeutroamineDef, comp.NeutroamineNeeded, forced);
            }

            if (comp.ArchiteNeeded > 0)
            {
                return CreateHaulJob(pawn, t, CompGeneSynthesizer.ArchiteDef, comp.ArchiteNeeded, forced);
            }

            return JobMaker.MakeJob(GeneJobDefOf.SynthesizeGene, t);
        }

        private Job CreateHaulJob(Pawn pawn, Thing assembler, ThingDef resourceDef, int needed, bool forced)
        {
            Thing found = FindResource(pawn, resourceDef, forced);
            if (found == null)
            {
                return null;
            }

            int maxReservable = pawn.Map?.reservationManager?.CanReserveStack(pawn, found, 10, null, forced) ?? found.stackCount;
            int count = Mathf.Min(needed, Mathf.Min(found.stackCount, maxReservable));
            if (count <= 0)
            {
                return null;
            }

            Job haulJob = JobMaker.MakeJob(GeneJobDefOf.HaulNeutroamineToGeneAssembler, assembler, found);
            haulJob.count = count;
            return haulJob;
        }

        private Thing FindResource(Pawn pawn, ThingDef resourceDef, bool forced)
        {
            if (pawn == null || pawn.Map == null || resourceDef == null)
            {
                return null;
            }

            return GenClosest.ClosestThingReachable(
                pawn.Position,
                pawn.Map,
                ThingRequest.ForDef(resourceDef),
                PathEndMode.ClosestTouch,
                TraverseParms.For(pawn),
                9999f,
                item => !item.IsForbidden(pawn) && pawn.CanReserve(item, 10, 1, null, forced)
            );
        }
    }
}
