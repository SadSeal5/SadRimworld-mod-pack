using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Genes.Jobs
{
    [DefOf]
    public static class GeneJobDefOf
    {
        public static JobDef HaulNeutroamineToGeneAssembler;
        public static JobDef SynthesizeGene;

        static GeneJobDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(GeneJobDefOf));
        }
    }
}
