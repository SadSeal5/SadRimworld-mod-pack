using System.Collections.Generic;
using SadSeal.MegaMod.WorkManager.Core;
using Verse;

namespace SadSeal.MegaMod.WorkManager
{
    public class WorkManagerGameComponent : GameComponent
    {
        public static WorkManagerGameComponent Instance { get; private set; }

        public static bool IsInitialized => Current.Game != null && Instance != null;

        private HashSet<int> disabledPawnPriorities = new HashSet<int>();
        private HashSet<int> disabledPawnSchedules = new HashSet<int>();

        public WorkManagerGameComponent(Game game)
        {
            Instance = this;
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            Instance = this;
        }

        public bool IsPawnPriorityEnabled(Pawn pawn)
        {
            if (pawn == null) return false;
            return !disabledPawnPriorities.Contains(pawn.thingIDNumber);
        }

        public void SetPawnPriorityEnabled(Pawn pawn, bool enabled)
        {
            if (pawn == null) return;
            if (enabled)
            {
                if (disabledPawnPriorities.Remove(pawn.thingIDNumber))
                {
                    ForceUpdateAll();
                }
            }
            else
            {
                if (disabledPawnPriorities.Add(pawn.thingIDNumber))
                {
                    ForceUpdateAll();
                }
            }
        }

        public bool IsPawnScheduleEnabled(Pawn pawn)
        {
            if (pawn == null) return false;
            return !disabledPawnSchedules.Contains(pawn.thingIDNumber);
        }

        public void SetPawnScheduleEnabled(Pawn pawn, bool enabled)
        {
            if (pawn == null) return;
            if (enabled)
            {
                if (disabledPawnSchedules.Remove(pawn.thingIDNumber))
                {
                    ForceUpdateAll();
                }
            }
            else
            {
                if (disabledPawnSchedules.Add(pawn.thingIDNumber))
                {
                    ForceUpdateAll();
                }
            }
        }

        public void TogglePawnPriority(Pawn pawn)
        {
            if (pawn == null) return;
            SetPawnPriorityEnabled(pawn, !IsPawnPriorityEnabled(pawn));
        }

        public void TogglePawnSchedule(Pawn pawn)
        {
            if (pawn == null) return;
            SetPawnScheduleEnabled(pawn, !IsPawnScheduleEnabled(pawn));
        }

        public static void ForceUpdateAll()
        {
            if (Current.Game == null || Find.Maps == null) return;
            var maps = Find.Maps;
            for (int i = 0; i < maps.Count; i++)
            {
                var map = maps[i];
                map.GetComponent<WorkPriorityManager>()?.MarkDirty();
                map.GetComponent<ScheduleManager>()?.MarkDirty();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref disabledPawnPriorities, "disabledPawnPriorities", LookMode.Value);
            Scribe_Collections.Look(ref disabledPawnSchedules, "disabledPawnSchedules", LookMode.Value);

            if (disabledPawnPriorities == null) disabledPawnPriorities = new HashSet<int>();
            if (disabledPawnSchedules == null) disabledPawnSchedules = new HashSet<int>();
        }
    }
}
