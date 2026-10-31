using System;
using System.Collections.Generic;
using RimWorld;
using SadSeal.MegaMod.WorkManager.Cache;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.WorkManager.Core
{
    public class WorkPriorityManager : MapComponent
    {
        private struct PawnScoreEntry
        {
            public Pawn Pawn;
            public float Score;
        }

        private static readonly List<Pawn> capablePawns = new List<Pawn>(32);
        private static readonly List<PawnScoreEntry> candidateScores = new List<PawnScoreEntry>(32);
        private static readonly Comparison<PawnScoreEntry> ScoreComparison = (a, b) => b.Score.CompareTo(a.Score);

        private static WorkTypeDef firefighterDef;
        public static WorkTypeDef FirefighterDef => firefighterDef ??= DefDatabase<WorkTypeDef>.GetNamedSilentFail("Firefighter");

        private static WorkTypeDef patientDef;
        public static WorkTypeDef PatientDef => patientDef ??= DefDatabase<WorkTypeDef>.GetNamedSilentFail("Patient");

        private static WorkTypeDef patientBedRestDef;
        public static WorkTypeDef PatientBedRestDef => patientBedRestDef ??= DefDatabase<WorkTypeDef>.GetNamedSilentFail("PatientBedRest");

        private static WorkTypeDef basicWorkerDef;
        public static WorkTypeDef BasicWorkerDef => basicWorkerDef ??= DefDatabase<WorkTypeDef>.GetNamedSilentFail("BasicWorker");

        private static WorkTypeDef doctorDef;
        public static WorkTypeDef DoctorDef => doctorDef ??= DefDatabase<WorkTypeDef>.GetNamedSilentFail("Doctor");

        private static WorkTypeDef cookingDef;
        public static WorkTypeDef CookingDef => cookingDef ??= DefDatabase<WorkTypeDef>.GetNamedSilentFail("Cooking");

        private static WorkTypeDef researchDef;
        public static WorkTypeDef ResearchDef => researchDef ??= DefDatabase<WorkTypeDef>.GetNamedSilentFail("Research");

        private static WorkTypeDef wardenDef;
        public static WorkTypeDef WardenDef => wardenDef ??= DefDatabase<WorkTypeDef>.GetNamedSilentFail("Warden");

        private static WorkTypeDef handlingDef;
        public static WorkTypeDef HandlingDef => handlingDef ??= DefDatabase<WorkTypeDef>.GetNamedSilentFail("Handling");

        private static WorkTypeDef haulingDef;
        public static WorkTypeDef HaulingDef => haulingDef ??= DefDatabase<WorkTypeDef>.GetNamedSilentFail("Hauling");

        private static WorkTypeDef cleaningDef;
        public static WorkTypeDef CleaningDef => cleaningDef ??= DefDatabase<WorkTypeDef>.GetNamedSilentFail("Cleaning");

        private bool isDirty = true;
        public void MarkDirty() => isDirty = true;

        public WorkPriorityManager(Map map) : base(map)
        {
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();

            if (Find.TickManager.CurTimeSpeed == TimeSpeed.Paused)
            {
                return;
            }

            var settings = WorkManagerModule.ModSettings;
            if (settings == null || !settings.EnableWorkManager || !settings.ManageWorkPriorities)
            {
                return;
            }

            int interval = Mathf.Clamp(settings.PriorityUpdateIntervalTicks, 250, 2500);
            if (isDirty || (Find.TickManager.TicksGame + map.Index * 37) % interval == 0)
            {
                isDirty = false;
                UpdateWorkPriorities();
            }
        }

        public void UpdateWorkPriorities()
        {
            if (Current.Game == null || map == null) return;

            // Ensure manual priorities (1-4) are active in game
            Current.Game.playSettings.useWorkPriorities = true;

            var settings = WorkManagerModule.ModSettings;
            var comp = WorkManagerGameComponent.Instance;

            // Collect capable player colonists on this map
            capablePawns.Clear();
            var freeColonists = map.mapPawns.FreeColonists;
            if (freeColonists == null || freeColonists.Count == 0) return;

            for (int i = 0; i < freeColonists.Count; i++)
            {
                var pawn = freeColonists[i];
                if (pawn == null || pawn.Dead || pawn.Downed || pawn.InMentalState) continue;
                if (pawn.workSettings == null || !pawn.workSettings.EverWork) continue;
                if (comp != null && !comp.IsPawnPriorityEnabled(pawn)) continue;

                capablePawns.Add(pawn);
            }

            if (capablePawns.Count == 0) return;

            // 1. Emergency Work Types: Always priority 1 for capable pawns
            AssignEmergencyWork(FirefighterDef);
            AssignEmergencyWork(PatientDef);
            AssignEmergencyWork(PatientBedRestDef);
            AssignEmergencyWork(BasicWorkerDef);

            // 2. Dedicated Specialist Roles
            if (settings.UseDedicatedWorkers)
            {
                AssignDedicatedWorkers(DoctorDef, settings.TargetDoctors, 1);
                AssignDedicatedWorkers(CookingDef, settings.TargetCooks, 2);
                AssignDedicatedWorkers(ResearchDef, settings.TargetResearchers, 2);
                AssignDedicatedWorkers(WardenDef, settings.TargetWardens, 2);
                AssignDedicatedWorkers(HandlingDef, settings.TargetHandlers, 2);
            }

            // 3. Other general work types: Rank-based priorities (2, 3, 4)
            var allWorkTypes = DefDatabase<WorkTypeDef>.AllDefsListForReading;
            for (int i = 0; i < allWorkTypes.Count; i++)
            {
                var workType = allWorkTypes[i];
                if (!workType.visible) continue;

                // Skip emergency types already handled
                if (workType == FirefighterDef || workType == PatientDef ||
                    workType == PatientBedRestDef || workType == BasicWorkerDef)
                {
                    continue;
                }

                // If dedicated workers is active, skip the dedicated types
                if (settings.UseDedicatedWorkers &&
                    (workType == DoctorDef || workType == CookingDef || workType == ResearchDef ||
                     workType == WardenDef || workType == HandlingDef))
                {
                    continue;
                }

                AssignGeneralWorkType(workType);
            }

            // 4. Assign Work to Idle Pawns (Elevate Hauling / Cleaning)
            if (settings.AssignWorkToIdlePawns)
            {
                for (int i = 0; i < capablePawns.Count; i++)
                {
                    var pawn = capablePawns[i];
                    if (pawn.mindState != null && pawn.mindState.IsIdle && !pawn.Drafted)
                    {
                        if (HaulingDef != null && !pawn.WorkTypeIsDisabled(HaulingDef))
                        {
                            int cur = pawn.workSettings.GetPriority(HaulingDef);
                            if (cur == 0 || cur > 2)
                            {
                                pawn.workSettings.SetPriority(HaulingDef, 2);
                            }
                        }

                        if (CleaningDef != null && !pawn.WorkTypeIsDisabled(CleaningDef))
                        {
                            int cur = pawn.workSettings.GetPriority(CleaningDef);
                            if (cur == 0 || cur > 3)
                            {
                                pawn.workSettings.SetPriority(CleaningDef, 3);
                            }
                        }
                    }
                }
            }
        }

        private void AssignEmergencyWork(WorkTypeDef workType)
        {
            if (workType == null) return;

            for (int i = 0; i < capablePawns.Count; i++)
            {
                var pawn = capablePawns[i];
                if (!pawn.WorkTypeIsDisabled(workType))
                {
                    pawn.workSettings.SetPriority(workType, 1);
                }
                else
                {
                    pawn.workSettings.SetPriority(workType, 0);
                }
            }
        }

        private void AssignDedicatedWorkers(WorkTypeDef workType, int targetCount, int primaryPriority)
        {
            if (workType == null || targetCount <= 0) return;

            candidateScores.Clear();
            for (int i = 0; i < capablePawns.Count; i++)
            {
                var pawn = capablePawns[i];
                float score = PawnSuitabilityScorer.CalculateScore(pawn, workType);
                if (score > 0f)
                {
                    candidateScores.Add(new PawnScoreEntry { Pawn = pawn, Score = score });
                }
                else
                {
                    pawn.workSettings.SetPriority(workType, 0);
                }
            }

            candidateScores.Sort(ScoreComparison);

            int assigned = 0;
            for (int i = 0; i < candidateScores.Count; i++)
            {
                var entry = candidateScores[i];
                if (assigned < targetCount)
                {
                    int prio = (assigned == 0) ? primaryPriority : Math.Min(primaryPriority + 1, 3);
                    entry.Pawn.workSettings.SetPriority(workType, prio);
                    assigned++;
                }
                else if (assigned < targetCount + 2 && entry.Score >= 40f)
                {
                    entry.Pawn.workSettings.SetPriority(workType, 3);
                    assigned++;
                }
                else
                {
                    entry.Pawn.workSettings.SetPriority(workType, 0);
                }
            }
        }

        private void AssignGeneralWorkType(WorkTypeDef workType)
        {
            candidateScores.Clear();
            for (int i = 0; i < capablePawns.Count; i++)
            {
                var pawn = capablePawns[i];
                float score = PawnSuitabilityScorer.CalculateScore(pawn, workType);
                if (score > 0f)
                {
                    candidateScores.Add(new PawnScoreEntry { Pawn = pawn, Score = score });
                }
                else
                {
                    pawn.workSettings.SetPriority(workType, 0);
                }
            }

            if (candidateScores.Count == 0) return;

            candidateScores.Sort(ScoreComparison);

            int count = candidateScores.Count;
            int topTierCount = Math.Max(1, count / 3);
            int midTierCount = Math.Max(1, count * 2 / 3);

            for (int i = 0; i < count; i++)
            {
                var entry = candidateScores[i];
                if (i < topTierCount)
                {
                    entry.Pawn.workSettings.SetPriority(workType, 2);
                }
                else if (i < midTierCount || entry.Score >= 40f)
                {
                    entry.Pawn.workSettings.SetPriority(workType, 3);
                }
                else
                {
                    if (workType.relevantSkills != null && workType.relevantSkills.Count > 0 && entry.Score < 25f)
                    {
                        entry.Pawn.workSettings.SetPriority(workType, 0);
                    }
                    else
                    {
                        entry.Pawn.workSettings.SetPriority(workType, 4);
                    }
                }
            }
        }
    }
}
