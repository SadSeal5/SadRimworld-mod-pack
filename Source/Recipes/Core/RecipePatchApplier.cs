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
        public float WorkToMake = -1f;
        public List<IngredientCount> Ingredients;
        public List<ThingDef> RecipeUsers;
        public ThingFilter FixedFilter;
        public ThingFilter DefaultFilter;
        public List<ThingDef> ExplicitRecipeUsers;
    }

    [StaticConstructorOnStartup]
    public static class RecipePatchApplier
    {
        public static Dictionary<string, VanillaSnapshot> VanillaSnapshots = new Dictionary<string, VanillaSnapshot>();

        private static readonly FieldInfo AllRecipesCachedField =
            typeof(ThingDef).GetField("allRecipesCached", BindingFlags.Instance | BindingFlags.NonPublic);

        public static void InvalidateRecipeCache(ThingDef workbench)
        {
            if (workbench == null) return;
            AllRecipesCachedField?.SetValue(workbench, null);
        }

        static RecipePatchApplier()
        {
            ApplyAll();
            CustomRecipeManager.InitializeAll();
        }

        public static void EnsureVanillaSaved(RecipeDef r)
        {
            if (r == null || VanillaSnapshots.ContainsKey(r.defName)) return;
            var snap = new VanillaSnapshot
            {
                Label = r.label,
                WorkAmount = r.workAmount,
                Ingredients = r.ingredients != null ? new List<IngredientCount>(r.ingredients) : new List<IngredientCount>(),
                RecipeUsers = r.recipeUsers != null ? new List<ThingDef>(r.recipeUsers) : new List<ThingDef>()
            };
            if (r.products != null && r.products.Count > 0 && r.products[0].thingDef != null)
            {
                snap.WorkToMake = r.products[0].thingDef.GetStatValueAbstract(StatDefOf.WorkToMake);
            }

            // The ingredient filter is mutated while applying custom ingredients, so a
            // full copy is required to restore it correctly on reset.
            if (r.fixedIngredientFilter != null)
            {
                snap.FixedFilter = new ThingFilter();
                snap.FixedFilter.CopyAllowancesFrom(r.fixedIngredientFilter);
            }
            if (r.defaultIngredientFilter != null)
            {
                snap.DefaultFilter = new ThingFilter();
                snap.DefaultFilter.CopyAllowancesFrom(r.defaultIngredientFilter);
            }

            // Remember which workbenches list this recipe explicitly (not via recipeUsers),
            // so disabling and then resetting does not silently drop it.
            snap.ExplicitRecipeUsers = new List<ThingDef>();
            if (r.recipeUsers != null)
            {
                for (int i = 0; i < r.recipeUsers.Count; i++)
                {
                    ThingDef user = r.recipeUsers[i];
                    if (user?.recipes != null && user.recipes.Contains(r))
                    {
                        snap.ExplicitRecipeUsers.Add(user);
                    }
                }
            }

            VanillaSnapshots[r.defName] = snap;
        }

        public static void ApplyAll()
        {
            var settings = MegaMod.Settings?.Recipes;
            if (settings?.tweaks == null || settings.tweaks.Count == 0) return;
            for (int i = 0; i < settings.tweaks.Count; i++)
            {
                var tw = settings.tweaks[i];
                if (tw == null) continue;
                var r = DefDatabase<RecipeDef>.GetNamed(tw.recipeDefName, false);
                if (r == null) continue;
                EnsureVanillaSaved(r);
                ApplyTweak(r, tw);
            }
        }

        public static void ApplyTweak(RecipeDef recipe, RecipeTweak tweak)
        {
            EnsureVanillaSaved(recipe);

            // Re-apply from the vanilla snapshot so toggling a tweak off (for example
            // unchecking "Disable Recipe") actually restores the original state.
            // Custom recipes are rebuilt from CustomRecipeData instead; resetting them
            // here would undo workbench/ingredient changes made in the same save.
            if (!CustomRecipeManager.IsCustomRecipe(recipe.defName))
            {
                ResetToVanilla(recipe);
            }

            if (!string.IsNullOrEmpty(tweak.newLabel)) recipe.label = tweak.newLabel;

            // Hem tarif workAmount hem de esyanin WorkToMake degeri guncellenir (CRAFTING FIX).
            // WorkToMake yalnizca tarif kendi workAmount degerini kullanmiyorsa guncellenir;
            // aksi halde ayni urunu ureten diger tarifler de yanlislikla etkilenir.
            if (tweak.newWorkAmount >= 0f)
            {
                bool usesProductWorkAmount = recipe.workAmount < 0f;
                recipe.workAmount = tweak.newWorkAmount;

                if (usesProductWorkAmount && recipe.products != null)
                {
                    for (int p = 0; p < recipe.products.Count; p++)
                    {
                        var prod = recipe.products[p].thingDef;
                        if (prod != null)
                        {
                            prod.SetStatBaseValue(StatDefOf.WorkToMake, tweak.newWorkAmount);
                        }
                    }
                }
            }

            if (tweak.hasCustomIngredients && tweak.customIngredients != null)
            {
                recipe.ingredients = new List<IngredientCount>();
                if (recipe.fixedIngredientFilter != null)
                    recipe.fixedIngredientFilter.SetDisallowAll();
                if (recipe.defaultIngredientFilter != null)
                    recipe.defaultIngredientFilter.SetDisallowAll();

                for (int i = 0; i < tweak.customIngredients.Count; i++)
                {
                    var s = tweak.customIngredients[i];
                    var td = DefDatabase<ThingDef>.GetNamed(s.thingDefName, false);
                    if (td != null)
                    {
                        var ing = new IngredientCount();
                        ing.filter.SetAllow(td, true);
                        ing.SetBaseCount(s.count);
                        recipe.ingredients.Add(ing);
                        recipe.fixedIngredientFilter?.SetAllow(td, true);
                        recipe.defaultIngredientFilter?.SetAllow(td, true);
                    }
                }
            }

            if (tweak.disabled)
            {
                if (VanillaSnapshots.TryGetValue(recipe.defName, out var snap))
                {
                    if (snap.ExplicitRecipeUsers == null)
                    {
                        snap.ExplicitRecipeUsers = new List<ThingDef>();
                    }
                }

                var users = new List<ThingDef>();
                if (recipe.recipeUsers != null)
                {
                    users.AddRange(recipe.recipeUsers);
                    recipe.recipeUsers.Clear();
                }

                // Some workbenches list recipes explicitly in XML instead of using
                // recipeUsers; remove those too so the recipe truly disappears.
                var allDefs = DefDatabase<ThingDef>.AllDefsListForReading;
                for (int i = 0; i < allDefs.Count; i++)
                {
                    ThingDef def = allDefs[i];
                    if (def?.recipes != null && def.recipes.Contains(recipe))
                    {
                        if (snap != null && !snap.ExplicitRecipeUsers.Contains(def))
                        {
                            snap.ExplicitRecipeUsers.Add(def);
                        }
                        if (!users.Contains(def))
                        {
                            users.Add(def);
                        }
                    }
                }

                for (int i = 0; i < users.Count; i++)
                {
                    ThingDef user = users[i];
                    if (user?.recipes != null)
                    {
                        user.recipes.Remove(recipe);
                    }
                    InvalidateRecipeCache(user);
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
                recipe.recipeUsers = new List<ThingDef>(snap.RecipeUsers);

                if (snap.FixedFilter != null && recipe.fixedIngredientFilter != null)
                {
                    recipe.fixedIngredientFilter.CopyAllowancesFrom(snap.FixedFilter);
                }
                if (snap.DefaultFilter != null && recipe.defaultIngredientFilter != null)
                {
                    recipe.defaultIngredientFilter.CopyAllowancesFrom(snap.DefaultFilter);
                }

                // Re-add recipes that were listed explicitly on a workbench before the
                // tweak removed them. recipeUsers entries are restored above already.
                if (snap.ExplicitRecipeUsers != null)
                {
                    for (int i = 0; i < snap.ExplicitRecipeUsers.Count; i++)
                    {
                        ThingDef user = snap.ExplicitRecipeUsers[i];
                        if (user?.recipes != null && !user.recipes.Contains(recipe))
                        {
                            user.recipes.Add(recipe);
                        }
                    }
                }

                for (int i = 0; i < recipe.recipeUsers.Count; i++)
                {
                    InvalidateRecipeCache(recipe.recipeUsers[i]);
                }

                if (snap.ExplicitRecipeUsers != null)
                {
                    for (int i = 0; i < snap.ExplicitRecipeUsers.Count; i++)
                    {
                        InvalidateRecipeCache(snap.ExplicitRecipeUsers[i]);
                    }
                }

                // Only restore the product stat when the recipe originally relied on it.
                if (snap.WorkAmount < 0f && snap.WorkToMake >= 0f && recipe.products != null)
                {
                    for (int p = 0; p < recipe.products.Count; p++)
                    {
                        recipe.products[p].thingDef?.SetStatBaseValue(StatDefOf.WorkToMake, snap.WorkToMake);
                    }
                }
            }
        }
    }
}
