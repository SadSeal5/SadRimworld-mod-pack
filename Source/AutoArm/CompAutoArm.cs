using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.AutoArm
{
    public class CompProperties_AutoArm : CompProperties
    {
        public CompProperties_AutoArm()
        {
            compClass = typeof(CompAutoArm);
        }
    }

    public class CompAutoArm : ThingComp
    {
        public bool enabled = true;
        public WeaponCategory preferredCategory = WeaponCategory.None;
        public bool isCategoryLocked = false;
        public int nextCheckTick = 0;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref enabled, "autoArm_enabled", true);
            Scribe_Values.Look(ref preferredCategory, "autoArm_preferredCategory", WeaponCategory.None);
            Scribe_Values.Look(ref isCategoryLocked, "autoArm_isCategoryLocked", false);
            Scribe_Values.Look(ref nextCheckTick, "autoArm_nextCheckTick", 0);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield break;
        }
    }
}
