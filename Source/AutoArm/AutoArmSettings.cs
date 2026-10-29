using Verse;

namespace SadSeal.MegaMod.AutoArm
{
    public class AutoArmSettings : IExposable
    {
        public bool enableAutoArm = true;
        public bool onlyFromStockpile = true;
        public float minScoreDifference = 0.15f;
        public int checkIntervalSeconds = 30;
        public bool autoAssignBySkill = true;
        public bool considerQuality = true;
        public bool considerHitPoints = true;
        public bool autoUnforbidDropped = true;

        public void ExposeData()
        {
            Scribe_Values.Look(ref enableAutoArm, "enableAutoArm", true);
            Scribe_Values.Look(ref onlyFromStockpile, "onlyFromStockpile", true);
            Scribe_Values.Look(ref minScoreDifference, "minScoreDifference", 0.15f);
            Scribe_Values.Look(ref checkIntervalSeconds, "checkIntervalSeconds", 30);
            Scribe_Values.Look(ref autoAssignBySkill, "autoAssignBySkill", true);
            Scribe_Values.Look(ref considerQuality, "considerQuality", true);
            Scribe_Values.Look(ref considerHitPoints, "considerHitPoints", true);
            Scribe_Values.Look(ref autoUnforbidDropped, "autoUnforbidDropped", true);
        }
    }
}
