using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.WorkManager.UI
{
    public static class WorkManagerSettingsDrawer
    {
        private static Vector2 scrollPosition = Vector2.zero;

        public static void DrawSettings(Rect inRect, WorkManagerSettings settings)
        {
            if (settings == null) return;

            Rect viewRect = new Rect(0f, 0f, inRect.width - 24f, 820f);
            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            Text.Font = GameFont.Medium;
            listing.Label("WorkManager_ModTitle".Translate());
            Text.Font = GameFont.Small;
            listing.Label("WorkManager_ModDesc".Translate());
            listing.Gap(10f);

            // Preset Buttons Section
            listing.Label("WorkManager_PresetsHeader".Translate());
            Rect presetRect = listing.GetRect(32f);
            float btnWidth = (presetRect.width - 15f) / 4f;

            Color oldColor = GUI.color;

            if (settings.ActivePreset == WorkManagerPreset.Balanced) GUI.color = Color.green;
            if (Widgets.ButtonText(new Rect(presetRect.x, presetRect.y, btnWidth, 32f), "WorkManager_PresetBalanced".Translate()))
            {
                settings.ApplyPreset(WorkManagerPreset.Balanced);
            }
            GUI.color = oldColor;

            if (settings.ActivePreset == WorkManagerPreset.Performance) GUI.color = Color.green;
            if (Widgets.ButtonText(new Rect(presetRect.x + btnWidth + 5f, presetRect.y, btnWidth, 32f), "WorkManager_PresetPerformance".Translate()))
            {
                settings.ApplyPreset(WorkManagerPreset.Performance);
            }
            GUI.color = oldColor;

            if (settings.ActivePreset == WorkManagerPreset.Micro) GUI.color = Color.green;
            if (Widgets.ButtonText(new Rect(presetRect.x + (btnWidth + 5f) * 2f, presetRect.y, btnWidth, 32f), "WorkManager_PresetMicro".Translate()))
            {
                settings.ApplyPreset(WorkManagerPreset.Micro);
            }
            GUI.color = oldColor;

            if (Widgets.ButtonText(new Rect(presetRect.x + (btnWidth + 5f) * 3f, presetRect.y, btnWidth, 32f), "WorkManager_ResetDefaults".Translate()))
            {
                settings.ResetToDefaults();
            }

            listing.Gap(14f);

            // General Features Toggles
            Text.Font = GameFont.Medium;
            listing.Label("WorkManager_SectionGeneral".Translate());
            Text.Font = GameFont.Small;
            listing.Gap(4f);

            bool oldEnable = settings.EnableWorkManager;
            listing.CheckboxLabeled("WorkManager_EnableWorkManager".Translate(), ref settings.EnableWorkManager, "WorkManager_EnableWorkManager_Desc".Translate());
            if (oldEnable != settings.EnableWorkManager) settings.ActivePreset = WorkManagerPreset.Custom;

            bool oldPriorities = settings.ManageWorkPriorities;
            listing.CheckboxLabeled("WorkManager_ManagePriorities".Translate(), ref settings.ManageWorkPriorities, "WorkManager_ManagePriorities_Desc".Translate());
            if (oldPriorities != settings.ManageWorkPriorities) settings.ActivePreset = WorkManagerPreset.Custom;

            bool oldSchedule = settings.ManageWorkSchedule;
            listing.CheckboxLabeled("WorkManager_ManageSchedule".Translate(), ref settings.ManageWorkSchedule, "WorkManager_ManageSchedule_Desc".Translate());
            if (oldSchedule != settings.ManageWorkSchedule) settings.ActivePreset = WorkManagerPreset.Custom;

            bool oldDedicated = settings.UseDedicatedWorkers;
            listing.CheckboxLabeled("WorkManager_UseDedicatedWorkers".Translate(), ref settings.UseDedicatedWorkers, "WorkManager_UseDedicatedWorkers_Desc".Translate());
            if (oldDedicated != settings.UseDedicatedWorkers) settings.ActivePreset = WorkManagerPreset.Custom;

            bool oldPassion = settings.UsePassionPriorities;
            listing.CheckboxLabeled("WorkManager_UsePassionPriorities".Translate(), ref settings.UsePassionPriorities, "WorkManager_UsePassionPriorities_Desc".Translate());
            if (oldPassion != settings.UsePassionPriorities) settings.ActivePreset = WorkManagerPreset.Custom;

            bool oldIdle = settings.AssignWorkToIdlePawns;
            listing.CheckboxLabeled("WorkManager_AssignWorkToIdlePawns".Translate(), ref settings.AssignWorkToIdlePawns, "WorkManager_AssignWorkToIdlePawns_Desc".Translate());
            if (oldIdle != settings.AssignWorkToIdlePawns) settings.ActivePreset = WorkManagerPreset.Custom;

            bool oldNight = settings.SupportNightOwls;
            listing.CheckboxLabeled("WorkManager_SupportNightOwls".Translate(), ref settings.SupportNightOwls, "WorkManager_SupportNightOwls_Desc".Translate());
            if (oldNight != settings.SupportNightOwls) settings.ActivePreset = WorkManagerPreset.Custom;

            listing.Gap(14f);

            // Timing Section
            Text.Font = GameFont.Medium;
            listing.Label("WorkManager_SectionTiming".Translate());
            Text.Font = GameFont.Small;
            listing.Gap(4f);

            int oldTicks = settings.PriorityUpdateIntervalTicks;
            settings.PriorityUpdateIntervalTicks = (int)listing.SliderLabeled(
                "WorkManager_PriorityInterval".Translate() + $": {settings.PriorityUpdateIntervalTicks} ticks ({settings.PriorityUpdateIntervalTicks / 60f:F1}s)",
                settings.PriorityUpdateIntervalTicks, 250f, 2500f, 0.5f,
                "WorkManager_PriorityInterval_Desc".Translate());
            if (oldTicks != settings.PriorityUpdateIntervalTicks) settings.ActivePreset = WorkManagerPreset.Custom;

            int oldHours = settings.ScheduleUpdateIntervalHours;
            settings.ScheduleUpdateIntervalHours = (int)listing.SliderLabeled(
                "WorkManager_ScheduleInterval".Translate() + $": {settings.ScheduleUpdateIntervalHours}h",
                settings.ScheduleUpdateIntervalHours, 1f, 12f, 0.5f,
                "WorkManager_ScheduleInterval_Desc".Translate());
            if (oldHours != settings.ScheduleUpdateIntervalHours) settings.ActivePreset = WorkManagerPreset.Custom;

            listing.Gap(14f);

            // Specialists Count Section
            Text.Font = GameFont.Medium;
            listing.Label("WorkManager_SectionSpecialists".Translate());
            Text.Font = GameFont.Small;
            listing.Gap(4f);

            int oldDoc = settings.TargetDoctors;
            settings.TargetDoctors = (int)listing.SliderLabeled(
                "WorkManager_TargetDoctors".Translate() + $": {settings.TargetDoctors}",
                settings.TargetDoctors, 1f, 5f, 0.5f);
            if (oldDoc != settings.TargetDoctors) settings.ActivePreset = WorkManagerPreset.Custom;

            int oldCook = settings.TargetCooks;
            settings.TargetCooks = (int)listing.SliderLabeled(
                "WorkManager_TargetCooks".Translate() + $": {settings.TargetCooks}",
                settings.TargetCooks, 1f, 5f, 0.5f);
            if (oldCook != settings.TargetCooks) settings.ActivePreset = WorkManagerPreset.Custom;

            int oldRes = settings.TargetResearchers;
            settings.TargetResearchers = (int)listing.SliderLabeled(
                "WorkManager_TargetResearchers".Translate() + $": {settings.TargetResearchers}",
                settings.TargetResearchers, 1f, 5f, 0.5f);
            if (oldRes != settings.TargetResearchers) settings.ActivePreset = WorkManagerPreset.Custom;

            int oldWarden = settings.TargetWardens;
            settings.TargetWardens = (int)listing.SliderLabeled(
                "WorkManager_TargetWardens".Translate() + $": {settings.TargetWardens}",
                settings.TargetWardens, 1f, 5f, 0.5f);
            if (oldWarden != settings.TargetWardens) settings.ActivePreset = WorkManagerPreset.Custom;

            int oldHandler = settings.TargetHandlers;
            settings.TargetHandlers = (int)listing.SliderLabeled(
                "WorkManager_TargetHandlers".Translate() + $": {settings.TargetHandlers}",
                settings.TargetHandlers, 1f, 5f, 0.5f);
            if (oldHandler != settings.TargetHandlers) settings.ActivePreset = WorkManagerPreset.Custom;

            listing.Gap(14f);

            // Manual Trigger Button
            if (listing.ButtonText("WorkManager_ForceUpdate".Translate()))
            {
                WorkManagerGameComponent.ForceUpdateAll();
                Messages.Message("WorkManager_RefreshedMsg".Translate(), MessageTypeDefOf.TaskCompletion, false);
            }

            listing.End();
            Widgets.EndScrollView();
        }
    }
}
