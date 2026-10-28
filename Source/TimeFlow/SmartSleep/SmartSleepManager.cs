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
        private static int checkInterval = 60;

        public static void Postfix()
        {
            var settings = MegaMod.Settings?.TimeFlow;
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

            if (Find.TickManager.TicksGame % checkInterval != 0) return;

            List<Pawn> colonists = map.mapPawns.FreeColonists;
            if (colonists == null || colonists.Count == 0) return;

            bool allSleeping = true;
            for (int i = 0; i < colonists.Count; i++)
            {
                Pawn c = colonists[i];
                if (c == null || c.Dead || c.Downed) continue;

                if (c.CurJobDef != JobDefOf.LayDown && !c.InBed())
                {
                    allSleeping = false;
                    break;
                }
            }

            bool hasDanger = map.dangerWatcher != null && map.dangerWatcher.DangerRating == StoryDanger.High;

            if (allSleeping && !hasDanger)
            {
                if (!isSleepingFastForwarding)
                {
                    savedSpeed = Find.TickManager.CurTimeSpeed;
                    isSleepingFastForwarding = true;
                    TimeSpeed targetSpeed = (settings.SleepFastForwardSpeed == 4) ? TimeSpeed.Ultrafast : TimeSpeed.Superfast;
                    Find.TickManager.CurTimeSpeed = targetSpeed;
                }
            }
            else
            {
                if (isSleepingFastForwarding)
                {
                    isSleepingFastForwarding = false;
                    Find.TickManager.CurTimeSpeed = savedSpeed;
                }
            }
        }
    }
}
