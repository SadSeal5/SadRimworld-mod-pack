using SadSeal.MegaMod.AllowTool;
using SadSeal.MegaMod.Combat;
using SadSeal.MegaMod.Durability;
using SadSeal.MegaMod.Genes;
using SadSeal.MegaMod.Hauling;
using SadSeal.MegaMod.Medical;
using SadSeal.MegaMod.QoL;
using SadSeal.MegaMod.Quarry;
using SadSeal.MegaMod.Recipes;
using SadSeal.MegaMod.Skills;
using SadSeal.MegaMod.TimeFlow;
using SadSeal.MegaMod.WorkManager;
using Verse;

namespace SadSeal.MegaMod
{
    public class MegaModSettings : ModSettings
    {
        public ModuleSettings Modules = new ModuleSettings();
        public CombatSettings Combat = new CombatSettings();
        public SkillSettings Skills = new SkillSettings();
        public RecipeSettings Recipes = new RecipeSettings();
        public GeneSynthesizerSettings Genes = new GeneSynthesizerSettings();
        public HaulSettings Hauling = new HaulSettings();
        public WorkManagerSettings WorkManager = new WorkManagerSettings();
        public DurabilitySettings Durability = new DurabilitySettings();
        public QoLSettings QoL = new QoLSettings();
        public MedicalSettings Medical = new MedicalSettings();
        public AllowToolSettings AllowTool = new AllowToolSettings();
        public TimeFlowSettings TimeFlow = new TimeFlowSettings();
        public QuarrySettings Quarry = new QuarrySettings();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref Modules, "modules");
            Scribe_Deep.Look(ref Combat, "combat");
            Scribe_Deep.Look(ref Skills, "skills");
            Scribe_Deep.Look(ref Recipes, "recipes");
            Scribe_Deep.Look(ref Genes, "genes");
            Scribe_Deep.Look(ref Hauling, "hauling");
            Scribe_Deep.Look(ref WorkManager, "workManager");
            Scribe_Deep.Look(ref Durability, "durability");
            Scribe_Deep.Look(ref QoL, "qol");
            Scribe_Deep.Look(ref Medical, "medical");
            Scribe_Deep.Look(ref AllowTool, "allowTool");
            Scribe_Deep.Look(ref TimeFlow, "timeFlow");
            Scribe_Deep.Look(ref Quarry, "quarry");

            if (Modules == null) Modules = new ModuleSettings();
            if (Combat == null) Combat = new CombatSettings();
            if (Skills == null) Skills = new SkillSettings();
            if (Recipes == null) Recipes = new RecipeSettings();
            if (Genes == null) Genes = new GeneSynthesizerSettings();
            if (Hauling == null) Hauling = new HaulSettings();
            if (WorkManager == null) WorkManager = new WorkManagerSettings();
            if (Durability == null) Durability = new DurabilitySettings();
            if (QoL == null) QoL = new QoLSettings();
            if (Medical == null) Medical = new MedicalSettings();
            if (AllowTool == null) AllowTool = new AllowToolSettings();
            if (TimeFlow == null) TimeFlow = new TimeFlowSettings();
            if (Quarry == null) Quarry = new QuarrySettings();
        }
    }
}
