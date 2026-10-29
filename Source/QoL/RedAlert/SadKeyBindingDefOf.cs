using RimWorld;
using Verse;

namespace SadSeal.MegaMod.QoL.RedAlert
{
    [DefOf]
    public static class SadKeyBindingDefOf
    {
        public static KeyBindingDef Sad_KeyBinding_RedAlert;

        static SadKeyBindingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SadKeyBindingDefOf));
        }
    }
}
