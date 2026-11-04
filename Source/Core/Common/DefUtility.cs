using System;
using System.Reflection;
using RimWorld;
using Verse;

namespace SadSeal.Common
{
    public static class DefUtility
    {
        private static readonly FieldInfo allRecipesCachedField =
            typeof(ThingDef).GetField("allRecipesCached", BindingFlags.Instance | BindingFlags.NonPublic);

        public static void HideThingDef(string defName)
        {
            var def = DefDatabase<ThingDef>.GetNamedSilentFail(defName);
            if (def == null) return;

            def.designationCategory = null;

            // The architect menu may already have been resolved. Remove any
            // Designator_Build entries that point at the hidden def.
            var categories = DefDatabase<DesignationCategoryDef>.AllDefsListForReading;
            for (int c = 0; c < categories.Count; c++)
            {
                var resolved = categories[c]?.AllResolvedDesignators;
                if (resolved == null) continue;

                for (int i = resolved.Count - 1; i >= 0; i--)
                {
                    if (resolved[i] is Designator_Build build && build.PlacingDef == def)
                    {
                        resolved.RemoveAt(i);
                    }
                }
            }
        }

        public static void HideRecipeDef(string defName)
        {
            var def = DefDatabase<RecipeDef>.GetNamedSilentFail(defName);
            if (def == null) return;

            def.recipeUsers?.Clear();

            // Recipes may also be linked through ThingDef.recipes (as our own
            // autopsy/quarry defs are). Remove those links and invalidate the
            // cached AllRecipes list so the bill disappears from workbenches.
            var allThings = DefDatabase<ThingDef>.AllDefsListForReading;
            for (int i = 0; i < allThings.Count; i++)
            {
                var td = allThings[i];
                if (td?.recipes == null) continue;
                if (td.recipes.Remove(def))
                {
                    allRecipesCachedField?.SetValue(td, null);
                }
            }
        }

        public static void DisableWorkGiver(string defName)
        {
            var def = DefDatabase<WorkGiverDef>.GetNamedSilentFail(defName);
            if (def != null)
            {
                var workTypes = DefDatabase<WorkTypeDef>.AllDefsListForReading;
                for (int i = 0; i < workTypes.Count; i++)
                {
                    workTypes[i].workGiversByPriority?.Remove(def);
                }
            }
        }

        /// <summary>
        /// Removes designator classes (by full type name) from every designation category.
        /// Used to hide designators that were injected by XML when a module is disabled.
        /// </summary>
        public static void RemoveDesignators(params string[] typeNames)
        {
            if (typeNames == null || typeNames.Length == 0) return;

            var categories = DefDatabase<DesignationCategoryDef>.AllDefsListForReading;
            for (int i = 0; i < categories.Count; i++)
            {
                var list = categories[i]?.specialDesignatorClasses;
                if (list != null)
                {
                    for (int j = list.Count - 1; j >= 0; j--)
                    {
                        Type type = list[j];
                        if (type == null) continue;
                        for (int k = 0; k < typeNames.Length; k++)
                        {
                            if (string.Equals(type.FullName, typeNames[k], StringComparison.Ordinal))
                            {
                                list.RemoveAt(j);
                                break;
                            }
                        }
                    }
                }

                // Also prune an already-resolved designator list.
                var resolved = categories[i]?.AllResolvedDesignators;
                if (resolved == null) continue;

                for (int j = resolved.Count - 1; j >= 0; j--)
                {
                    var designator = resolved[j];
                    if (designator == null) continue;
                    string fullName = designator.GetType().FullName;
                    for (int k = 0; k < typeNames.Length; k++)
                    {
                        if (string.Equals(fullName, typeNames[k], StringComparison.Ordinal))
                        {
                            resolved.RemoveAt(j);
                            break;
                        }
                    }
                }
            }
        }
    }
}

namespace SadSeal.Core.Common
{
    // Alias for explicit SadSeal.Core.Common namespace references
    public static class DefUtility
    {
        public static void HideThingDef(string defName) => SadSeal.Common.DefUtility.HideThingDef(defName);
        public static void HideRecipeDef(string defName) => SadSeal.Common.DefUtility.HideRecipeDef(defName);
        public static void DisableWorkGiver(string defName) => SadSeal.Common.DefUtility.DisableWorkGiver(defName);
        public static void RemoveDesignators(params string[] typeNames) => SadSeal.Common.DefUtility.RemoveDesignators(typeNames);
    }
}
