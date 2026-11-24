using System;
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Recipes
{
    public static class CustomRecipeManager
    {
        private static readonly FieldInfo allRecipesCachedField = typeof(ThingDef).GetField("allRecipesCached", BindingFlags.Instance | BindingFlags.NonPublic);

        public static bool IsCustomRecipe(string defName)
        {
            if (string.IsNullOrEmpty(defName)) return false;
            var list = RecipesModule.ModSettings?.customRecipes;
            if (list == null) return false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].defName == defName) return true;
            }
            return false;
        }

        public static CustomRecipeData GetCustomRecipe(string defName)
        {
            if (string.IsNullOrEmpty(defName)) return null;
            var list = RecipesModule.ModSettings?.customRecipes;
            if (list == null) return null;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].defName == defName) return list[i];
            }
            return null;
        }

        public static void InitializeAll()
        {
            var list = RecipesModule.ModSettings?.customRecipes;
            if (list == null || list.Count == 0) return;
            for (int i = 0; i < list.Count; i++)
            {
                BuildAndRegister(list[i], false);
            }
        }

        public static RecipeDef BuildAndRegister(CustomRecipeData data, bool addToSettings = true)
        {
            if (RecipesModule.Instance?.IsEnabled != true) return null;
            if (data == null || string.IsNullOrEmpty(data.defName)) return null;

            // Product
            var prodDef = DefDatabase<ThingDef>.GetNamed(data.productDefName, false);
            if (prodDef == null)
            {
                Log.Warning($"[SadSeal.Recipes] Skipping custom recipe '{data.defName}': product ThingDef '{data.productDefName}' not found.");
                return null;
            }

            var r = DefDatabase<RecipeDef>.GetNamed(data.defName, false) ?? new RecipeDef();
            r.defName = data.defName;
            r.label = data.label;
            r.jobString = "MegaMod_RecipeJobString".CanTranslate() ? "MegaMod_RecipeJobString".Translate(data.label).Resolve() : $"Producing {data.label}";
            r.workAmount = data.workAmount;
            r.workSpeedStat = StatDefOf.GeneralLaborSpeed;
            r.effectWorking = DefDatabase<EffecterDef>.GetNamedSilentFail("Cook");
            r.soundWorking = DefDatabase<SoundDef>.GetNamedSilentFail("Recipe_CookMeal");

            var product = new ThingDefCountClass(prodDef, Mathf.Max(1, data.productCount));
            if (prodDef.MadeFromStuff)
            {
                // RimWorld 1.7 RecipeDef has no defaultStuff field; GenRecipe reads
                // dominantIngredient.def (and ThingDefCountClass.stuff stores the
                // resolved default stuff metadata).
                product.stuff = GenStuff.DefaultStuffFor(prodDef);
            }
            r.products = new List<ThingDefCountClass> { product };

            // Ingredients & Filters
            r.ingredients = new List<IngredientCount>();
            r.fixedIngredientFilter = new ThingFilter();
            r.defaultIngredientFilter = new ThingFilter();

            bool hasStuffIngredient = false;
            bool firstIngredientHasStuff = false;

            if (data.ingredients != null)
            {
                for (int i = 0; i < data.ingredients.Count; i++)
                {
                    var s = data.ingredients[i];
                    var ing = new IngredientCount();
                    bool anyAllowed = false;
                    bool ingHasStuff = false;

                    if (s.isFilter && s.filterDefNames != null && s.filterDefNames.Count > 0)
                    {
                        for (int d = 0; d < s.filterDefNames.Count; d++)
                        {
                            var td = DefDatabase<ThingDef>.GetNamed(s.filterDefNames[d], false);
                            if (td == null) continue;
                            ing.filter.SetAllow(td, true);
                            r.fixedIngredientFilter.SetAllow(td, true);
                            r.defaultIngredientFilter.SetAllow(td, true);
                            anyAllowed = true;
                            if (td.IsStuff) ingHasStuff = true;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(s.thingDefName)) continue;
                        var td = DefDatabase<ThingDef>.GetNamed(s.thingDefName, false);
                        if (td == null) continue;
                        ing.filter.SetAllow(td, true);
                        r.fixedIngredientFilter.SetAllow(td, true);
                        r.defaultIngredientFilter.SetAllow(td, true);
                        anyAllowed = true;
                        if (td.IsStuff) ingHasStuff = true;
                    }

                    if (anyAllowed)
                    {
                        ing.SetBaseCount(Mathf.Max(1, s.count));
                        if (r.ingredients.Count == 0 && ingHasStuff)
                        {
                            firstIngredientHasStuff = true;
                        }
                        if (ingHasStuff) hasStuffIngredient = true;
                        r.ingredients.Add(ing);
                    }
                }
            }

            // Ensure GenRecipe.MakeRecipeProducts always receives a non-null
            // dominantIngredient for MadeFromStuff products: use ingredient 0 if
            // it is stuff (matching RecipeDefGenerator) or if no ingredient is
            // stuff (avoiding a NullReferenceException in MakeRecipeProducts).
            r.productHasIngredientStuff = prodDef.MadeFromStuff &&
                (firstIngredientHasStuff || (!hasStuffIngredient && r.ingredients.Count > 0));

            // Workbenches
            if (r.recipeUsers != null)
            {
                for (int i = 0; i < r.recipeUsers.Count; i++)
                {
                    var oldWb = r.recipeUsers[i];
                    if (oldWb != null)
                    {
                        oldWb.recipes?.Remove(r);
                        allRecipesCachedField?.SetValue(oldWb, null);
                    }
                }
                r.recipeUsers.Clear();
            }
            else
            {
                r.recipeUsers = new List<ThingDef>();
            }

            if (data.workbenchDefNames != null)
            {
                for (int i = 0; i < data.workbenchDefNames.Count; i++)
                {
                    string wbName = data.workbenchDefNames[i];
                    var wb = DefDatabase<ThingDef>.GetNamed(wbName, false);
                    if (wb != null)
                    {
                        wb.recipes?.Remove(r); // ensure wb.recipes does NOT contain r to prevent duplicates
                        if (!r.recipeUsers.Contains(wb)) r.recipeUsers.Add(wb);
                        allRecipesCachedField?.SetValue(wb, null); // invalidate cache so RimWorld cleanly re-evaluates AllRecipes
                    }
                }
            }

            // Research Prerequisites
            r.researchPrerequisites = new List<ResearchProjectDef>();
            if (data.researchPrerequisiteDefNames != null)
            {
                for (int i = 0; i < data.researchPrerequisiteDefNames.Count; i++)
                {
                    string resName = data.researchPrerequisiteDefNames[i];
                    var res = DefDatabase<ResearchProjectDef>.GetNamed(resName, false);
                    if (res != null) r.researchPrerequisites.Add(res);
                }
            }
            if (r.researchPrerequisites.Count == 1) r.researchPrerequisite = r.researchPrerequisites[0];
            else r.researchPrerequisite = null;

            // Skill requirement & workSkill for quality-capable products
            r.workSkill = prodDef.HasComp<CompQuality>()
                ? (prodDef.recipeMaker?.workSkill ?? SkillDefOf.Crafting)
                : null;

            if (data.minSkillLevel > 0)
            {
                r.skillRequirements = new List<SkillRequirement>
                {
                    new SkillRequirement { skill = SkillDefOf.Crafting, minLevel = data.minSkillLevel }
                };
            }
            else
            {
                r.skillRequirements = null;
            }

            // Resolve filters/stat references for the runtime-created recipe.
            r.ResolveReferences();

            // Register in DefDatabase
            if (DefDatabase<RecipeDef>.GetNamed(data.defName, false) == null)
            {
                DefDatabase<RecipeDef>.Add(r);
            }

            var disableTweak = RecipesModule.ModSettings?.tweaks?.Find(t => t.recipeDefName == data.defName && t.disabled);
            if (disableTweak != null)
            {
                RecipePatchApplier.VanillaSnapshots.Remove(data.defName);
                RecipePatchApplier.ApplyTweak(r, disableTweak);
            }

            // Add to settings
            if (addToSettings)
            {
                var settings = RecipesModule.ModSettings;
                if (settings != null)
                {
                    if (settings.customRecipes == null) settings.customRecipes = new List<CustomRecipeData>();
                    settings.customRecipes.RemoveAll(x => x.defName == data.defName);
                    settings.customRecipes.Add(data);
                    settings.Write();
                }
            }

            RecipeCache.Rebuild();
            return r;
        }

        public static void RemoveCustomRecipe(CustomRecipeData data)
        {
            if (data == null) return;
            var r = DefDatabase<RecipeDef>.GetNamed(data.defName, false);
            if (r != null)
            {
                var maps = Find.Maps;
                if (maps != null)
                {
                    for (int m = 0; m < maps.Count; m++)
                    {
                        var buildings = maps[m]?.listerBuildings?.allBuildingsColonist;
                        if (buildings == null) continue;
                        for (int i = 0; i < buildings.Count; i++)
                        {
                            if (buildings[i] is IBillGiver bg && bg.BillStack != null)
                            {
                                var bills = bg.BillStack.Bills;
                                for (int j = bills.Count - 1; j >= 0; j--)
                                {
                                    if (bills[j]?.recipe == r)
                                    {
                                        bg.BillStack.Delete(bills[j]);
                                    }
                                }
                            }
                        }
                    }
                }

                if (BillUtility.Clipboard?.recipe == r)
                {
                    BillUtility.Clipboard = null;
                }

                if (r.recipeUsers != null)
                {
                    for (int i = 0; i < r.recipeUsers.Count; i++)
                    {
                        var wb = r.recipeUsers[i];
                        if (wb != null)
                        {
                            wb.recipes?.Remove(r);
                            allRecipesCachedField?.SetValue(wb, null);
                        }
                    }
                    r.recipeUsers.Clear();
                }

                var allThings = DefDatabase<ThingDef>.AllDefsListForReading;
                if (allThings != null)
                {
                    for (int i = 0; i < allThings.Count; i++)
                    {
                        var wb = allThings[i];
                        if (wb != null)
                        {
                            wb.recipes?.Remove(r);
                            allRecipesCachedField?.SetValue(wb, null);
                        }
                    }
                }

                var removeMethod = typeof(DefDatabase<RecipeDef>).GetMethod("Remove", BindingFlags.NonPublic | BindingFlags.Static);
                removeMethod?.Invoke(null, new object[] { r });
            }

            RecipePatchApplier.VanillaSnapshots.Remove(data.defName);

            var settings = RecipesModule.ModSettings;
            if (settings != null)
            {
                settings.customRecipes?.RemoveAll(x => x.defName == data.defName);
                settings.tweaks?.RemoveAll(x => x.recipeDefName == data.defName);
                settings.Write();
            }

            RecipeCache.Rebuild();
        }
    }
}
