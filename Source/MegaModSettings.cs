using SadSeal.MegaMod.Combat;
using SadSeal.MegaMod.Genes;
using SadSeal.MegaMod.Hauling;
using SadSeal.MegaMod.Recipes;
using SadSeal.MegaMod.Skills;
using SadSeal.MegaMod.WorkManager;
using Verse;

namespace SadSeal.MegaMod
{
    public class MegaModSettings : ModSettings
    {
        public CombatSettings Combat = new CombatSettings();
        public SkillSettings Skills = new SkillSettings();
        public RecipeSettings Recipes = new RecipeSettings();
        public GeneSynthesizerSettings Genes = new GeneSynthesizerSettings();
        public HaulSettings Hauling = new HaulSettings();
        public WorkManagerSettings WorkManager = new WorkManagerSettings();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref Combat, "combat");
            Scribe_Deep.Look(ref Skills, "skills");
            Scribe_Deep.Look(ref Recipes, "recipes");
            Scribe_Deep.Look(ref Genes, "genes");
            Scribe_Deep.Look(ref Hauling, "hauling");
            Scribe_Deep.Look(ref WorkManager, "workManager");

            if (Combat == null) Combat = new CombatSettings();
            if (Skills == null) Skills = new SkillSettings();
            if (Recipes == null) Recipes = new RecipeSettings();
            if (Genes == null) Genes = new GeneSynthesizerSettings();
            if (Hauling == null) Hauling = new HaulSettings();
            if (WorkManager == null) WorkManager = new WorkManagerSettings();
        }
    }
}
