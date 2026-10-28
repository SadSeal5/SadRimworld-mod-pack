using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.QoL.Patches.Work
{
    [HarmonyPatch(typeof(WorkGiver_Repair), nameof(WorkGiver_Repair.HasJobOnThing))]
    public static class Patch_RepairInZone
    {
        public static void Postfix(Verse.Pawn pawn, Thing t, bool forced, ref bool __result)
        {
            if (!__result || forced) return;
            if (MegaMod.Settings?.QoL?.repairInZone != true) return;
            if (pawn?.playerSettings?.AreaRestrictionInPawnCurrentMap == null || t == null) return;

            Area allowed = pawn.playerSettings.AreaRestrictionInPawnCurrentMap;
            if (!allowed[t.Position])
            {
                __result = false;
            }
        }
    }
}
