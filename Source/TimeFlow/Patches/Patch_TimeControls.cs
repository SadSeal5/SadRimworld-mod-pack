using System;
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
            var settings = TimeFlowModule.ModSettings;
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

        public static void Postfix()
        {
            var settings = TimeFlowModule.ModSettings;
            if (settings != null && settings.UnlockSuperSpeed)
            {
                // Only trigger hotkey when not typing in text fields or modal dialogs
                if (KeyBindingDefOf.TimeSpeed_Ultrafast.KeyDownEvent &&
                    (Find.WindowStack == null || !Find.WindowStack.WindowsPreventCameraMotion))
                {
                    Find.TickManager.CurTimeSpeed = TimeSpeed.Ultrafast;
                    Event.current.Use();
                }
            }
        }

        public static Exception Finalizer(Exception __exception, bool __state)
        {
            if (__state)
            {
                Prefs.DevMode = false;
            }
            return __exception;
        }
    }
}
