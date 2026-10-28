using Verse;

namespace SadSeal.MegaMod.QoL.Patches.Base
{
    [StaticConstructorOnStartup]
    public static class Patch_StackableChunks
    {
        static Patch_StackableChunks()
        {
            var settings = MegaMod.Settings?.QoL;
            if (settings != null && settings.stackableChunks)
            {
                QoLApplier.ApplyChunkStackLimits(true, settings.chunkStackLimit);
            }
        }
    }
}
