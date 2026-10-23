using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.WorkManager.UI
{
    [StaticConstructorOnStartup]
    public static class WorkManagerTextures
    {
        private static Texture2D globalOn;
        private static Texture2D globalOff;
        private static Texture2D pawnOn;
        private static Texture2D pawnOff;
        private static Texture2D refresh;
        private static Texture2D settings;
        private static Texture2D scheduleOn;
        private static Texture2D scheduleOff;

        public static Texture2D GlobalOn => globalOn ??= LoadTexture("UI/WorkManager/work-manager-global-on", "work-manager-global-on");
        public static Texture2D GlobalOff => globalOff ??= LoadTexture("UI/WorkManager/work-manager-global-off", "work-manager-global-off");
        public static Texture2D PawnOn => pawnOn ??= LoadTexture("UI/WorkManager/work-manager-pawn-on", "work-manager-pawn-on");
        public static Texture2D PawnOff => pawnOff ??= LoadTexture("UI/WorkManager/work-manager-pawn-off", "work-manager-pawn-off");
        public static Texture2D Refresh => refresh ??= LoadTexture("UI/WorkManager/work-manager-refresh", "work-manager-refresh");
        public static Texture2D Settings => settings ??= LoadTexture("UI/WorkManager/work-manager-settings", "work-manager-settings");
        public static Texture2D ScheduleOn => scheduleOn ??= LoadTexture("UI/WorkManager/work-manager-schedule-on", "work-manager-schedule-on");
        public static Texture2D ScheduleOff => scheduleOff ??= LoadTexture("UI/WorkManager/work-manager-schedule-off", "work-manager-schedule-off");

        private static Texture2D LoadTexture(string primaryPath, string fallbackPath)
        {
            var tex = ContentFinder<Texture2D>.Get(primaryPath, false);
            if (tex != null) return tex;
            tex = ContentFinder<Texture2D>.Get(fallbackPath, false);
            if (tex != null) return tex;
            return BaseContent.BadTex;
        }
    }
}
