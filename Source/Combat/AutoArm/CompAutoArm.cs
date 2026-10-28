using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Combat.AutoArm
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
            if (parent is Pawn pawn && pawn.Faction == Faction.OfPlayer && pawn.RaceProps.Humanlike && !pawn.Dead && pawn.IsColonist)
            {
                Command_Action cmd = new Command_Action();
                cmd.defaultLabel = GetGizmoLabel();
                cmd.defaultDesc = "Auto-Arm: Otomatik silah yükseltme ayarları. Tıklayarak kategoriyi değiştirin veya kilitleyin.";
                cmd.icon = ContentFinder<Texture2D>.Get("UI/Commands/Attack", false) ?? BaseContent.WhiteTex;
                cmd.action = () =>
                {
                    List<FloatMenuOption> options = new List<FloatMenuOption>();
                    options.Add(new FloatMenuOption("Dinamik (Elindeki Silahı Takip Et)", () =>
                    {
                        preferredCategory = WeaponCategory.None;
                        isCategoryLocked = false;
                        enabled = true;
                    }));
                    options.Add(new FloatMenuOption("Kilitli: Otomatik / Seri Atımlı (Burst)", () =>
                    {
                        preferredCategory = WeaponCategory.RangedBurst;
                        isCategoryLocked = true;
                        enabled = true;
                    }));
                    options.Add(new FloatMenuOption("Kilitli: Keskin Nişancı / Tüfek (Precision)", () =>
                    {
                        preferredCategory = WeaponCategory.RangedPrecision;
                        isCategoryLocked = true;
                        enabled = true;
                    }));
                    options.Add(new FloatMenuOption("Kilitli: Kesici Yakın Dövüş (Sharp)", () =>
                    {
                        preferredCategory = WeaponCategory.MeleeSharp;
                        isCategoryLocked = true;
                        enabled = true;
                    }));
                    options.Add(new FloatMenuOption("Kilitli: Ezici Yakın Dövüş (Blunt)", () =>
                    {
                        preferredCategory = WeaponCategory.MeleeBlunt;
                        isCategoryLocked = true;
                        enabled = true;
                    }));
                    options.Add(new FloatMenuOption("Devre Dışı", () =>
                    {
                        enabled = false;
                    }));

                    Find.WindowStack.Add(new FloatMenu(options));
                };

                yield return cmd;
            }
        }

        private string GetGizmoLabel()
        {
            if (!enabled) return "Auto-Arm: Kapalı";
            if (!isCategoryLocked || preferredCategory == WeaponCategory.None) return "Auto-Arm: Dinamik";
            switch (preferredCategory)
            {
                case WeaponCategory.RangedBurst: return "Auto-Arm: Seri Atım";
                case WeaponCategory.RangedPrecision: return "Auto-Arm: Tüfek/Sniper";
                case WeaponCategory.MeleeSharp: return "Auto-Arm: Kesici";
                case WeaponCategory.MeleeBlunt: return "Auto-Arm: Ezici";
                default: return "Auto-Arm: Dinamik";
            }
        }
    }
}
