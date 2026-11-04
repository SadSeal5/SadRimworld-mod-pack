using System.Collections.Generic;
using Verse;

namespace SadSeal.MegaMod.Recipes
{
    public class RecipeSettings : IExposable
    {
        public List<RecipeTweak> tweaks = new List<RecipeTweak>();
        public List<CustomRecipeData> customRecipes = new List<CustomRecipeData>();

        public void ExposeData()
        {
            Scribe_Collections.Look(ref tweaks, "tweaks", LookMode.Deep);
            Scribe_Collections.Look(ref customRecipes, "customRecipes", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (tweaks == null) tweaks = new List<RecipeTweak>();
                if (customRecipes == null) customRecipes = new List<CustomRecipeData>();
                PruneVanilla();
            }
        }

        public RecipeTweak FindTweak(string defName)
        {
            if (tweaks == null) return null;
            for (int i = 0; i < tweaks.Count; i++)
            {
                if (tweaks[i] != null && tweaks[i].recipeDefName == defName) return tweaks[i];
            }
            return null;
        }

        public RecipeTweak GetOrCreate(string defName)
        {
            if (tweaks == null) tweaks = new List<RecipeTweak>();
            var existing = FindTweak(defName);
            if (existing != null) return existing;
            var t = new RecipeTweak { recipeDefName = defName };
            tweaks.Add(t);
            return t;
        }

        public void PruneVanilla()
        {
            tweaks?.RemoveAll(t => t == null || t.IsVanilla);
        }

        public void Write()
        {
            MegaMod.Instance?.WriteSettings();
        }
    }
}
