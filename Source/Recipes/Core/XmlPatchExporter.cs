using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using Verse;
using RimWorld;

namespace SadSeal.MegaMod.Recipes
{
    public static class XmlPatchExporter
    {
        private static string Dir => Path.Combine(GenFilePaths.ConfigFolderPath, "SadSealMegaMod");
        private static string FilePath => Path.Combine(Dir, "GeneratedPatches.xml");

        public static void ExportAll()
        {
            var tweaks = RecipesModule.ModSettings?.tweaks;
            if (tweaks == null || tweaks.Count == 0)
            {
                Messages.Message("MegaMod_ExportXmlEmpty".Translate(), MessageTypeDefOf.RejectInput, false);
                return;
            }

            Directory.CreateDirectory(Dir);
            var sb = new StringBuilder(1024);
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<Patch>");

            for (int i = 0; i < tweaks.Count; i++)
            {
                var tw = tweaks[i];
                var r = DefDatabase<RecipeDef>.GetNamed(tw.recipeDefName, false);
                if (r == null) continue;

                string escapedDefName = EscapeXml(tw.recipeDefName);
                string xpath = "Defs/RecipeDef[defName=\"" + escapedDefName + "\"]";
                bool wroteAny = false;

                if (!string.IsNullOrEmpty(tw.newLabel))
                {
                    Repl(sb, xpath + "/label", "<label>" + EscapeXml(tw.newLabel) + "</label>");
                    wroteAny = true;
                }

                if (tw.newWorkAmount >= 0f)
                {
                    Repl(sb, xpath + "/workAmount",
                        "<workAmount>" + tw.newWorkAmount.ToString("F0", CultureInfo.InvariantCulture) + "</workAmount>");
                    wroteAny = true;
                }

                if (tw.hasCustomIngredients && tweakCustom(tw))
                {
                    var ingXml = new StringBuilder();
                    ingXml.AppendLine("<ingredients>");
                    for (int j = 0; j < tw.customIngredients.Count; j++)
                    {
                        var item = tw.customIngredients[j];
                        ingXml.AppendLine("      <li>");
                        ingXml.AppendLine("        <filter><thingDefs>");

                        if (item.isFilter && item.filterDefNames != null && item.filterDefNames.Count > 0)
                        {
                            for (int d = 0; d < item.filterDefNames.Count; d++)
                            {
                                ingXml.AppendLine("          <li>" + EscapeXml(item.filterDefNames[d]) + "</li>");
                            }
                        }
                        else if (!string.IsNullOrEmpty(item.thingDefName))
                        {
                            ingXml.AppendLine("          <li>" + EscapeXml(item.thingDefName) + "</li>");
                        }

                        ingXml.AppendLine("        </thingDefs></filter>");
                        ingXml.AppendLine("        <count>" + item.count + "</count>");
                        ingXml.AppendLine("      </li>");
                    }
                    ingXml.Append("    </ingredients>");
                    Repl(sb, xpath + "/ingredients", ingXml.ToString());
                    wroteAny = true;
                }

                if (tw.disabled)
                {
                    // Remove the recipe from workbenches both via recipeUsers and
                    // via any ThingDef/recipes entries that already reference it.
                    sb.AppendLine(Remove(xpath + "/recipeUsers"));
                    sb.AppendLine(Remove("Defs/ThingDef/recipes/li[text()=\"" + escapedDefName + "\"]"));
                    wroteAny = true;
                }

                if (!wroteAny) continue;
            }

            sb.AppendLine("</Patch>");
            File.WriteAllText(FilePath, sb.ToString(), Encoding.UTF8);
            Messages.Message("MegaMod_ExportXmlSuccess".Translate(FilePath), MessageTypeDefOf.PositiveEvent, false);
        }

        private static bool tweakCustom(RecipeTweak tw)
        {
            return tw.customIngredients != null && tw.customIngredients.Count > 0;
        }

        private static string EscapeXml(string value)
        {
            return SecurityElement.Escape(value ?? string.Empty);
        }

        private static void Repl(StringBuilder sb, string xpath, string val)
        {
            sb.AppendLine("  <Operation Class=\"PatchOperationReplace\">\n    <xpath>" + xpath + "</xpath>\n    <value>" + val + "</value>\n  </Operation>");
        }

        private static string Remove(string xpath)
        {
            // success="Always" keeps the generated patch quiet when the target
            // node does not exist (e.g. a recipe without recipeUsers).
            return "  <Operation Class=\"PatchOperationRemove\" success=\"Always\">\n    <xpath>" + xpath + "</xpath>\n  </Operation>";
        }
    }
}
