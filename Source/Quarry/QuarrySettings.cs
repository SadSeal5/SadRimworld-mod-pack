using Verse;

namespace SadSeal.MegaMod.Quarry
{
    public class QuarrySettings : IExposable
    {
        public float MiningSpeedMultiplier = 1.0f;
        public float ChunkDropChance = 0.40f;
        public float ResourceDropChance = 0.60f;
        public bool InfiniteQuarry = true;

        public float WeightSteel = 40f;
        public float WeightPlasteel = 15f;
        public float WeightUranium = 10f;
        public float WeightComponents = 15f;
        public float WeightGold = 8f;
        public float WeightSilver = 12f;

        public void ExposeData()
        {
            Scribe_Values.Look(ref MiningSpeedMultiplier, "MiningSpeedMultiplier", 1.0f);
            Scribe_Values.Look(ref ChunkDropChance, "ChunkDropChance", 0.40f);
            Scribe_Values.Look(ref ResourceDropChance, "ResourceDropChance", 0.60f);
            Scribe_Values.Look(ref InfiniteQuarry, "InfiniteQuarry", true);

            Scribe_Values.Look(ref WeightSteel, "WeightSteel", 40f);
            Scribe_Values.Look(ref WeightPlasteel, "WeightPlasteel", 15f);
            Scribe_Values.Look(ref WeightUranium, "WeightUranium", 10f);
            Scribe_Values.Look(ref WeightComponents, "WeightComponents", 15f);
            Scribe_Values.Look(ref WeightGold, "WeightGold", 8f);
            Scribe_Values.Look(ref WeightSilver, "WeightSilver", 12f);
        }
    }
}
