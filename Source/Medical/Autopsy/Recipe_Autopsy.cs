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

            for (int i = 0; i < ingredients.Count; i++)
            {
                if (ingredients[i] is Corpse corpse)
                {
                    AutopsyHelper.PerformAutopsy(billDoer, corpse);
                    break;
                }
            }
        }

        public override void ConsumeIngredient(Thing ingredient, RecipeDef recipe, Map map)
        {
            base.ConsumeIngredient(ingredient, recipe, map);
        }
    }
}
