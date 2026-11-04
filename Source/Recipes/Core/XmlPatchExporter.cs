using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Recipes
{
    public static class XmlPatchExporter
    {
        private static string Dir => Path.Combine(GenFilePaths.ConfigFolderPath, "SadSealMegaMod");
        private static string FilePath => Path.Combine(Dir, "GeneratedPatches.xml");

        public static void ExportAll()
        {
            var tweaks = MegaMod.Settings?.Recipes?.tweaks;
            if (tweaks == null || tweaks.Count == 0)
            {
                Messages.Message("MegaMod_RecipeExportEmpty".Translate(), MessageTypeDefOf.RejectInput, false);
                return;
            }

            Directory.CreateDirectory(Dir);
            var sb = new StringBuilder(1024);
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<Patch>");

            for (int i = 0; i < tweaks.Count; i++)
            {
                var tw = tweaks[i];
                if (tw == null || tw.IsVanilla) continue;
                if (CustomRecipeManager.IsCustomRecipe(tw.recipeDefName)) continue;

                var r = DefDatabase<RecipeDef>.GetNamed(tw.recipeDefName, false);
                if (r == null) continue;

                string xpath = "Defs/RecipeDef[defName=\"" + Escape(tw.recipeDefName) + "\"]";

                if (!string.IsNullOrEmpty(tw.newLabel))
                {
                    ReplaceOrAdd(sb, xpath + "/label", "<label>" + Escape(tw.newLabel) + "</label>");
                }

                if (tw.newWorkAmount >= 0f)
                {
                    string work = tw.newWorkAmount.ToString("0.##", CultureInfo.InvariantCulture);
                    ReplaceOrAdd(sb, xpath + "/workAmount", "<workAmount>" + work + "</workAmount>");
                }

                if (tw.hasCustomIngredients && tw.customIngredients != null && tw.customIngredients.Count > 0)
                {
                    var ingXml = new StringBuilder();
                    ingXml.AppendLine("<ingredients>");
                    for (int j = 0; j < tw.customIngredients.Count; j++)
                    {
                        var item = tw.customIngredients[j];
                        ingXml.AppendLine("  <li>");
                        ingXml.AppendLine("    <filter><thingDefs><li>" + Escape(item.thingDefName) + "</li></thingDefs></filter>");
                        ingXml.AppendLine("    <count>" + item.count.ToString(CultureInfo.InvariantCulture) + "</count>");
                        ingXml.AppendLine("  </li>");
                    }
                    ingXml.Append("</ingredients>");
                    ReplaceOrAdd(sb, xpath + "/ingredients", ingXml.ToString());
                }

                if (tw.disabled)
                {
                    // An empty recipeUsers list removes the recipe from every workbench
                    // when the exported patch is loaded by the game.
                    ReplaceOrAdd(sb, xpath + "/recipeUsers", "<recipeUsers />");
                }
            }

            sb.AppendLine("</Patch>");
            File.WriteAllText(FilePath, sb.ToString(), new UTF8Encoding(false));
            Messages.Message("MegaMod_RecipeExportDone".Translate(FilePath), MessageTypeDefOf.PositiveEvent, false);
        }

        private static void ReplaceOrAdd(StringBuilder sb, string xpath, string value)
        {
            string parentXpath = xpath.Substring(0, xpath.LastIndexOf('/'));
            sb.AppendLine("  <Operation Class=\"PatchOperationConditional\">");
            sb.AppendLine("    <xpath>" + Escape(xpath) + "</xpath>");
            sb.AppendLine("    <match Class=\"PatchOperationReplace\">");
            sb.AppendLine("      <xpath>" + Escape(xpath) + "</xpath>");
            sb.AppendLine("      <value>" + value + "</value>");
            sb.AppendLine("    </match>");
            sb.AppendLine("    <nomatch Class=\"PatchOperationAdd\">");
            sb.AppendLine("      <xpath>" + Escape(parentXpath) + "</xpath>");
            sb.AppendLine("      <value>" + value + "</value>");
            sb.AppendLine("    </nomatch>");
            sb.AppendLine("  </Operation>");
        }

        private static string Escape(string value)
        {
            return SecurityElement.Escape(value ?? string.Empty);
        }
    }
}
