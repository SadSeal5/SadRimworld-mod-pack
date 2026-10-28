using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.AllowTool.SelectSimilar
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.GetGizmos))]
    public static class Patch_Thing_GetGizmos
    {
        public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Thing __instance)
        {
            if (__result != null)
            {
                foreach (Gizmo g in __result)
                {
                    yield return g;
                }
            }

            var settings = MegaMod.Settings?.AllowTool;
            if (settings == null || !settings.EnableSelectSimilar) yield break;

            if (__instance == null || !__instance.Spawned || (__instance.Faction != null && __instance.Faction.HostileTo(Faction.OfPlayer)))
            {
                yield break;
            }

            // Provide Select Similar gizmo for items (including stone chunks), structures, ores, and plants
            if (__instance.def.category == ThingCategory.Item ||
                __instance.def.category == ThingCategory.Building ||
                __instance.def.category == ThingCategory.Plant)
            {
                yield return new Command_Action
                {
                    defaultLabel = "MegaMod_SelectSimilar_Gizmo".Translate(),
                    defaultDesc = "MegaMod_SelectSimilar_Gizmo_Desc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/SelectNextInCell", false) ?? BaseContent.WhiteTex,
                    action = () => Designator_SelectSimilar.SelectAllSimilar(__instance, false)
                };
            }
        }
    }
}
