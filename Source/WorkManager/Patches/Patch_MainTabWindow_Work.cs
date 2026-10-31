using HarmonyLib;
using RimWorld;
using SadSeal.MegaMod.WorkManager.UI;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.WorkManager.Patches
{
    [HarmonyPatch(typeof(MainTabWindow_Work), "DoWindowContents")]
    public static class Patch_MainTabWindow_Work
    {
        public static void Postfix(Rect rect)
        {
            var settings = WorkManagerModule.ModSettings;
            if (settings == null) return;

            float btnSize = 24f;
            float spacing = 6f;
            float right = rect.xMax - 12f;
            float top = rect.yMin + 4f;

            // Settings Button (Rightmost)
            Rect settingsRect = new Rect(right - btnSize, top, btnSize, btnSize);
            if (Widgets.ButtonImage(settingsRect, WorkManagerTextures.Settings))
            {
                SadSeal.Core.SadSealCoreMod.SelectModule("WorkManager");
                Find.WindowStack.Add(new Dialog_ModSettings(SadSeal.Core.SadSealCoreMod.Instance));
            }
            TooltipHandler.TipRegion(settingsRect, "WorkManager_SettingsTip".Translate());

            // Refresh Button
            Rect refreshRect = new Rect(settingsRect.x - spacing - btnSize, top, btnSize, btnSize);
            if (Widgets.ButtonImage(refreshRect, WorkManagerTextures.Refresh))
            {
                WorkManagerGameComponent.ForceUpdateAll();
                Messages.Message("WorkManager_RefreshedMsg".Translate(), MessageTypeDefOf.TaskCompletion, false);
            }
            TooltipHandler.TipRegion(refreshRect, "WorkManager_UpdateNowTip".Translate());

            // Global Toggle Button
            Rect toggleRect = new Rect(refreshRect.x - spacing - btnSize, top, btnSize, btnSize);
            Texture2D toggleTex = settings.EnableWorkManager ? WorkManagerTextures.GlobalOn : WorkManagerTextures.GlobalOff;
            if (Widgets.ButtonImage(toggleRect, toggleTex))
            {
                settings.EnableWorkManager = !settings.EnableWorkManager;
                WorkManagerGameComponent.ForceUpdateAll();
            }
            string toggleTip = settings.EnableWorkManager ? "WorkManager_GlobalEnabledTip".Translate() : "WorkManager_GlobalDisabledTip".Translate();
            TooltipHandler.TipRegion(toggleRect, toggleTip);
        }
    }
}
