using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;

namespace SadSeal.MegaMod.QoL.RedAlert
{
    public class RedAlertManager : MapComponent
    {
        private bool isRedAlertActive;
        public bool IsRedAlertActive => isRedAlertActive;

        private Dictionary<Pawn, Area> savedAllowedAreas = new Dictionary<Pawn, Area>();
        private HashSet<Pawn> autoDraftedPawns = new HashSet<Pawn>();

        public RedAlertManager(Map map) : base(map)
        {
        }

        public void ToggleRedAlert()
        {
            if (isRedAlertActive)
            {
                DeactivateRedAlert();
            }
            else
            {
                ActivateRedAlert();
            }
        }

        public void ActivateRedAlert()
        {
            if (isRedAlertActive) return;
            isRedAlertActive = true;

            Area safeArea = null;
            if (map.areaManager != null)
            {
                safeArea = map.areaManager.AllAreas.FirstOrDefault(a =>
                {
                    string label = a.Label;
                    if (string.IsNullOrEmpty(label)) return false;
                    return label.IndexOf("Bunker", StringComparison.OrdinalIgnoreCase) >= 0 ||
                           label.IndexOf("Shelter", StringComparison.OrdinalIgnoreCase) >= 0 ||
                           label.IndexOf("Safe", StringComparison.OrdinalIgnoreCase) >= 0 ||
                           label.IndexOf("Siginak", StringComparison.OrdinalIgnoreCase) >= 0;
                });

                if (safeArea == null)
                {
                    safeArea = map.areaManager.Home;
                }
            }

            if (map.mapPawns != null)
            {
                var colonists = map.mapPawns.FreeColonistsSpawned;
                if (colonists != null)
                {
                    foreach (Pawn pawn in colonists)
                    {
                        if (pawn == null || pawn.Dead) continue;

                        bool isNonViolent = pawn.WorkTagIsDisabled(WorkTags.Violent) || pawn.drafter == null;
                        if (isNonViolent)
                        {
                            if (pawn.playerSettings != null && safeArea != null)
                            {
                                savedAllowedAreas[pawn] = pawn.playerSettings.AreaRestrictionInPawnCurrentMap;
                                pawn.playerSettings.AreaRestrictionInPawnCurrentMap = safeArea;
                            }
                        }
                        else
                        {
                            if (pawn.drafter != null && !pawn.drafter.Drafted && !pawn.Downed)
                            {
                                pawn.drafter.Drafted = true;
                                autoDraftedPawns.Add(pawn);
                            }
                        }
                    }
                }

                var animals = map.mapPawns.SpawnedColonyAnimals;
                if (animals != null && safeArea != null)
                {
                    foreach (Pawn animal in animals)
                    {
                        if (animal == null || animal.Dead || animal.playerSettings == null) continue;

                        savedAllowedAreas[animal] = animal.playerSettings.AreaRestrictionInPawnCurrentMap;
                        animal.playerSettings.AreaRestrictionInPawnCurrentMap = safeArea;
                    }
                }
            }

            Messages.Message("MegaMod_RedAlert_Activated".Translate(), MessageTypeDefOf.ThreatBig, false);
            (DefDatabase<SoundDef>.GetNamedSilentFail("RaidArrived") ?? DefDatabase<SoundDef>.GetNamedSilentFail("LetterArrive_BadUrgentBig") ?? SoundDefOf.LetterArrive)?.PlayOneShotOnCamera(map);
        }

        public void DeactivateRedAlert()
        {
            if (!isRedAlertActive) return;
            isRedAlertActive = false;

            foreach (var kvp in savedAllowedAreas)
            {
                Pawn pawn = kvp.Key;
                if (pawn != null && !pawn.Dead && pawn.Spawned && pawn.playerSettings != null)
                {
                    pawn.playerSettings.AreaRestrictionInPawnCurrentMap = kvp.Value;
                }
            }
            savedAllowedAreas.Clear();

            foreach (Pawn pawn in autoDraftedPawns)
            {
                if (pawn != null && !pawn.Dead && pawn.Spawned && pawn.drafter != null)
                {
                    pawn.drafter.Drafted = false;
                }
            }
            autoDraftedPawns.Clear();

            Messages.Message("MegaMod_RedAlert_Deactivated".Translate(), MessageTypeDefOf.PositiveEvent, false);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref isRedAlertActive, "isRedAlertActive", false);
            Scribe_Collections.Look(ref savedAllowedAreas, "savedAllowedAreas", LookMode.Reference, LookMode.Reference);
            Scribe_Collections.Look(ref autoDraftedPawns, "autoDraftedPawns", LookMode.Reference);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (savedAllowedAreas == null)
                {
                    savedAllowedAreas = new Dictionary<Pawn, Area>();
                }
                else
                {
                    var nullKeys = savedAllowedAreas.Where(kvp => kvp.Key == null).Select(kvp => kvp.Key).ToList();
                    foreach (var k in nullKeys)
                    {
                        savedAllowedAreas.Remove(k);
                    }
                }

                if (autoDraftedPawns == null)
                {
                    autoDraftedPawns = new HashSet<Pawn>();
                }
                else
                {
                    autoDraftedPawns.RemoveWhere(p => p == null);
                }
            }
        }
    }
}
