using SadSeal.MegaMod.Combat;
using SadSeal.MegaMod.Recipes;
using SadSeal.MegaMod.Skills;
using Verse;

namespace SadSeal.MegaMod
{
    public class MegaModSettings : ModSettings
    {
        public CombatSettings Combat = new CombatSettings();
        public SkillSettings Skills = new SkillSettings();
        public RecipeSettings Recipes = new RecipeSettings();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref Combat, "combat");
            Scribe_Deep.Look(ref Skills, "skills");
            Scribe_Deep.Look(ref Recipes, "recipes");

            if (Combat == null) Combat = new CombatSettings();
            if (Skills == null) Skills = new SkillSettings();
            if (Recipes == null) Recipes = new RecipeSettings();
        }
    }
}
