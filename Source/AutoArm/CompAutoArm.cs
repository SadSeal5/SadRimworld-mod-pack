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
            if (AutoArmModule.Instance?.IsEnabled != true) yield break;

            Pawn pawn = parent as Pawn;
            if (pawn == null || pawn.Faction != Faction.OfPlayer || !pawn.IsColonist) yield break;

            yield return new Command_Toggle
            {
                defaultLabel = "MegaMod_AutoArm_GizmoEnable".Translate(),
                defaultDesc = "MegaMod_AutoArm_GizmoEnableDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/ForbidOff", false) ?? TexCommand.DesirePower,
                isActive = () => enabled,
                toggleAction = () => { enabled = !enabled; }
            };

            yield return new Command_Action
            {
                defaultLabel = "MegaMod_AutoArm_GizmoPreference".Translate(GetPreferenceLabel()),
                defaultDesc = "MegaMod_AutoArm_GizmoPreferenceDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/Attack", false) ?? TexCommand.DesirePower,
                action = OpenPreferenceMenu
            };
        }

        private string GetPreferenceLabel()
        {
            if (!isCategoryLocked || preferredCategory == WeaponCategory.None)
            {
                return "MegaMod_AutoArm_PrefAuto".Translate();
            }
            return PreferenceLabel(preferredCategory);
        }

        private static string PreferenceLabel(WeaponCategory category)
        {
            switch (category)
            {
                case WeaponCategory.RangedSingleShot: return "MegaMod_AutoArm_PrefRangedSingle".Translate();
                case WeaponCategory.RangedBurst: return "MegaMod_AutoArm_PrefRangedBurst".Translate();
                case WeaponCategory.MeleeSharp: return "MegaMod_AutoArm_PrefMeleeSharp".Translate();
                case WeaponCategory.MeleeBlunt: return "MegaMod_AutoArm_PrefMeleeBlunt".Translate();
                default: return "MegaMod_AutoArm_PrefAuto".Translate();
            }
        }

        private void OpenPreferenceMenu()
        {
            var options = new List<FloatMenuOption>
            {
                new FloatMenuOption("MegaMod_AutoArm_PrefAuto".Translate().ToString(), () =>
                {
                    isCategoryLocked = false;
                    preferredCategory = WeaponCategory.None;
                })
            };

            AddPreference(options, WeaponCategory.RangedSingleShot, "MegaMod_AutoArm_PrefRangedSingle");
            AddPreference(options, WeaponCategory.RangedBurst, "MegaMod_AutoArm_PrefRangedBurst");
            AddPreference(options, WeaponCategory.MeleeSharp, "MegaMod_AutoArm_PrefMeleeSharp");
            AddPreference(options, WeaponCategory.MeleeBlunt, "MegaMod_AutoArm_PrefMeleeBlunt");

            Find.WindowStack.Add(new FloatMenu(options));
        }

        private void AddPreference(List<FloatMenuOption> options, WeaponCategory category, string key)
        {
            options.Add(new FloatMenuOption(key.Translate().ToString(), () =>
            {
                preferredCategory = category;
                isCategoryLocked = true;
            }));
        }
    }
}
