using System;
using Verse;

namespace SadSeal.Common
{
    [StaticConstructorOnStartup]
    public static class ModBridge
    {
        private static bool isCombatExtendedActive;
        private static bool isSimpleSidearmsActive;
        private static bool isVanillaExpandedActive;
        private static bool initialized = false;

        public static bool IsCombatExtendedActive
        {
            get
            {
                EnsureInitialized();
                return isCombatExtendedActive;
            }
        }

        public static bool IsSimpleSidearmsActive
        {
            get
            {
                EnsureInitialized();
                return isSimpleSidearmsActive;
            }
        }

        public static bool IsVanillaExpandedActive
        {
            get
            {
                EnsureInitialized();
                return isVanillaExpandedActive;
            }
        }

        static ModBridge()
        {
            EnsureInitialized();
        }

        public static void EnsureInitialized()
        {
            if (initialized) return;
            initialized = true;

            var runningMods = LoadedModManager.RunningModsListForReading;
            if (runningMods == null) return;

            foreach (var mod in runningMods)
            {
                if (mod == null) continue;
                string pkg = (mod.PackageIdPlayerFacing ?? mod.PackageId ?? string.Empty).ToLowerInvariant();

                if (pkg.Contains("combatextended"))
                {
                    isCombatExtendedActive = true;
                }
                if (pkg.Contains("simplesidearms"))
                {
                    isSimpleSidearmsActive = true;
                }
                if (pkg.Contains("vanillaexpanded"))
                {
                    isVanillaExpandedActive = true;
                }
            }
        }
    }
}
