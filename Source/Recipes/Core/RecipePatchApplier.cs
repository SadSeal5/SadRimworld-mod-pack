using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Recipes
{
    public class VanillaSnapshot
    {
        public string Label;
        public float WorkAmount;
        public List<IngredientCount> Ingredients;
        public List<ThingDef> RecipeUsers;
        public List<ThingDef> ThingDefRecipes;
        public ThingFilter FixedIngredientFilter;
        public ThingFilter DefaultIngredientFilter;
    }

    public static class RecipePatchApplier
    {
        private static readonly FieldInfo allRecipesCachedField = typeof(ThingDef).GetField("allRecipesCached", BindingFlags.Instance | BindingFlags.NonPublic);

        public static Dictionary<string, VanillaSnapshot> VanillaSnapshots = new Dictionary<string, VanillaSnapshot>();

        public static void EnsureVanillaSaved(RecipeDef r)
        {
            if (r == null || VanillaSnapshots.ContainsKey(r.defName)) return;
            var snap = new VanillaSnapshot
            {
                Label = r.label,
                WorkAmount = r.workAmount,
                Ingredients = r.ingredients != null ? new List<IngredientCount>(r.ingredients) : new List<IngredientCount>(),
                RecipeUsers = r.recipeUsers != null ? new List<ThingDef>(r.recipeUsers) : new List<ThingDef>(),
                ThingDefRecipes = new List<ThingDef>(),
                FixedIngredientFilter = CloneFilter(r.fixedIngredientFilter),
                DefaultIngredientFilter = CloneFilter(r.defaultIngredientFilter)
            };

            var allThings = DefDatabase<ThingDef>.AllDefsListForReading;
            for (int i = 0; i < allThings.Count; i++)
            {
                var td = allThings[i];
                if (td?.recipes != null && td.recipes.Contains(r))
                {
                    snap.ThingDefRecipes.Add(td);
                }
            }

            VanillaSnapshots[r.defName] = snap;
        }

        private static ThingFilter CloneFilter(ThingFilter source)
        {
            if (source == null) return null;
            var copy = new ThingFilter();
            copy.CopyAllowancesFrom(source);
            return copy;
        }

        public static void ApplyAll()
        {
            if (RecipesModule.Instance?.IsEnabled == false) return;
            var settings = RecipesModule.ModSettings;
            if (settings?.tweaks == null || settings.tweaks.Count == 0) return;
            for (int i = 0; i < settings.tweaks.Count; i++)
            {
                var tw = settings.tweaks[i];
                var r = DefDatabase<RecipeDef>.GetNamed(tw.recipeDefName, false);
                if (r == null) continue;
                EnsureVanillaSaved(r);
                ApplyTweak(r, tw);
            }
        }

        public static void ApplyTweak(RecipeDef recipe, RecipeTweak tweak)
        {
            if (RecipesModule.Instance?.IsEnabled != true) return;
            if (recipe == null || tweak == null) return;
            EnsureVanillaSaved(recipe);

            if (!string.IsNullOrEmpty(tweak.newLabel)) recipe.label = tweak.newLabel;

            // RecipeDef.WorkAmountTotal already prefers recipe.workAmount when it is
            // >= 0. Do not touch the shared product WorkToMake stat, otherwise two
            // recipes producing the same thing overwrite each other.
            if (tweak.newWorkAmount >= 0f)
            {
                recipe.workAmount = tweak.newWorkAmount;
            }

            if (tweak.hasCustomIngredients && tweak.customIngredients != null)
            {
                recipe.ingredients = new List<IngredientCount>();
                recipe.fixedIngredientFilter?.SetDisallowAll();
                recipe.defaultIngredientFilter?.SetDisallowAll();

                for (int i = 0; i < tweak.customIngredients.Count; i++)
                {
                    var s = tweak.customIngredients[i];
                    var ing = new IngredientCount();

                    if (s.isFilter && s.filterDefNames != null && s.filterDefNames.Count > 0)
                    {
                        for (int d = 0; d < s.filterDefNames.Count; d++)
                        {
                            var td = DefDatabase<ThingDef>.GetNamed(s.filterDefNames[d], false);
                            if (td == null) continue;
                            ing.filter.SetAllow(td, true);
                            recipe.fixedIngredientFilter?.SetAllow(td, true);
                            recipe.defaultIngredientFilter?.SetAllow(td, true);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(s.thingDefName)) continue;
                        var td = DefDatabase<ThingDef>.GetNamed(s.thingDefName, false);
                        if (td == null) continue;
                        ing.filter.SetAllow(td, true);
                        recipe.fixedIngredientFilter?.SetAllow(td, true);
                        recipe.defaultIngredientFilter?.SetAllow(td, true);
                    }

                    ing.SetBaseCount(s.count);
                    recipe.ingredients.Add(ing);
                }
            }

            if (tweak.disabled)
            {
                RemoveRecipeLinks(recipe);
            }
            else
            {
                RestoreRecipeLinks(recipe);
            }
        }

        private static void RemoveRecipeLinks(RecipeDef recipe)
        {
            if (recipe == null) return;

            recipe.recipeUsers?.Clear();

            var allThings = DefDatabase<ThingDef>.AllDefsListForReading;
            for (int i = 0; i < allThings.Count; i++)
            {
                var td = allThings[i];
                if (td?.recipes == null) continue;
                if (td.recipes.Remove(recipe))
                {
                    allRecipesCachedField?.SetValue(td, null);
                }
            }
        }

        private static void RestoreRecipeLinks(RecipeDef recipe)
        {
            if (recipe == null || !VanillaSnapshots.TryGetValue(recipe.defName, out var snap)) return;

            if (recipe.recipeUsers == null) recipe.recipeUsers = new List<ThingDef>();
            for (int i = 0; i < snap.RecipeUsers.Count; i++)
            {
                var td = snap.RecipeUsers[i];
                if (td != null && !recipe.recipeUsers.Contains(td))
                {
                    recipe.recipeUsers.Add(td);
                }
            }

            for (int i = 0; i < snap.ThingDefRecipes.Count; i++)
            {
                var td = snap.ThingDefRecipes[i];
                if (td == null) continue;
                if (td.recipes == null) td.recipes = new List<RecipeDef>();
                if (!td.recipes.Contains(recipe))
                {
                    td.recipes.Add(recipe);
                    allRecipesCachedField?.SetValue(td, null);
                }
            }
        }

        public static void ResetToVanilla(RecipeDef recipe)
        {
            if (recipe == null) return;
            if (VanillaSnapshots.TryGetValue(recipe.defName, out var snap))
            {
                recipe.label = snap.Label;
                recipe.workAmount = snap.WorkAmount;
                recipe.ingredients = new List<IngredientCount>(snap.Ingredients);

                if (recipe.fixedIngredientFilter != null && snap.FixedIngredientFilter != null)
                {
                    recipe.fixedIngredientFilter.CopyAllowancesFrom(snap.FixedIngredientFilter);
                }
                if (recipe.defaultIngredientFilter != null && snap.DefaultIngredientFilter != null)
                {
                    recipe.defaultIngredientFilter.CopyAllowancesFrom(snap.DefaultIngredientFilter);
                }

                if (recipe.recipeUsers == null) recipe.recipeUsers = new List<ThingDef>();
                recipe.recipeUsers.Clear();
                recipe.recipeUsers.AddRange(snap.RecipeUsers);

                // Re-add the recipe to every ThingDef that had it before the tweak.
                var allThings = DefDatabase<ThingDef>.AllDefsListForReading;
                for (int i = 0; i < allThings.Count; i++)
                {
                    var td = allThings[i];
                    if (td?.recipes == null) continue;
                    bool shouldHave = snap.ThingDefRecipes.Contains(td);
                    bool has = td.recipes.Contains(recipe);
                    if (shouldHave && !has)
                    {
                        td.recipes.Add(recipe);
                        allRecipesCachedField?.SetValue(td, null);
                    }
                    else if (!shouldHave && has)
                    {
                        td.recipes.Remove(recipe);
                        allRecipesCachedField?.SetValue(td, null);
                    }
                }
            }
        }
    }
}
