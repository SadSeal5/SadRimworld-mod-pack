using System.Collections.Generic;
using RimWorld;
using Verse;

namespace SadSeal.MegaMod.QoL
{
    [StaticConstructorOnStartup]
    public static class QoLApplier
    {
        private static readonly Dictionary<ThingDef, float> originalTreeLifespans = new Dictionary<ThingDef, float>();
        private static bool originalLifespansCached;

        static QoLApplier()
        {
            ApplyAll();
        }

        public static void ApplyAll()
        {
            if (MegaMod.Settings?.Modules != null && !MegaMod.Settings.Modules.enableQoL) return;
            var settings = MegaMod.Settings?.QoL;
            if (settings == null) return;

            ApplyMetalFlammability(settings.metalDoesntBurn);
            ApplyPreciousMetals(settings.prettyPreciousMetals);
            ApplyChunkStackLimits(settings.stackableChunks, settings.chunkStackLimit);
            ApplySmoothPriorities(settings.smoothWallsBeforeFloors);
            ApplyTreeLifespan(settings.treesDontDie);
            ApplyQoLSettings();
        }

        public static void ApplyQoLSettings()
        {
            if (MegaMod.Settings?.QoL?.filthVanishes == true)
            {
                foreach (ThingDef td in DefDatabase<ThingDef>.AllDefs)
                {
                    if (td.filth != null)
                    {
                        td.filth.rainWashes = true;
                        if (td.filth.disappearsInDays.TrueMax <= 0f || td.filth.disappearsInDays.TrueMax > 15f)
                        {
                            td.filth.disappearsInDays = new FloatRange(5f, 10f);
                        }
                    }
                }
            }
        }

        public static void ApplyMetalFlammability(bool noBurn)
        {
            ThingDef[] metals = { ThingDefOf.Steel, ThingDefOf.Plasteel };
            foreach (var def in metals)
            {
                if (def == null) continue;

                float defaultFlam = (def == ThingDefOf.Steel) ? 0.4f : 0f;
                def.SetStatBaseValue(StatDefOf.Flammability, noBurn ? 0f : defaultFlam);

                if (def.stuffProps?.statFactors != null)
                {
                    var factor = def.stuffProps.statFactors.Find(m => m.stat == StatDefOf.Flammability);
                    if (factor != null)
                    {
                        factor.value = noBurn ? 0f : defaultFlam;
                    }
                    else if (noBurn)
                    {
                        def.stuffProps.statFactors.Add(new StatModifier { stat = StatDefOf.Flammability, value = 0f });
                    }
                }
            }
        }

        public static void ApplyPreciousMetals(bool pretty)
        {
            if (ThingDefOf.Gold != null)
            {
                ThingDefOf.Gold.SetStatBaseValue(StatDefOf.Beauty, pretty ? 4f : 0f);
            }

            if (ThingDefOf.Silver != null)
            {
                ThingDefOf.Silver.SetStatBaseValue(StatDefOf.Beauty, pretty ? 2f : 0f);
            }
        }

        public static void ApplyChunkStackLimits(bool stackable, int limit)
        {
            int targetLimit = stackable ? limit : 1;
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
            {
                if (def.category == ThingCategory.Item &&
                    ((def.thingCategories != null && def.thingCategories.Contains(ThingCategoryDefOf.StoneChunks)) ||
                     def == ThingDefOf.ChunkSlagSteel ||
                     (def.defName != null && def.defName.Contains("Chunk"))))
                {
                    def.stackLimit = targetLimit;
                }
            }
        }

        public static void ApplySmoothPriorities(bool smoothWallsFirst)
        {
            WorkGiverDef wall = DefDatabase<WorkGiverDef>.GetNamedSilentFail("ConstructSmoothWalls");
            WorkGiverDef floor = DefDatabase<WorkGiverDef>.GetNamedSilentFail("ConstructSmoothFloors");

            if (wall != null && floor != null)
            {
                if (smoothWallsFirst)
                {
                    wall.priorityInType = 110;
                    floor.priorityInType = 90;
                }
                else
                {
                    wall.priorityInType = 100;
                    floor.priorityInType = 100;
                }
            }
        }

        public static void ApplyTreeLifespan(bool treesDontDie)
        {
            if (!originalLifespansCached)
            {
                foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
                {
                    if (def.plant != null && def.plant.IsTree)
                    {
                        originalTreeLifespans[def] = def.plant.lifespanDaysPerGrowDays;
                    }
                }
                originalLifespansCached = true;
            }

            foreach (var kvp in originalTreeLifespans)
            {
                ThingDef def = kvp.Key;
                float orig = kvp.Value;
                if (def.plant != null)
                {
                    def.plant.lifespanDaysPerGrowDays = treesDontDie ? 0f : orig;
                }
            }
        }
    }
}
