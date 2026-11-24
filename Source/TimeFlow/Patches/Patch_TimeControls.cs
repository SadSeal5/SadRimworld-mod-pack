using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SadSeal.MegaMod.TimeFlow.Patches
{
    [HarmonyPatch(typeof(TimeControls), nameof(TimeControls.DoTimeControlsGUI))]
    public static class Patch_TimeControls
    {
        public static void Postfix()
        {
            if (TimeFlowModule.Instance?.IsEnabled != true || Find.TickManager == null) return;

            var settings = TimeFlowModule.ModSettings;
            if (settings != null
                && settings.UnlockSuperSpeed
                && Event.current != null
                && KeyBindingDefOf.TimeSpeed_Ultrafast.KeyDownEvent
                && (Find.WindowStack == null || (!Find.WindowStack.WindowsForcePause && !Find.WindowStack.WindowsPreventCameraMotion)))
            {
                Find.TickManager.CurTimeSpeed = TimeSpeed.Ultrafast;
                SoundDefOf.Clock_Superfast.PlayOneShotOnCamera();
                Event.current.Use();
            }
        }
    }
}
