using RimWorld;
using Verse;

namespace SadSeal.MegaMod.QoL
{
    [DefOf]
    public static class SadQoLDefOf
    {
        public static JobDef SadQoL_CalmDown;

        static SadQoLDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SadQoLDefOf));
        }
    }
}
