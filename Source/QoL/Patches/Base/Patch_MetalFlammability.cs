using Verse;

namespace SadSeal.MegaMod.QoL.Patches.Base
{
    [StaticConstructorOnStartup]
    public static class Patch_MetalFlammability
    {
        static Patch_MetalFlammability()
        {
            if (MegaMod.Settings?.QoL?.metalDoesntBurn == true)
            {
                QoLApplier.ApplyMetalFlammability(true);
            }
        }
    }
}
