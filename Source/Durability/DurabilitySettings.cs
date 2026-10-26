using Verse;

namespace SadSeal.MegaMod.Durability
{
    public class DurabilitySettings : IExposable
    {
        public bool removeApparelDurability = true;
        public bool removeWeaponDurability = true;
        public bool restoreExistingOnLoad = true;
        public bool preventTaintedApparel = false;

        public void ExposeData()
        {
            Scribe_Values.Look(ref removeApparelDurability, "removeApparelDurability", true);
            Scribe_Values.Look(ref removeWeaponDurability, "removeWeaponDurability", true);
            Scribe_Values.Look(ref restoreExistingOnLoad, "restoreExistingOnLoad", true);
            Scribe_Values.Look(ref preventTaintedApparel, "preventTaintedApparel", false);
        }

        public void ResetToDefaults()
        {
            removeApparelDurability = true;
            removeWeaponDurability = true;
            restoreExistingOnLoad = true;
            preventTaintedApparel = false;
        }
    }
}
