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
        private static int lastCheckTick = -1;
        private static bool manualOverrideActive = false;

        public static void ResetState()
        {
            isSleepingFastForwarding = false;
            savedSpeed = TimeSpeed.Normal;
            lastSetSpeed = TimeSpeed.Normal;
            lastCheckTick = -1;
            manualOverrideActive = false;
        }

        public static void Postfix()
        {
            var settings = TimeFlowModule.ModSettings;
            if (TimeFlowModule.Instance?.IsEnabled != true || settings == null || !settings.EnableSmartSleepFastForward)
            {
                if (isSleepingFastForwarding)
                {
                    if (Current.Game?.PlayerHasControl == true
                        && Find.TickManager != null
                        && (Find.World == null || Find.GravshipController?.LandingAreaConfirmationInProgress != true))
                    {
                        Find.TickManager.CurTimeSpeed = savedSpeed;
                        lastSetSpeed = savedSpeed;
                    }
                    isSleepingFastForwarding = false;
                }
                manualOverrideActive = false;
                return;
            }

            if (Current.Game == null
                || Find.TickManager == null
                || Find.TickManager.Paused
                || !Current.Game.PlayerHasControl
                || (Find.World != null && Find.GravshipController?.LandingAreaConfirmationInProgress == true))
            {
                return;
            }

            // If player manually changed speed during fast-forwarding, respect the player's choice!
            if (isSleepingFastForwarding && Find.TickManager.CurTimeSpeed != lastSetSpeed)
            {
                isSleepingFastForwarding = false;
                manualOverrideActive = true;
                return;
            }

            int ticksGame = Find.TickManager.TicksGame;
            if (lastCheckTick >= 0 && ticksGame >= lastCheckTick && ticksGame - lastCheckTick < checkInterval)
            {
                return;
            }
            lastCheckTick = ticksGame;

            List<Map> maps = Find.Maps;
            if (maps == null || maps.Count == 0) return;

            bool anyColonist = false;
            bool allSleeping = true;
            bool criticalDangerOrBleed = false;

            for (int m = 0; m < maps.Count; m++)
            {
                Map map = maps[m];
                if (map == null) continue;

                bool hasFireDanger = map.fireWatcher != null && map.fireWatcher.LargeFireDangerPresent;
                bool hasGeneralDanger = map.dangerWatcher != null && map.dangerWatcher.DangerRating != StoryDanger.None;
                if (hasFireDanger || hasGeneralDanger)
                {
                    criticalDangerOrBleed = true;
                    break;
                }

                List<Pawn> colonists = map.mapPawns?.FreeColonists;
                if (colonists == null) continue;

                for (int i = 0; i < colonists.Count; i++)
                {
                    Pawn c = colonists[i];
                    if (c == null || c.Dead) continue;

                    anyColonist = true;

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

                if (criticalDangerOrBleed || !allSleeping)
                {
                    break;
                }
            }

            if (anyColonist && allSleeping && !criticalDangerOrBleed)
            {
                if (!isSleepingFastForwarding && !manualOverrideActive)
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
                manualOverrideActive = false;
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
