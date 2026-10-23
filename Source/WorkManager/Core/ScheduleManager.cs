using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.WorkManager.Core
{
    public class ScheduleManager : MapComponent
    {
        private bool isDirty = true;
        public void MarkDirty() => isDirty = true;

        private static TraitDef nightOwlTrait;
        private static TraitDef NightOwlTrait => nightOwlTrait ??= DefDatabase<TraitDef>.GetNamedSilentFail("NightOwl");

        public ScheduleManager(Map map) : base(map)
        {
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();

            if (Find.TickManager.CurTimeSpeed == TimeSpeed.Paused)
            {
                return;
            }

            var settings = MegaMod.Settings.WorkManager;
            if (!settings.EnableWorkManager || !settings.ManageWorkSchedule)
            {
                return;
            }

            int intervalTicks = Mathf.Clamp(settings.ScheduleUpdateIntervalHours, 1, 12) * 2500;
            if (isDirty || (Find.TickManager.TicksGame + map.Index * 59) % intervalTicks == 0)
            {
                isDirty = false;
                UpdateSchedules();
            }
        }

        public void UpdateSchedules()
        {
            if (map == null || map.mapPawns == null) return;

            var freeColonists = map.mapPawns.FreeColonists;
            if (freeColonists == null || freeColonists.Count == 0) return;

            var comp = WorkManagerGameComponent.Instance;
            var settings = MegaMod.Settings.WorkManager;

            for (int i = 0; i < freeColonists.Count; i++)
            {
                var pawn = freeColonists[i];
                if (pawn == null || pawn.Dead || pawn.timetable == null || pawn.story?.traits == null) continue;
                if (comp != null && !comp.IsPawnScheduleEnabled(pawn)) continue;

                bool isNightOwl = settings.SupportNightOwls && NightOwlTrait != null && pawn.story.traits.HasTrait(NightOwlTrait);

                for (int hour = 0; hour < 24; hour++)
                {
                    TimeAssignmentDef targetAssignment = isNightOwl ? GetNightOwlAssignment(hour) : GetDayColonistAssignment(hour);
                    if (pawn.timetable.GetAssignment(hour) != targetAssignment)
                    {
                        pawn.timetable.SetAssignment(hour, targetAssignment);
                    }
                }
            }
        }

        private static TimeAssignmentDef GetNightOwlAssignment(int hour)
        {
            // Work at night: 23:00 - 07:00 (hours 23, 0, 1, 2, 3, 4, 5, 6)
            if (hour >= 23 || hour <= 6) return TimeAssignmentDefOf.Work;

            // Recreation: 07:00 - 10:00 (hours 7, 8, 9)
            if (hour >= 7 && hour <= 9) return TimeAssignmentDefOf.Joy;

            // Free time / buffer before sleep
            if (hour == 10) return TimeAssignmentDefOf.Anything;

            // Sleep during day: 11:00 - 18:00 (hours 11, 12, 13, 14, 15, 16, 17)
            if (hour >= 11 && hour <= 17) return TimeAssignmentDefOf.Sleep;

            // Free time after waking: 18:00 - 23:00
            return TimeAssignmentDefOf.Anything;
        }

        private static TimeAssignmentDef GetDayColonistAssignment(int hour)
        {
            // Sleep: 22:00 - 06:00 (hours 22, 23, 0, 1, 2, 3, 4, 5)
            if (hour >= 22 || hour <= 5) return TimeAssignmentDefOf.Sleep;

            // Morning recreation: 06:00 - 08:00 (hours 6, 7)
            if (hour == 6 || hour == 7) return TimeAssignmentDefOf.Joy;

            // Work: 08:00 - 17:00 (hours 8, 9, 10, 11, 12, 13, 14, 15, 16)
            if (hour >= 8 && hour <= 16) return TimeAssignmentDefOf.Work;

            // Evening recreation: 17:00 - 19:00 (hours 17, 18)
            if (hour == 17 || hour == 18) return TimeAssignmentDefOf.Joy;

            // Free time before bed: 19:00 - 22:00 (hours 19, 20, 21)
            return TimeAssignmentDefOf.Anything;
        }
    }
}
