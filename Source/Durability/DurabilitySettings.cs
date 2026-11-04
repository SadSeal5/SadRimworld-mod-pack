using Verse;

namespace SadSeal.MegaMod.Durability
{
    public enum DurabilityPreset
    {
        Custom,
        AllDisabled,
        WeaponsOnly,
        ApparelOnly,
        AllImmortal
    }

    public class DurabilitySettings : IExposable
    {
        public bool removeApparelDurability = true;
        public bool removeWeaponDurability = true;
        public bool restoreExistingOnLoad = true;
        public bool preventTaintedApparel = false;
        public DurabilityPreset ActivePreset = DurabilityPreset.AllImmortal;

        public void ExposeData()
        {
            Scribe_Values.Look(ref removeApparelDurability, "removeApparelDurability", true);
            Scribe_Values.Look(ref removeWeaponDurability, "removeWeaponDurability", true);
            Scribe_Values.Look(ref restoreExistingOnLoad, "restoreExistingOnLoad", true);
            Scribe_Values.Look(ref preventTaintedApparel, "preventTaintedApparel", false);
            Scribe_Values.Look(ref ActivePreset, "activePreset", DurabilityPreset.AllImmortal);
        }

        public void ApplyPreset(DurabilityPreset preset)
        {
            ActivePreset = preset;
            switch (preset)
            {
                case DurabilityPreset.AllDisabled:
                    removeApparelDurability = false;
                    removeWeaponDurability = false;
                    break;
                case DurabilityPreset.WeaponsOnly:
                    removeApparelDurability = false;
                    removeWeaponDurability = true;
                    break;
                case DurabilityPreset.ApparelOnly:
                    removeApparelDurability = true;
                    removeWeaponDurability = false;
                    break;
                case DurabilityPreset.AllImmortal:
                    removeApparelDurability = true;
                    removeWeaponDurability = true;
                    break;
            }
        }

        public void ResetToDefaults()
        {
            ApplyPreset(DurabilityPreset.AllImmortal);
            restoreExistingOnLoad = true;
            preventTaintedApparel = false;
        }
    }
}
