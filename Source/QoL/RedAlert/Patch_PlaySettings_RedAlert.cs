using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.QoL.RedAlert
{
    [HarmonyPatch(typeof(PlaySettings), nameof(PlaySettings.DoPlaySettingsGlobalControls))]
    public static class Patch_PlaySettings_RedAlert
    {
        [HarmonyPostfix]
        public static void Postfix(WidgetRow row, bool worldView)
        {
            if (worldView || Find.CurrentMap == null || !Find.CurrentMap.IsPlayerHome)
            {
                return;
            }

            if (MegaMod.Settings?.Modules?.enableQoL == false)
            {
                return;
            }

            RedAlertManager alertMgr = Find.CurrentMap.GetComponent<RedAlertManager>();
            if (alertMgr == null)
            {
                return;
            }

            Texture2D icon = alertMgr.IsRedAlertActive ? RedAlertTextures.IconActive : RedAlertTextures.IconInactive;
            string tooltip = alertMgr.IsRedAlertActive
                ? "MegaMod_RedAlert_TooltipActive".Translate()
                : "MegaMod_RedAlert_TooltipInactive".Translate();

            if (row.ButtonIcon(icon, tooltip))
            {
                alertMgr.ToggleRedAlert();
            }

            KeyBindingDef keyDef = SadKeyBindingDefOf.Sad_KeyBinding_RedAlert ?? KeyBindingDef.Named("Sad_KeyBinding_RedAlert");
            if (keyDef != null && keyDef.KeyDownEvent)
            {
                if (Find.WindowStack == null || !Find.WindowStack.WindowsPreventCameraMotion)
                {
                    alertMgr.ToggleRedAlert();
                    Event.current.Use();
                }
            }
        }
    }
}
