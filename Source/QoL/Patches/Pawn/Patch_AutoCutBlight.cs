using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.QoL.Patches.Pawn
{
    [HarmonyPatch(typeof(Plant), nameof(Plant.CropBlighted))]
    public static class Patch_AutoCutBlight
    {
        public static void Postfix(Plant __instance)
        {
            if (QoLModule.ModSettings?.enableAutoCutBlight != true) return;
            if (__instance == null || !__instance.Spawned) return;

            Map map = __instance.Map;
            if (map == null) return;
            if (__instance.Position.Fogged(map)) return;

            // If plant has an existing harvest designation, delete it first to prevent conflict
            Designation harvestDes = map.designationManager.DesignationOn(__instance, DesignationDefOf.HarvestPlant);
            if (harvestDes != null)
            {
                map.designationManager.RemoveDesignation(harvestDes);
            }

            // Immediately designate the blighted plant for cutting
            if (map.designationManager.DesignationOn(__instance, DesignationDefOf.CutPlant) == null)
            {
                map.designationManager.AddDesignation(new Designation(__instance, DesignationDefOf.CutPlant));
            }
        }
    }
}
