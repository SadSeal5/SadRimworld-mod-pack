using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Recipes.UI
{
    public static class RecipeSettingsDrawer
    {
        public static void DrawSettings(Rect inRect, RecipeSettings settings)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);

            Text.Font = GameFont.Medium;
            listing.Label("MegaMod_RecipeTitle".Translate());
            Text.Font = GameFont.Small;
            listing.Gap(6f);

            listing.Label("MegaMod_RecipeDesc".Translate());
            listing.Gap(12f);

            int count = settings?.tweaks?.Count ?? 0;
            string countText = "MegaMod_RecipeActiveCount".Translate(count);
            listing.Label(countText);
            listing.Gap(14f);

            Rect btnRow1 = listing.GetRect(38f);
            if (Widgets.ButtonText(btnRow1, "MegaMod_OpenRecipeEditor".Translate(count)))
            {
                Find.WindowStack.Add(new Dialog_RecipeTweaker());
            }

            listing.Gap(10f);

            Rect btnRow2 = listing.GetRect(34f);
            float halfW = (btnRow2.width - 10f) / 2f;
            Rect exportBtn = new Rect(btnRow2.x, btnRow2.y, halfW, 34f);
            Rect resetBtn = new Rect(btnRow2.x + halfW + 10f, btnRow2.y, halfW, 34f);

            if (Widgets.ButtonText(exportBtn, "MegaMod_ExportXmlPatch".Translate()))
            {
                XmlPatchExporter.ExportAll();
            }

            if (Widgets.ButtonText(resetBtn, "MegaMod_ResetAllRecipes".Translate()))
            {
                Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
                    "MegaMod_ResetAllConfirm".Translate(),
                    () =>
                    {
                        if (settings?.tweaks != null)
                        {
                            for (int i = 0; i < settings.tweaks.Count; i++)
                            {
                                var r = DefDatabase<RecipeDef>.GetNamed(settings.tweaks[i].recipeDefName, false);
                                if (r != null) RecipePatchApplier.ResetToVanilla(r);
                            }
                            settings.tweaks.Clear();
                            settings.Write();
                            Messages.Message("MegaMod_ResetAllSuccess".Translate(), MessageTypeDefOf.PositiveEvent, false);
                        }
                    },
                    destructive: true
                ));
            }

            listing.Gap(20f);
            GUI.color = Color.gray;
            listing.Label("MegaMod_RecipeHelp".Translate());
            GUI.color = Color.white;

            listing.End();
        }
    }
}
