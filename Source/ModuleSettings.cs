using Verse;

namespace SadSeal.MegaMod
{
    public class ModuleSettings : IExposable
    {
        public bool enableCombat = true;
        public bool enableSkills = true;
        public bool enableRecipes = true;
        public bool enableGenes = true;
        public bool enableHauling = true;
        public bool enableWorkManager = true;
        public bool enableDurability = true;
        public bool enableQoL = true;
        public bool enableMedical = true;
        public bool enableAllowTool = true;
        public bool enableTimeFlow = true;
        public bool enableQuarry = true;

        public void ExposeData()
        {
            Scribe_Values.Look(ref enableCombat, "enableCombat", true);
            Scribe_Values.Look(ref enableSkills, "enableSkills", true);
            Scribe_Values.Look(ref enableRecipes, "enableRecipes", true);
            Scribe_Values.Look(ref enableGenes, "enableGenes", true);
            Scribe_Values.Look(ref enableHauling, "enableHauling", true);
            Scribe_Values.Look(ref enableWorkManager, "enableWorkManager", true);
            Scribe_Values.Look(ref enableDurability, "enableDurability", true);
            Scribe_Values.Look(ref enableQoL, "enableQoL", true);
            Scribe_Values.Look(ref enableMedical, "enableMedical", true);
            Scribe_Values.Look(ref enableAllowTool, "enableAllowTool", true);
            Scribe_Values.Look(ref enableTimeFlow, "enableTimeFlow", true);
            Scribe_Values.Look(ref enableQuarry, "enableQuarry", true);
        }

        public bool IsModuleEnabled(MegaModTab tab)
        {
            return tab switch
            {
                MegaModTab.Modules => true,
                MegaModTab.Combat => enableCombat,
                MegaModTab.Skills => enableSkills,
                MegaModTab.Recipes => enableRecipes,
                MegaModTab.Genes => enableGenes,
                MegaModTab.Hauling => enableHauling,
                MegaModTab.WorkManager => enableWorkManager,
                MegaModTab.Durability => enableDurability,
                MegaModTab.QoL => enableQoL,
                MegaModTab.Medical => enableMedical,
                MegaModTab.AllowTool => enableAllowTool,
                MegaModTab.TimeFlow => enableTimeFlow,
                MegaModTab.Quarry => enableQuarry,
                _ => true
            };
        }

        public void SetAll(bool enabled)
        {
            enableCombat = enabled;
            enableSkills = enabled;
            enableRecipes = enabled;
            enableGenes = enabled;
            enableHauling = enabled;
            enableWorkManager = enabled;
            enableDurability = enabled;
            enableQoL = enabled;
            enableMedical = enabled;
            enableAllowTool = enabled;
            enableTimeFlow = enabled;
            enableQuarry = enabled;
        }
    }
}
