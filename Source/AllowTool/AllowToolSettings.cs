using Verse;

namespace SadSeal.MegaMod.AllowTool
{
    public class AllowToolSettings : IExposable
    {
        public bool EnableUnforbidAll = true;
        public bool EnableHaulUrgently = true;
        public bool EnableSelectSimilar = true;
        public bool EnableFinishOff = true;

        public void ExposeData()
        {
            Scribe_Values.Look(ref EnableUnforbidAll, "EnableUnforbidAll", true);
            Scribe_Values.Look(ref EnableHaulUrgently, "EnableHaulUrgently", true);
            Scribe_Values.Look(ref EnableSelectSimilar, "EnableSelectSimilar", true);
            Scribe_Values.Look(ref EnableFinishOff, "EnableFinishOff", true);
        }
    }
}
