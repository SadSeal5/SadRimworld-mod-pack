using HarmonyLib;
using Verse;
using Verse.Profile;

namespace SadSeal.Core.Events
{
    [HarmonyPatch(typeof(MemoryUtility), nameof(MemoryUtility.ClearAllMapsAndWorld))]
    public static class GameLifecyclePatches
    {
        public static void Postfix()
        {
            ModEventBus.Clear();
            GenTypes.GetTypeInAnyAssembly("SadSeal.MegaMod.TimeFlow.SmartSleep.SmartSleepManager")
                ?.GetMethod("ResetState", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                ?.Invoke(null, null);
        }
    }
}
