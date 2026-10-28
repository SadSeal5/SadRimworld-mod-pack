using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Quarry
{
    public class RecipeWorker_Quarry : RecipeWorker
    {
        public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
        {
            base.Notify_IterationCompleted(billDoer, ingredients);

            if (billDoer == null || billDoer.Map == null) return;

            var settings = MegaMod.Settings?.Quarry;
            Map map = billDoer.Map;
            IntVec3 pos = billDoer.Position;

            // Grant mining skill experience
            if (billDoer.skills != null)
            {
                float xp = 150f * (settings != null ? settings.MiningSpeedMultiplier : 1.0f);
                billDoer.skills.Learn(SkillDefOf.Mining, xp);
            }

            ThingDef outputDef = null;
            int outputCount = 1;

            string recipeName = recipe?.defName ?? "";

            if (recipeName.Contains("MineChunks"))
            {
                // Stone chunks
                outputDef = GetRandomStoneChunkDef();
                outputCount = 1;
            }
            else if (recipeName.Contains("MineComponents"))
            {
                // Industrial components
                outputDef = ThingDefOf.ComponentIndustrial;
                outputCount = Rand.RangeInclusive(1, 2);

                if (Rand.Value < 0.12f)
                {
                    outputDef = ThingDefOf.ComponentSpacer;
                    outputCount = 1;
                }
            }
            else
            {
                // General resources (Steel, Plasteel, Uranium, etc.)
                outputDef = PickWeightedResource(settings, out outputCount);
            }

            if (outputDef != null)
            {
                Thing minedThing = ThingMaker.MakeThing(outputDef);
                minedThing.stackCount = Mathf.Clamp(outputCount, 1, outputDef.stackLimit);
                GenSpawn.Spawn(minedThing, pos, map);

                Messages.Message("MegaMod_QuarryMinedItem".Translate(billDoer.LabelShort, minedThing.Label),
                    MessageTypeDefOf.PositiveEvent, false);
            }
        }

        private static ThingDef GetRandomStoneChunkDef()
        {
            List<ThingDef> chunks = new List<ThingDef>();
            foreach (ThingDef td in DefDatabase<ThingDef>.AllDefs)
            {
                if (td.thingCategories != null && td.thingCategories.Contains(ThingCategoryDefOf.StoneChunks))
                {
                    chunks.Add(td);
                }
            }

            if (chunks.Count > 0)
            {
                return chunks.RandomElement();
            }

            return ThingDefOf.Steel;
        }

        private static ThingDef PickWeightedResource(QuarrySettings settings, out int count)
        {
            float wSteel = settings?.WeightSteel ?? 40f;
            float wPlasteel = settings?.WeightPlasteel ?? 15f;
            float wUranium = settings?.WeightUranium ?? 10f;
            float wComp = settings?.WeightComponents ?? 15f;
            float wGold = settings?.WeightGold ?? 8f;
            float wSilver = settings?.WeightSilver ?? 12f;

            float total = wSteel + wPlasteel + wUranium + wComp + wGold + wSilver;
            float roll = Rand.Range(0f, total);

            if (roll < wSteel)
            {
                count = Rand.RangeInclusive(20, 45);
                return ThingDefOf.Steel;
            }
            roll -= wSteel;

            if (roll < wPlasteel)
            {
                count = Rand.RangeInclusive(8, 20);
                return ThingDefOf.Plasteel;
            }
            roll -= wPlasteel;

            if (roll < wUranium)
            {
                count = Rand.RangeInclusive(6, 15);
                return ThingDefOf.Uranium;
            }
            roll -= wUranium;

            if (roll < wComp)
            {
                count = Rand.RangeInclusive(1, 2);
                return ThingDefOf.ComponentIndustrial;
            }
            roll -= wComp;

            if (roll < wGold)
            {
                count = Rand.RangeInclusive(5, 15);
                return ThingDefOf.Gold;
            }

            count = Rand.RangeInclusive(30, 75);
            return ThingDefOf.Silver;
        }
    }
}
