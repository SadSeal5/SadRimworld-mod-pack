using HarmonyLib;
using Verse.Profile;

namespace SadSeal.Core.Events
{
    [HarmonyPatch(typeof(MemoryUtility), nameof(MemoryUtility.ClearAllMapsAndWorld))]
    public static class GameLifecyclePatches
    {
        public static void Postfix()
        {
            ModEventBus.Clear();
        }
    }
}
