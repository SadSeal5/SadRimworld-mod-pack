using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Quarry
{
    public class RecipeWorker_Quarry : RecipeWorker
    {
        private static List<ThingDef> cachedStoneChunks;

        public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
        {
            base.Notify_IterationCompleted(billDoer, ingredients);

            if (billDoer == null || billDoer.Map == null) return;

            var settings = MegaMod.Settings?.Quarry;
            Map map = billDoer.Map;
            IntVec3 pos = billDoer.Position;

            // Increment excavation count on quarry building
            Building_Quarry quarry = billDoer.CurJob?.GetTarget(Verse.AI.TargetIndex.A).Thing as Building_Quarry
                                  ?? pos.GetFirstThing<Building_Quarry>(map);
            if (quarry != null)
            {
                quarry.TotalExcavationsCompleted++;
                if (quarry.IsDepleted)
                {
                    Messages.Message("MegaMod_QuarryDepleted".Translate(), quarry, MessageTypeDefOf.CautionInput, false);
                }
            }

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
                if (recipeName.Contains("Marble"))
                {
                    outputDef = DefDatabase<ThingDef>.GetNamedSilentFail("ChunkMarble") ?? GetRandomStoneChunkDef();
                }
                else if (recipeName.Contains("Granite"))
                {
                    outputDef = DefDatabase<ThingDef>.GetNamedSilentFail("ChunkGranite") ?? GetRandomStoneChunkDef();
                }
                else if (recipeName.Contains("Slate"))
                {
                    outputDef = DefDatabase<ThingDef>.GetNamedSilentFail("ChunkSlate") ?? GetRandomStoneChunkDef();
                }
                else if (recipeName.Contains("Limestone"))
                {
                    outputDef = DefDatabase<ThingDef>.GetNamedSilentFail("ChunkLimestone") ?? GetRandomStoneChunkDef();
                }
                else if (recipeName.Contains("Sandstone"))
                {
                    outputDef = DefDatabase<ThingDef>.GetNamedSilentFail("ChunkSandstone") ?? GetRandomStoneChunkDef();
                }
                else
                {
                    outputDef = GetRandomStoneChunkDef();
                }
                outputCount = 1;
            }
            else if (recipeName.Contains("MineComponents"))
            {
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
                outputDef = PickWeightedResource(settings, out outputCount);
            }

            if (outputDef != null)
            {
                Thing minedThing = ThingMaker.MakeThing(outputDef);
                minedThing.stackCount = Mathf.Clamp(outputCount, 1, outputDef.stackLimit);
                GenPlace.TryPlaceThing(minedThing, pos, map, ThingPlaceMode.Near);
            }
        }

        private static ThingDef GetRandomStoneChunkDef()
        {
            if (cachedStoneChunks == null)
            {
                cachedStoneChunks = new List<ThingDef>();
                foreach (ThingDef td in DefDatabase<ThingDef>.AllDefs)
                {
                    if (td.thingCategories != null && td.thingCategories.Contains(ThingCategoryDefOf.StoneChunks))
                    {
                        cachedStoneChunks.Add(td);
                    }
                }
            }

            if (cachedStoneChunks.Count > 0)
            {
                return cachedStoneChunks.RandomElement();
            }

            return ThingDefOf.Steel;
        }

        private static ThingDef PickWeightedResource(QuarrySettings settings, out int count)
        {
            float wSteel = Mathf.Max(0f, settings?.WeightSteel ?? 40f);
            float wPlasteel = Mathf.Max(0f, settings?.WeightPlasteel ?? 15f);
            float wUranium = Mathf.Max(0f, settings?.WeightUranium ?? 10f);
            float wComp = Mathf.Max(0f, settings?.WeightComponents ?? 15f);
            float wGold = Mathf.Max(0f, settings?.WeightGold ?? 8f);
            float wSilver = Mathf.Max(0f, settings?.WeightSilver ?? 12f);

            float total = wSteel + wPlasteel + wUranium + wComp + wGold + wSilver;
            if (total <= 0f)
            {
                wSteel = 40f;
                total = 40f;
            }

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

    [HarmonyPatch(typeof(RecipeDef), nameof(RecipeDef.WorkAmountTotal))]
    public static class Patch_RecipeDef_WorkAmountTotal
    {
        public static void Postfix(RecipeDef __instance, ref float __result)
        {
            if (__instance?.workerClass == typeof(RecipeWorker_Quarry))
            {
                float mult = MegaMod.Settings?.Quarry?.MiningSpeedMultiplier ?? 1.0f;
                if (mult > 0.05f)
                {
                    __result /= mult;
                }
            }
        }
    }
}
