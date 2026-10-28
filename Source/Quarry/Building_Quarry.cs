using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Quarry
{
    public class Building_Quarry : Building_WorkTable
    {
        public int TotalExcavationsCompleted = 0;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref TotalExcavationsCompleted, "TotalExcavationsCompleted", 0);
        }

        public override string GetInspectString()
        {
            string baseString = base.GetInspectString();
            string customInfo = "MegaMod_Quarry_InspectString".Translate(TotalExcavationsCompleted);

            if (string.IsNullOrEmpty(baseString))
            {
                return customInfo;
            }
            return baseString + "\n" + customInfo;
        }
    }
}
