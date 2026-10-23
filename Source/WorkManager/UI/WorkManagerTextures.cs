using System.IO;
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

        public static Texture2D GlobalOn => GetValidTexture(ref globalOn, "UI/WorkManager/work-manager-global-on", "work-manager-global-on", "work-manager-priorities-on");
        public static Texture2D GlobalOff => GetValidTexture(ref globalOff, "UI/WorkManager/work-manager-global-off", "work-manager-global-off", "work-manager-priorities-off");
        public static Texture2D PawnOn => GetValidTexture(ref pawnOn, "UI/WorkManager/work-manager-pawn-on", "work-manager-pawn-on");
        public static Texture2D PawnOff => GetValidTexture(ref pawnOff, "UI/WorkManager/work-manager-pawn-off", "work-manager-pawn-off");
        public static Texture2D Refresh => GetValidTexture(ref refresh, "UI/WorkManager/work-manager-refresh", "work-manager-refresh");
        public static Texture2D Settings => GetValidTexture(ref settings, "UI/WorkManager/work-manager-settings", "work-manager-settings");
        public static Texture2D ScheduleOn => GetValidTexture(ref scheduleOn, "UI/WorkManager/work-manager-schedule-on", "work-manager-schedule-on");
        public static Texture2D ScheduleOff => GetValidTexture(ref scheduleOff, "UI/WorkManager/work-manager-schedule-off", "work-manager-schedule-off");

        private static Texture2D GetValidTexture(ref Texture2D cached, params string[] paths)
        {
            if (cached != null && cached != BaseContent.BadTex)
            {
                return cached;
            }

            foreach (var path in paths)
            {
                var tex = ContentFinder<Texture2D>.Get(path, false);
                if (tex != null && tex != BaseContent.BadTex)
                {
                    cached = tex;
                    return cached;
                }
            }

            // Direct filesystem fallback if ContentFinder failed
            try
            {
                foreach (var mod in LoadedModManager.RunningMods)
                {
                    string pkg = mod.PackageIdPlayerFacing?.ToLowerInvariant() ?? "";
                    string name = mod.Name?.ToLowerInvariant() ?? "";
                    if (pkg.Contains("sadseal") || pkg.Contains("workmanager") || name.Contains("sadseal") || name.Contains("work manager") || name.Contains("mega mod"))
                    {
                        foreach (var path in paths)
                        {
                            string fileName = Path.GetFileName(path) + ".png";
                            string[] files = Directory.GetFiles(mod.RootDir, fileName, SearchOption.AllDirectories);
                            if (files.Length > 0)
                            {
                                byte[] bytes = File.ReadAllBytes(files[0]);
                                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                                if (tex.LoadImage(bytes))
                                {
                                    cached = tex;
                                    return cached;
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            cached = BaseContent.BadTex;
            return cached;
        }
    }
}
