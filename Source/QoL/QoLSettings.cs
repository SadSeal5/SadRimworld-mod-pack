using Verse;

namespace SadSeal.MegaMod.QoL
{
    public class QoLSettings : IExposable
    {
        // Pawn Behavior
        public bool ughYouGotMe = true;
        public bool whereIsMyWeapon = true;
        public bool prioritizeSleep = true;
        public bool dontBlockDoors = true;

        // Base & Resources
        public bool metalDoesntBurn = true;
        public bool filthVanishes = true;
        public bool stackableChunks = true;
        public int chunkStackLimit = 5;
        public bool treesDontDie = true;
        public bool prettyPreciousMetals = true;

        // Work & Crafting
        public bool noJobAuthors = true;
        public bool smoothWallsBeforeFloors = true;
        public bool repairInZone = true;
        public bool haulAfterSlaughter = true;

        // Raids & Alerts
        public bool disableBreachRaids = false;
        public bool disableDropPodRaids = false;
        public bool noDeathAcidifiers = true;
        public bool traderAlerts = true;

        public void ExposeData()
        {
            // Pawn
            Scribe_Values.Look(ref ughYouGotMe, "ughYouGotMe", true);
            Scribe_Values.Look(ref whereIsMyWeapon, "whereIsMyWeapon", true);
            Scribe_Values.Look(ref prioritizeSleep, "prioritizeSleep", true);
            Scribe_Values.Look(ref dontBlockDoors, "dontBlockDoors", true);

            // Base & Resources
            Scribe_Values.Look(ref metalDoesntBurn, "metalDoesntBurn", true);
            Scribe_Values.Look(ref filthVanishes, "filthVanishes", true);
            Scribe_Values.Look(ref stackableChunks, "stackableChunks", true);
            Scribe_Values.Look(ref chunkStackLimit, "chunkStackLimit", 5);
            Scribe_Values.Look(ref treesDontDie, "treesDontDie", true);
            Scribe_Values.Look(ref prettyPreciousMetals, "prettyPreciousMetals", true);

            // Work & Crafting
            Scribe_Values.Look(ref noJobAuthors, "noJobAuthors", true);
            Scribe_Values.Look(ref smoothWallsBeforeFloors, "smoothWallsBeforeFloors", true);
            Scribe_Values.Look(ref repairInZone, "repairInZone", true);
            Scribe_Values.Look(ref haulAfterSlaughter, "haulAfterSlaughter", true);

            // Raids & Alerts
            Scribe_Values.Look(ref disableBreachRaids, "disableBreachRaids", false);
            Scribe_Values.Look(ref disableDropPodRaids, "disableDropPodRaids", false);
            Scribe_Values.Look(ref noDeathAcidifiers, "noDeathAcidifiers", true);
            Scribe_Values.Look(ref traderAlerts, "traderAlerts", true);
        }

        public void SetPresetBalanced()
        {
            ughYouGotMe = true;
            whereIsMyWeapon = true;
            prioritizeSleep = true;
            dontBlockDoors = true;

            metalDoesntBurn = true;
            filthVanishes = true;
            stackableChunks = true;
            chunkStackLimit = 5;
            treesDontDie = true;
            prettyPreciousMetals = true;

            noJobAuthors = true;
            smoothWallsBeforeFloors = true;
            repairInZone = true;
            haulAfterSlaughter = true;

            disableBreachRaids = false;
            disableDropPodRaids = false;
            noDeathAcidifiers = true;
            traderAlerts = true;

            QoLApplier.ApplyAll();
        }

        public void SetPresetMaxComfort()
        {
            ughYouGotMe = true;
            whereIsMyWeapon = true;
            prioritizeSleep = true;
            dontBlockDoors = true;

            metalDoesntBurn = true;
            filthVanishes = true;
            stackableChunks = true;
            chunkStackLimit = 10;
            treesDontDie = true;
            prettyPreciousMetals = true;

            noJobAuthors = true;
            smoothWallsBeforeFloors = true;
            repairInZone = true;
            haulAfterSlaughter = true;

            disableBreachRaids = true;
            disableDropPodRaids = true;
            noDeathAcidifiers = true;
            traderAlerts = true;

            QoLApplier.ApplyAll();
        }

        public void SetPresetVanilla()
        {
            ughYouGotMe = false;
            whereIsMyWeapon = false;
            prioritizeSleep = false;
            dontBlockDoors = false;

            metalDoesntBurn = false;
            filthVanishes = false;
            stackableChunks = false;
            chunkStackLimit = 1;
            treesDontDie = false;
            prettyPreciousMetals = false;

            noJobAuthors = false;
            smoothWallsBeforeFloors = false;
            repairInZone = false;
            haulAfterSlaughter = false;

            disableBreachRaids = false;
            disableDropPodRaids = false;
            noDeathAcidifiers = false;
            traderAlerts = false;

            QoLApplier.ApplyAll();
        }

        public void ResetToDefaults()
        {
            SetPresetBalanced();
        }
    }
}
