using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.Incubation
{
    public static class GrowthVatUtility
    {
        private static readonly MethodInfo FinishPawnMethod = AccessTools.Method(typeof(Building_GrowthVat), "FinishPawn");
        private static readonly MethodInfo FinishMethod = AccessTools.Method(typeof(Building_GrowthVat), "Finish");

        public static void EjectContents(this Building_GrowthVat vat)
        {
            if (vat == null) return;

            try
            {
                if (FinishPawnMethod != null)
                {
                    FinishPawnMethod.Invoke(vat, null);
                }
                else if (FinishMethod != null)
                {
                    FinishMethod.Invoke(vat, null);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[SadSeal.Incubation] Failed to eject contents from growth vat: {ex}");
            }
        }
    }
}
