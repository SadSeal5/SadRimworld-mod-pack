using Verse;

namespace SadSeal.MegaMod
{
    [StaticConstructorOnStartup]
    public static class MegaModDefApplier
    {
        static MegaModDefApplier()
        {
            var modules = MegaMod.Settings?.Modules;
            if (modules == null) return;

            if (!modules.enableQuarry)
            {
                var quarryDef = DefDatabase<ThingDef>.GetNamedSilentFail("Sad_DeepQuarry");
                if (quarryDef != null)
                {
                    quarryDef.designationCategory = null;
                }
            }

            if (!modules.enableMedical)
            {
                var autopsyDef = DefDatabase<ThingDef>.GetNamedSilentFail("SadMedical_AutopsyTable");
                if (autopsyDef != null)
                {
                    autopsyDef.designationCategory = null;
                }
            }
        }
    }
}
