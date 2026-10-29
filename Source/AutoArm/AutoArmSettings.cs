using System.Collections.Generic;
using Verse;

namespace SadSeal.MegaMod.AutoArm
{
    public class AutoArmSettings : IExposable
    {
        public bool enableAutoArm = true;
        public bool onlyFromStockpile = true;
        public float minScoreDifference = 0.15f;
        public int checkIntervalSeconds = 30;
        public bool autoAssignBySkill = true;
        public bool considerQuality = true;
        public bool considerHitPoints = true;
        public bool autoUnforbidDropped = true;

        public HashSet<string> blacklistedWeapons = new HashSet<string>();

        public AutoArmSettings()
        {
            PopulateDefaultBlacklist();
        }

        public bool IsBlacklisted(ThingDef def)
        {
            if (def == null || blacklistedWeapons == null) return false;
            return blacklistedWeapons.Contains(def.defName);
        }

        public void PopulateDefaultBlacklist()
        {
            if (blacklistedWeapons == null)
            {
                blacklistedWeapons = new HashSet<string>();
            }

            blacklistedWeapons.Add("Gun_DoomsdayRocket");
            blacklistedWeapons.Add("Gun_TripleRocket");
            blacklistedWeapons.Add("Weapon_GrenadeFrag");
            blacklistedWeapons.Add("Weapon_GrenadeMolotov");
            blacklistedWeapons.Add("Weapon_GrenadeEMP");
            blacklistedWeapons.Add("Gun_SmokeLauncher");
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref enableAutoArm, "enableAutoArm", true);
            Scribe_Values.Look(ref onlyFromStockpile, "onlyFromStockpile", true);
            Scribe_Values.Look(ref minScoreDifference, "minScoreDifference", 0.15f);
            Scribe_Values.Look(ref checkIntervalSeconds, "checkIntervalSeconds", 30);
            Scribe_Values.Look(ref autoAssignBySkill, "autoAssignBySkill", true);
            Scribe_Values.Look(ref considerQuality, "considerQuality", true);
            Scribe_Values.Look(ref considerHitPoints, "considerHitPoints", true);
            Scribe_Values.Look(ref autoUnforbidDropped, "autoUnforbidDropped", true);

            Scribe_Collections.Look(ref blacklistedWeapons, "blacklistedWeapons", LookMode.Value);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (blacklistedWeapons == null)
                {
                    blacklistedWeapons = new HashSet<string>();
                    PopulateDefaultBlacklist();
                }
            }
        }
    }
}
