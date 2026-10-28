using Verse;

namespace SadSeal.MegaMod.TimeFlow
{
    public class TimeFlowSettings : IExposable
    {
        public bool UnlockSuperSpeed = true;
        public bool DisableForcedSlowdown = true;
        public bool EnableSmartSleepFastForward = true;
        public int SleepFastForwardSpeed = 4; // 3 = Superfast, 4 = Ultrafast

        public void ExposeData()
        {
            Scribe_Values.Look(ref UnlockSuperSpeed, "UnlockSuperSpeed", true);
            Scribe_Values.Look(ref DisableForcedSlowdown, "DisableForcedSlowdown", true);
            Scribe_Values.Look(ref EnableSmartSleepFastForward, "EnableSmartSleepFastForward", true);
            Scribe_Values.Look(ref SleepFastForwardSpeed, "SleepFastForwardSpeed", 4);
        }
    }
}
