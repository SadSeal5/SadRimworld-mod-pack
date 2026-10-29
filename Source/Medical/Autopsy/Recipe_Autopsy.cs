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
                corpse.Strip();
                AutopsyHelper.PerformAutopsy(billDoer, corpse, medicineUsed);
            }
        }

        public override void ConsumeIngredient(Thing ingredient, RecipeDef recipe, Map map)
        {
            base.ConsumeIngredient(ingredient, recipe, map);
        }
    }
}
