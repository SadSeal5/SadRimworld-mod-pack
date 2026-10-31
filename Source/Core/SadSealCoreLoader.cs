using SadSeal.Core.Registry;
using Verse;

namespace SadSeal.Core
{
    [StaticConstructorOnStartup]
    public static class SadSealCoreLoader
    {
        static SadSealCoreLoader()
        {
            ModuleManager.OnDefsLoadedAll();
        }
    }
}
