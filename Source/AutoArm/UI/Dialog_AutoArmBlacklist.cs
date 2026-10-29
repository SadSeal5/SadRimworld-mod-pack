using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.AutoArm.UI
{
    public class Dialog_AutoArmBlacklist : Window
    {
        public override Vector2 InitialSize => new Vector2(780f, 650f);

        private readonly AutoArmSettings settings;
        private string searchKey = string.Empty;
        private Vector2 scrollPosition = Vector2.zero;
        private static List<ThingDef> cachedWeapons;

        public Dialog_AutoArmBlacklist(AutoArmSettings settings)
        {
            this.settings = settings;
            doCloseX = true;
            doCloseButton = false;
            closeOnClickedOutside = false;
            draggable = true;
            absorbInputAroundWindow = true;

            if (cachedWeapons == null)
            {
                cachedWeapons = DefDatabase<ThingDef>.AllDefsListForReading
                    .Where(d => d.IsWeapon && !d.destroyOnDrop)
                    .OrderBy(d => d.LabelCap.Resolve())
                    .ToList();
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Rect titleRect = new Rect(inRect.x, inRect.y, inRect.width - 40f, 32f);
            Widgets.Label(titleRect, "MegaMod_AutoArm_BlacklistTitle".Translate());
            Text.Font = GameFont.Small;

            // Search row
            float searchY = inRect.y + 38f;
            Rect searchLabelRect = new Rect(inRect.x, searchY + 4f, 130f, 26f);
            Widgets.Label(searchLabelRect, "MegaMod_AutoArm_BlacklistSearch".Translate());

            Rect searchBoxRect = new Rect(inRect.x + 135f, searchY, 320f, 28f);
            searchKey = Widgets.TextField(searchBoxRect, searchKey);

            // Counter label on top right
            int blacklistedCount = settings.blacklistedWeapons?.Count ?? 0;
            Rect countRect = new Rect(inRect.xMax - 260f, searchY + 4f, 260f, 26f);
            Text.Anchor = TextAnchor.MiddleRight;
            GUI.color = new Color(0.85f, 0.85f, 0.85f);
            Widgets.Label(countRect, "MegaMod_AutoArm_BlacklistCount".Translate(blacklistedCount, cachedWeapons.Count));
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;

            // Header row
            float headerY = searchY + 36f;
            Rect headerRect = new Rect(inRect.x, headerY, inRect.width, 24f);
            Widgets.DrawLightHighlight(headerRect);

            Widgets.Label(new Rect(headerRect.x + 6f, headerRect.y + 2f, 30f, 22f), "✓");
            Widgets.Label(new Rect(headerRect.x + 40f, headerRect.y + 2f, 240f, 22f), "Weapon".Translate());
            Widgets.Label(new Rect(headerRect.x + 290f, headerRect.y + 2f, 260f, 22f), "defName");
            Widgets.Label(new Rect(headerRect.x + 560f, headerRect.y + 2f, 120f, 22f), "Category".Translate());

            // List area
            float listY = headerY + 28f;
            float bottomBarH = 44f;
            float listHeight = inRect.height - (listY - inRect.y) - bottomBarH - 8f;
            Rect outRect = new Rect(inRect.x, listY, inRect.width, listHeight);

            var matches = cachedWeapons.Where(w =>
                string.IsNullOrEmpty(searchKey) ||
                w.LabelCap.Resolve().IndexOf(searchKey, StringComparison.OrdinalIgnoreCase) >= 0 ||
                w.defName.IndexOf(searchKey, StringComparison.OrdinalIgnoreCase) >= 0
            ).ToList();

            float rowHeight = 28f;
            Rect viewRect = new Rect(0f, 0f, inRect.width - 18f, matches.Count * rowHeight);

            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);

            for (int i = 0; i < matches.Count; i++)
            {
                ThingDef def = matches[i];
                Rect rowRect = new Rect(0f, i * rowHeight, viewRect.width, rowHeight);

                if (i % 2 == 1)
                {
                    Widgets.DrawAltRect(rowRect);
                }

                if (Mouse.IsOver(rowRect))
                {
                    Widgets.DrawHighlight(rowRect);
                }

                bool isBlacklisted = settings.blacklistedWeapons != null && settings.blacklistedWeapons.Contains(def.defName);
                bool checkState = isBlacklisted;
                Widgets.Checkbox(rowRect.x + 6f, rowRect.y + 2f, ref checkState);
                if (checkState != isBlacklisted)
                {
                    if (checkState)
                    {
                        if (settings.blacklistedWeapons == null) settings.blacklistedWeapons = new HashSet<string>();
                        settings.blacklistedWeapons.Add(def.defName);
                    }
                    else
                    {
                        settings.blacklistedWeapons?.Remove(def.defName);
                    }
                }

                // Weapon Icon
                if (def.uiIcon != null)
                {
                    Widgets.DefIcon(new Rect(rowRect.x + 36f, rowRect.y + 2f, 24f, 24f), def);
                }

                // Weapon Label
                Widgets.Label(new Rect(rowRect.x + 68f, rowRect.y + 3f, 215f, 24f), def.LabelCap);

                // defName in gray
                GUI.color = new Color(0.7f, 0.7f, 0.7f);
                Widgets.Label(new Rect(rowRect.x + 290f, rowRect.y + 3f, 260f, 24f), def.defName);
                GUI.color = Color.white;

                // Category
                string categoryStr = def.IsRangedWeapon
                    ? "MegaMod_AutoArm_CategoryRanged".Translate()
                    : "MegaMod_AutoArm_CategoryMelee".Translate();
                Widgets.Label(new Rect(rowRect.x + 560f, rowRect.y + 3f, 120f, 24f), categoryStr);

                // Row click toggles blacklist
                if (Widgets.ButtonInvisible(new Rect(rowRect.x + 36f, rowRect.y, rowRect.width - 36f, rowRect.height)))
                {
                    if (isBlacklisted)
                    {
                        settings.blacklistedWeapons?.Remove(def.defName);
                    }
                    else
                    {
                        if (settings.blacklistedWeapons == null) settings.blacklistedWeapons = new HashSet<string>();
                        settings.blacklistedWeapons.Add(def.defName);
                    }
                }
            }

            Widgets.EndScrollView();

            // Bottom action buttons
            float btnY = inRect.yMax - bottomBarH + 6f;
            float btnH = 34f;
            float btnGap = 8f;
            float totalBtnW = inRect.width;
            float btnW = (totalBtnW - (btnGap * 3f)) / 4f;

            Rect btn1 = new Rect(inRect.x, btnY, btnW, btnH);
            Rect btn2 = new Rect(btn1.xMax + btnGap, btnY, btnW, btnH);
            Rect btn3 = new Rect(btn2.xMax + btnGap, btnY, btnW, btnH);
            Rect btn4 = new Rect(btn3.xMax + btnGap, btnY, btnW, btnH);

            if (Widgets.ButtonText(btn1, "MegaMod_AutoArm_ExcludeAllExplosives".Translate()))
            {
                ExcludeAllExplosives();
            }

            if (Widgets.ButtonText(btn2, "MegaMod_AutoArm_ResetDefaults".Translate()))
            {
                if (settings.blacklistedWeapons == null) settings.blacklistedWeapons = new HashSet<string>();
                settings.blacklistedWeapons.Clear();
                settings.PopulateDefaultBlacklist();
            }

            if (Widgets.ButtonText(btn3, "MegaMod_AutoArm_ClearAll".Translate()))
            {
                settings.blacklistedWeapons?.Clear();
            }

            if (Widgets.ButtonText(btn4, "MegaMod_AutoArm_Close".Translate()))
            {
                Close();
            }
        }

        private void ExcludeAllExplosives()
        {
            if (settings.blacklistedWeapons == null) settings.blacklistedWeapons = new HashSet<string>();

            foreach (var w in cachedWeapons)
            {
                string lower = w.defName.ToLowerInvariant();
                bool isExplosive = lower.Contains("grenade") || lower.Contains("molotov") ||
                                  lower.Contains("doomsday") || lower.Contains("rocket") ||
                                  lower.Contains("orbital") || lower.Contains("tornado") ||
                                  lower.Contains("emp") || lower.Contains("smoke") ||
                                  lower.Contains("firefoam") || lower.Contains("triple") ||
                                  lower.Contains("mortar") || lower.Contains("bomb");

                if (!isExplosive && w.Verbs != null)
                {
                    foreach (var verb in w.Verbs)
                    {
                        if (verb.defaultProjectile?.projectile != null)
                        {
                            var proj = verb.defaultProjectile.projectile;
                            if (proj.explosionRadius > 0f) isExplosive = true;
                            if (proj.damageDef == DamageDefOf.Bomb || proj.damageDef == DamageDefOf.Flame ||
                                proj.damageDef == DamageDefOf.Extinguish || proj.damageDef == DamageDefOf.EMP ||
                                proj.damageDef == DamageDefOf.Smoke || proj.damageDef == DamageDefOf.Stun)
                            {
                                isExplosive = true;
                            }
                        }
                    }
                }

                if (isExplosive)
                {
                    settings.blacklistedWeapons.Add(w.defName);
                }
            }
        }
    }
}
