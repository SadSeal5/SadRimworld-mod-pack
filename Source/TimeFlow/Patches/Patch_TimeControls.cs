using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.TimeFlow.Patches
{
    [HarmonyPatch(typeof(TimeControls), nameof(TimeControls.DoTimeControlsGUI))]
    public static class Patch_TimeControls
    {
        public static void Prefix(ref bool __state)
        {
            var settings = MegaMod.Settings?.TimeFlow;
            if (settings != null && settings.UnlockSuperSpeed && !Prefs.DevMode)
            {
                Prefs.DevMode = true;
                __state = true;
            }
            else
            {
                __state = false;
            }
        }

        public static void Postfix(bool __state)
        {
            if (__state)
            {
                Prefs.DevMode = false;
            }

            var settings = MegaMod.Settings?.TimeFlow;
            if (settings != null && settings.UnlockSuperSpeed)
            {
                if (KeyBindingDefOf.TimeSpeed_Ultrafast.KeyDownEvent)
                {
                    Find.TickManager.CurTimeSpeed = TimeSpeed.Ultrafast;
                    Event.current.Use();
                }
            }
        }
    }
}
