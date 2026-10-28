using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using SadSeal.MegaMod.QoL.Alerts;

namespace SadSeal.MegaMod.QoL.Patches.Raids
{
    [HarmonyPatch(typeof(AlertsReadout), MethodType.Constructor)]
    public static class Patch_AlertsReadout
    {
        public static void Postfix(AlertsReadout __instance)
        {
            var allAlerts = Traverse.Create(__instance).Field<List<Alert>>("AllAlerts").Value;
            if (allAlerts != null)
            {
                allAlerts.Add(new Alert_TraderVisiting());
            }
        }
    }
}
