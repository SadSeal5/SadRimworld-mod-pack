using System.Collections.Generic;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Medical.Autopsy
{
    public class Recipe_Autopsy : RecipeWorker
    {
        public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
        {
            base.Notify_IterationCompleted(billDoer, ingredients);

            if (ingredients == null || billDoer == null) return;

            try
            {
                Corpse corpse = null;
                Thing medicineUsed = null;

                for (int i = 0; i < ingredients.Count; i++)
                {
                    Thing ing = ingredients[i];
                    if (ing is Corpse c)
                    {
                        corpse = c;
                    }
                    else if (ing?.def != null && (ing.def.IsMedicine || (ing.def.thingCategories != null && ing.def.thingCategories.Contains(ThingCategoryDefOf.Medicine))))
                    {
                        medicineUsed = ing;
                    }
                }

                if (corpse != null)
                {
                    StripCorpseSafely(corpse, billDoer);
                    AutopsyHelper.PerformAutopsy(billDoer, corpse, medicineUsed);
                    if (!corpse.Destroyed)
                    {
                        corpse.Destroy(DestroyMode.Vanish);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Error($"[SadSeal's Mega Mod] Error during autopsy iteration completed: {ex}");
            }
        }

        private static void StripCorpseSafely(Corpse corpse, Pawn billDoer)
        {
            if (corpse == null || billDoer == null || billDoer.Map == null) return;

            if (corpse.Spawned && corpse.Map != null)
            {
                corpse.Strip();
                return;
            }

            Pawn innerPawn = corpse.InnerPawn;
            if (innerPawn == null) return;

            Map map = billDoer.Map;
            IntVec3 pos = billDoer.Position;

            if (innerPawn.apparel != null)
            {
                List<Apparel> worn = new List<Apparel>(innerPawn.apparel.WornApparel);
                for (int i = 0; i < worn.Count; i++)
                {
                    innerPawn.apparel.Remove(worn[i]);
                    GenPlace.TryPlaceThing(worn[i], pos, map, ThingPlaceMode.Near);
                }
            }

            if (innerPawn.equipment != null)
            {
                List<ThingWithComps> eq = new List<ThingWithComps>(innerPawn.equipment.AllEquipmentListForReading);
                for (int i = 0; i < eq.Count; i++)
                {
                    innerPawn.equipment.Remove(eq[i]);
                    GenPlace.TryPlaceThing(eq[i], pos, map, ThingPlaceMode.Near);
                }
            }

            if (innerPawn.inventory?.innerContainer != null)
            {
                innerPawn.inventory.innerContainer.TryDropAll(pos, map, ThingPlaceMode.Near);
            }
        }

        public override void ConsumeIngredient(Thing ingredient, RecipeDef recipe, Map map)
        {
            if (ingredient is Corpse)
            {
                // Defer destruction of corpse until Notify_IterationCompleted so inner pawn body parts remain intact
                return;
            }
            base.ConsumeIngredient(ingredient, recipe, map);
        }
    }
}
