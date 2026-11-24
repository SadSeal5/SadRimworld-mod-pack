using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Genes.Patches
{
    [HarmonyPatch(typeof(Building_GeneAssembler), nameof(Building_GeneAssembler.PostPostMake))]
    public static class GeneAssemblerPatch
    {
        [HarmonyPrepare]
        public static bool Prepare() => ModsConfig.BiotechActive;

        [HarmonyPostfix]
        public static void Postfix(Building_GeneAssembler __instance)
        {
            EnsureCompOnInstance(__instance);
        }

        public static void EnsureCompOnInstance(Building_GeneAssembler instance)
        {
            if (instance == null || GenesModule.Instance?.IsEnabled != true || !ModsConfig.BiotechActive) return;
            EnsureDefHasComp(instance.def);

            if (instance.GetComp<CompGeneSynthesizer>() != null) return;

            if (instance.AllComps.Count == 0 && instance.def != null)
            {
                instance.InitializeComps();
            }

            if (instance.GetComp<CompGeneSynthesizer>() == null && instance.AllComps.Count > 0)
            {
                CompGeneSynthesizer comp = new CompGeneSynthesizer
                {
                    parent = instance
                };
                comp.Initialize(new CompProperties_GeneSynthesizer());
                instance.AllComps.Add(comp);
            }
        }

        public static void EnsureDefHasComp(ThingDef def)
        {
            if (def == null) return;
            if (def.comps == null)
            {
                def.comps = new System.Collections.Generic.List<CompProperties>();
            }

            for (int i = 0; i < def.comps.Count; i++)
            {
                CompProperties c = def.comps[i];
                if (c is CompProperties_GeneSynthesizer || c?.compClass == typeof(CompGeneSynthesizer))
                {
                    return;
                }
            }

            def.comps.Add(new CompProperties_GeneSynthesizer());
        }

        public static void RemoveDefComp(ThingDef def)
        {
            if (def?.comps == null) return;
            def.comps.RemoveAll(c => c is CompProperties_GeneSynthesizer || c?.compClass == typeof(CompGeneSynthesizer));
        }
    }

    [HarmonyPatch(typeof(Building_GeneAssembler), nameof(Building_GeneAssembler.ExposeData))]
    public static class GeneAssemblerExposePatch
    {
        [HarmonyPrepare]
        public static bool Prepare() => ModsConfig.BiotechActive;

        [HarmonyPrefix]
        public static void Prefix(Building_GeneAssembler __instance)
        {
            if (GenesModule.Instance?.IsEnabled != true || !ModsConfig.BiotechActive) return;
            if (__instance?.def != null && Scribe.mode == LoadSaveMode.LoadingVars)
            {
                GeneAssemblerPatch.EnsureDefHasComp(__instance.def);
            }
        }
    }

    [HarmonyPatch(typeof(Building_GeneAssembler), nameof(Building_GeneAssembler.GetGizmos))]
    public static class GeneAssemblerGetGizmosPatch
    {
        [HarmonyPrepare]
        public static bool Prepare() => ModsConfig.BiotechActive;

        [HarmonyPostfix]
        public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Building_GeneAssembler __instance)
        {
            bool hasSynthesizerGizmo = false;

            if (__result != null)
            {
                foreach (Gizmo g in __result)
                {
                    if (g is Command_Action cmd && cmd.defaultLabel == "GeneSynthesis_GizmoSynthesize".Translate())
                    {
                        hasSynthesizerGizmo = true;
                    }
                    yield return g;
                }
            }

            if (GenesModule.Instance?.IsEnabled != true || !ModsConfig.BiotechActive) yield break;
            if (__instance == null || !__instance.Spawned) yield break;

            GeneAssemblerPatch.EnsureCompOnInstance(__instance);

            CompGeneSynthesizer comp = __instance.GetComp<CompGeneSynthesizer>();
            if (comp != null && !hasSynthesizerGizmo)
            {
                foreach (Gizmo extra in comp.CompGetGizmosExtra())
                {
                    yield return extra;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Building_GeneAssembler), nameof(Building_GeneAssembler.GetInspectString))]
    public static class GeneAssemblerInspectStringPatch
    {
        [HarmonyPrepare]
        public static bool Prepare() => ModsConfig.BiotechActive;

        [HarmonyPostfix]
        public static string Postfix(string __result, Building_GeneAssembler __instance)
        {
            if (__instance == null || !__instance.Spawned) return __result;

            CompGeneSynthesizer comp = __instance.GetComp<CompGeneSynthesizer>();
            if (comp == null || !comp.IsSynthesizing || comp.TargetGene == null) return __result;

            string extra = comp.CompInspectStringExtra();
            if (string.IsNullOrEmpty(extra)) return __result;

            return string.IsNullOrEmpty(__result) ? extra : __result + "\n" + extra;
        }
    }
}
