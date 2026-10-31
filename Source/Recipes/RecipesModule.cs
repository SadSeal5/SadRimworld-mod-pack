using SadSeal.Core.Modules;
using SadSeal.MegaMod.Recipes.UI;
using UnityEngine;

namespace SadSeal.MegaMod.Recipes
{
    public class RecipesModule : ModModule<RecipeSettings>
    {
        public static RecipesModule Instance { get; private set; }

        public override string Id => "Recipes";
        public override string DisplayNameKey => "MegaMod_TabRecipes";
        public override string DescriptionKey => "MegaMod_ModuleRecipes_Desc";
        public override int Order => 3;

        public static new RecipeSettings ModSettings => Instance?.CurrentSettings ?? ModModule<RecipeSettings>.ModSettings;
        public static new RecipeSettings Settings => ModSettings;

        public RecipesModule()
        {
            Instance = this;
        }

        public override void OnDefsLoaded()
        {
            if (IsEnabled)
            {
                RecipePatchApplier.ApplyAll();
                CustomRecipeManager.InitializeAll();
            }
        }

        public override void DrawSettings(Rect inRect)
        {
            RecipeSettingsDrawer.DrawSettings(inRect, Settings);
        }
    }
}
