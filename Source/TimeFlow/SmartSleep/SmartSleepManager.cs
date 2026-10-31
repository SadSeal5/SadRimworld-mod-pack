using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.TimeFlow.SmartSleep
{
    [HarmonyPatch(typeof(TickManager), nameof(TickManager.TickManagerUpdate))]
    public static class SmartSleepManager
    {
        private static bool isSleepingFastForwarding = false;
        private static TimeSpeed savedSpeed = TimeSpeed.Normal;
        private static TimeSpeed lastSetSpeed = TimeSpeed.Normal;
        private static int checkInterval = 30;

        public static void Postfix()
        {
            var settings = TimeFlowModule.ModSettings;
            if (settings == null || !settings.EnableSmartSleepFastForward)
            {
                if (isSleepingFastForwarding)
                {
                    isSleepingFastForwarding = false;
                }
                return;
            }

            if (Find.TickManager == null || Find.TickManager.Paused) return;

            Map map = Find.CurrentMap;
            if (map == null) return;

            // If player manually changed speed during fast-forwarding, respect the player's choice!
            if (isSleepingFastForwarding && Find.TickManager.CurTimeSpeed != lastSetSpeed)
            {
                isSleepingFastForwarding = false;
                return;
            }

            if (Find.TickManager.TicksGame % checkInterval != 0) return;

            List<Pawn> colonists = map.mapPawns.FreeColonists;
            if (colonists == null || colonists.Count == 0) return;

            bool allSleeping = true;
            bool criticalDangerOrBleed = false;

            for (int i = 0; i < colonists.Count; i++)
            {
                Pawn c = colonists[i];
                if (c == null || c.Dead) continue;

                // If any colonist is bleeding out, don't fast forward through their death
                if (c.health?.hediffSet != null && c.health.hediffSet.BleedRateTotal > 0.05f)
                {
                    criticalDangerOrBleed = true;
                    break;
                }

                if (c.Downed) continue;

                // Accurate vanilla awake check
                if (RestUtility.Awake(c))
                {
                    allSleeping = false;
                    break;
                }
            }

            bool hasFireDanger = map.fireWatcher != null && map.fireWatcher.LargeFireDangerPresent;
            bool hasGeneralDanger = map.dangerWatcher != null && map.dangerWatcher.DangerRating != StoryDanger.None;
            if (hasFireDanger || hasGeneralDanger)
            {
                criticalDangerOrBleed = true;
            }

            bool hasDanger = map.dangerWatcher != null && map.dangerWatcher.DangerRating == StoryDanger.High;

            if (allSleeping && !hasDanger && !criticalDangerOrBleed)
            {
                if (!isSleepingFastForwarding)
                {
                    savedSpeed = Find.TickManager.CurTimeSpeed;
                    if (savedSpeed == TimeSpeed.Paused) savedSpeed = TimeSpeed.Normal;

                    isSleepingFastForwarding = true;
                    TimeSpeed targetSpeed = (settings.SleepFastForwardSpeed == 4) ? TimeSpeed.Ultrafast : TimeSpeed.Superfast;
                    Find.TickManager.CurTimeSpeed = targetSpeed;
                    lastSetSpeed = targetSpeed;
                }
            }
            else
            {
                if (isSleepingFastForwarding)
                {
                    isSleepingFastForwarding = false;
                    Find.TickManager.CurTimeSpeed = savedSpeed;
                    lastSetSpeed = savedSpeed;
                }
            }
        }
    }
}
