using Verse;

namespace SadSeal.MegaMod.WorkManager
{
    public enum WorkManagerPreset
    {
        Custom,
        Balanced,
        Performance,
        Micro
    }

    public class WorkManagerSettings : IExposable
    {
        public bool EnableWorkManager = true;
        public bool ManageWorkPriorities = true;
        public bool ManageWorkSchedule = true;
        public bool UseDedicatedWorkers = true;
        public bool UsePassionPriorities = true;
        public bool AssignWorkToIdlePawns = true;
        public bool SupportNightOwls = true;

        public int PriorityUpdateIntervalTicks = 500;
        public int ScheduleUpdateIntervalHours = 4;

        public int TargetDoctors = 2;
        public int TargetCooks = 1;
        public int TargetResearchers = 1;
        public int TargetWardens = 1;
        public int TargetHandlers = 1;

        public WorkManagerPreset ActivePreset = WorkManagerPreset.Balanced;

        public void ApplyPreset(WorkManagerPreset preset)
        {
            ActivePreset = preset;
            switch (preset)
            {
                case WorkManagerPreset.Balanced:
                    EnableWorkManager = true;
                    ManageWorkPriorities = true;
                    ManageWorkSchedule = true;
                    UseDedicatedWorkers = true;
                    UsePassionPriorities = true;
                    AssignWorkToIdlePawns = true;
                    SupportNightOwls = true;
                    PriorityUpdateIntervalTicks = 500;
                    ScheduleUpdateIntervalHours = 4;
                    TargetDoctors = 2;
                    TargetCooks = 1;
                    TargetResearchers = 1;
                    TargetWardens = 1;
                    TargetHandlers = 1;
                    break;

                case WorkManagerPreset.Performance:
                    EnableWorkManager = true;
                    ManageWorkPriorities = true;
                    ManageWorkSchedule = true;
                    UseDedicatedWorkers = true;
                    UsePassionPriorities = true;
                    AssignWorkToIdlePawns = false;
                    SupportNightOwls = true;
                    PriorityUpdateIntervalTicks = 1500;
                    ScheduleUpdateIntervalHours = 8;
                    TargetDoctors = 2;
                    TargetCooks = 1;
                    TargetResearchers = 1;
                    TargetWardens = 1;
                    TargetHandlers = 1;
                    break;

                case WorkManagerPreset.Micro:
                    EnableWorkManager = true;
                    ManageWorkPriorities = true;
                    ManageWorkSchedule = true;
                    UseDedicatedWorkers = true;
                    UsePassionPriorities = true;
                    AssignWorkToIdlePawns = true;
                    SupportNightOwls = true;
                    PriorityUpdateIntervalTicks = 250;
                    ScheduleUpdateIntervalHours = 2;
                    TargetDoctors = 3;
                    TargetCooks = 2;
                    TargetResearchers = 2;
                    TargetWardens = 2;
                    TargetHandlers = 2;
                    break;
            }
        }

        public void ResetToDefaults()
        {
            ApplyPreset(WorkManagerPreset.Balanced);
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref EnableWorkManager, "EnableWorkManager", true);
            Scribe_Values.Look(ref ManageWorkPriorities, "ManageWorkPriorities", true);
            Scribe_Values.Look(ref ManageWorkSchedule, "ManageWorkSchedule", true);
            Scribe_Values.Look(ref UseDedicatedWorkers, "UseDedicatedWorkers", true);
            Scribe_Values.Look(ref UsePassionPriorities, "UsePassionPriorities", true);
            Scribe_Values.Look(ref AssignWorkToIdlePawns, "AssignWorkToIdlePawns", true);
            Scribe_Values.Look(ref SupportNightOwls, "SupportNightOwls", true);

            Scribe_Values.Look(ref PriorityUpdateIntervalTicks, "PriorityUpdateIntervalTicks", 500);
            Scribe_Values.Look(ref ScheduleUpdateIntervalHours, "ScheduleUpdateIntervalHours", 4);

            Scribe_Values.Look(ref TargetDoctors, "TargetDoctors", 2);
            Scribe_Values.Look(ref TargetCooks, "TargetCooks", 1);
            Scribe_Values.Look(ref TargetResearchers, "TargetResearchers", 1);
            Scribe_Values.Look(ref TargetWardens, "TargetWardens", 1);
            Scribe_Values.Look(ref TargetHandlers, "TargetHandlers", 1);

            Scribe_Values.Look(ref ActivePreset, "ActivePreset", WorkManagerPreset.Balanced);
        }
    }
}
