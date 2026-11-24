using System.Collections.Generic;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Genes
{
    public class GeneSynthesisOrder : IExposable
    {
        public List<GeneDef> Genes = new List<GeneDef>();
        public string Label;
        public int NeutroamineCost;
        public int ArchiteCost;
        public float WorkRequired;
        public float WorkDone;
        public bool IsCombinedPack;
        public XenotypeDef SourceXenotype;

        public GeneSynthesisOrder()
        {
        }

        public GeneSynthesisOrder(GeneDef gene, int neutroCost, float work)
        {
            if (gene != null)
            {
                Genes.Add(gene);
            }
            Label = gene?.LabelCap ?? "GeneSynthesis_UnknownGene".Translate().ToString();
            NeutroamineCost = neutroCost;
            ArchiteCost = CalculateArchiteCost(Genes);
            WorkRequired = work;
            WorkDone = 0f;
            IsCombinedPack = false;
            SourceXenotype = null;
        }

        public GeneSynthesisOrder(List<GeneDef> genes, string label, int neutroCost, float work, bool isCombinedPack = false, XenotypeDef sourceXenotype = null)
        {
            if (genes != null)
            {
                Genes.AddRange(genes);
            }
            Label = label;
            NeutroamineCost = neutroCost;
            ArchiteCost = CalculateArchiteCost(Genes);
            WorkRequired = work;
            WorkDone = 0f;
            IsCombinedPack = isCombinedPack;
            SourceXenotype = sourceXenotype;
        }

        public static int CalculateArchiteCost(IEnumerable<GeneDef> genes)
        {
            if (genes == null || GenesModule.ModSettings?.RequireArchiteCapsules != true)
            {
                return 0;
            }

            int cost = 0;
            foreach (GeneDef g in genes)
            {
                if (g != null && g.biostatArc > 0)
                {
                    cost += g.biostatArc;
                }
            }
            return cost;
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref Genes, "genes", LookMode.Def);
            Scribe_Values.Look(ref Label, "label");
            Scribe_Values.Look(ref NeutroamineCost, "neutroamineCost", 0);
            Scribe_Values.Look(ref ArchiteCost, "architeCost", 0);
            Scribe_Values.Look(ref WorkRequired, "workRequired", 0f);
            Scribe_Values.Look(ref WorkDone, "workDone", 0f);
            Scribe_Values.Look(ref IsCombinedPack, "isCombinedPack", false);
            Scribe_Defs.Look(ref SourceXenotype, "sourceXenotype");

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Genes ??= new List<GeneDef>();
                Genes.RemoveAll(g => g == null);
            }
        }
    }
}
