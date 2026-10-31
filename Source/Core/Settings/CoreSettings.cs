using SadSeal.Core.Registry;
using Verse;

namespace SadSeal.Core.Settings
{
    public class CoreSettings : ModSettings
    {
        public class ModuleStateContainer : IExposable
        {
            public void ExposeData()
            {
                foreach (var module in ModuleManager.Modules)
                {
                    bool enabled = module.IsEnabled;
                    string key = "enable" + module.Id;
                    Scribe_Values.Look(ref enabled, key, module.DefaultEnabled);
                    module.IsEnabled = enabled;
                }
            }
        }

        private ModuleStateContainer modulesContainer = new ModuleStateContainer();

        public override void ExposeData()
        {
            base.ExposeData();

            if (modulesContainer == null)
            {
                modulesContainer = new ModuleStateContainer();
            }

            Scribe_Deep.Look(ref modulesContainer, "modules");

            // Delegate to all discovered modules for their typed settings
            ModuleManager.ExposeAllData();
        }
    }
}
