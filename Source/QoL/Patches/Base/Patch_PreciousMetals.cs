using Verse;

namespace SadSeal.MegaMod.QoL.Patches.Base
{
    [StaticConstructorOnStartup]
    public static class Patch_PreciousMetals
    {
        static Patch_PreciousMetals()
        {
            if (MegaMod.Settings?.QoL?.prettyPreciousMetals == true)
            {
                QoLApplier.ApplyPreciousMetals(true);
            }
        }
    }
}
