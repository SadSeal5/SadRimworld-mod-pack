using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.Medical.Autopsy
{
    public static class AutopsyHelper
    {
        private static readonly Dictionary<string, string> OrganDefMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Heart", "Heart" },
            { "Lung", "Lung" },
            { "Kidney", "Kidney" },
            { "Liver", "Liver" }
        };

        public static void PerformAutopsy(Pawn billDoer, Corpse corpse, Thing medicineUsed = null)
        {
            if (billDoer == null || corpse == null || corpse.InnerPawn == null) return;

            var settings = MegaMod.Settings?.Medical;
            if (settings == null || !settings.EnableAutopsy) return;

            Pawn pawn = corpse.InnerPawn;
            Map map = billDoer.Map;
            IntVec3 pos = billDoer.Position;

            float doctorSkill = billDoer.skills != null ? billDoer.skills.GetSkill(SkillDefOf.Medicine).Level : 5f;
            float baseChance = settings.AutopsySuccessBaseChance + (doctorSkill * settings.AutopsySkillFactor);

            // Medicine scaling
            float medicineBonus = 0f;
            if (medicineUsed != null)
            {
                float potency = medicineUsed.GetStatValue(StatDefOf.MedicalPotency);
                // Scaled bonus based on potency: herbal (+2.5%), industrial (+12.5%), ultratech (+27.5%)
                medicineBonus = (potency - 0.5f) * 0.25f;
            }
            else if (settings.AutopsyRequiresMedicine)
            {
                // Severe penalty if medicine was required by settings but none was provided
                baseChance *= 0.50f;
            }

            float successChance = Mathf.Clamp(baseChance + medicineBonus, 0.10f, 0.98f);
            int extractedCount = 0;

            if (pawn.RaceProps.IsMechanoid && settings.ExtractMechanoidParts)
            {
                // Mechanoid extraction
                int steelAmount = Mathf.RoundToInt(pawn.BodySize * 15f * successChance);
                int plasteelAmount = Mathf.RoundToInt(pawn.BodySize * 8f * successChance);

                if (steelAmount > 0)
                {
                    Thing steel = ThingMaker.MakeThing(ThingDefOf.Steel);
                    steel.stackCount = steelAmount;
                    GenSpawn.Spawn(steel, pos, map);
                    extractedCount++;
                }

                if (plasteelAmount > 0)
                {
                    Thing plasteel = ThingMaker.MakeThing(ThingDefOf.Plasteel);
                    plasteel.stackCount = plasteelAmount;
                    GenSpawn.Spawn(plasteel, pos, map);
                    extractedCount++;
                }

                if (Rand.Value < successChance * 0.5f)
                {
                    Thing comp = ThingMaker.MakeThing(ThingDefOf.ComponentIndustrial);
                    comp.stackCount = Rand.RangeInclusive(1, 2);
                    GenSpawn.Spawn(comp, pos, map);
                    extractedCount++;
                }

                Messages.Message("MegaMod_AutopsyMechanoidSuccess".Translate(billDoer.LabelShort, corpse.LabelShort, extractedCount),
                    MessageTypeDefOf.PositiveEvent, false);
                return;
            }

            // 1. Extract Bionics and Artificial Implants
            List<Hediff> hediffs = new List<Hediff>(pawn.health.hediffSet.hediffs);
            for (int i = 0; i < hediffs.Count; i++)
            {
                Hediff h = hediffs[i];
                ThingDef dropDef = null;

                if (h is Hediff_AddedPart added)
                {
                    dropDef = added.def.spawnThingOnRemoved;
                }
                else if (h is Hediff_Implant implant)
                {
                    dropDef = implant.def.spawnThingOnRemoved;
                }

                if (dropDef != null)
                {
                    if (Rand.Value < (settings.AutopsyBionicExtractChance * successChance))
                    {
                        Thing bionic = ThingMaker.MakeThing(dropDef);
                        GenSpawn.Spawn(bionic, pos, map);
                        extractedCount++;
                    }
                    pawn.health.RemoveHediff(h);
                }
            }

            // 2. Extract Biological Organs (only from fresh corpses)
            if (corpse.GetRotStage() == RotStage.Fresh)
            {
                List<BodyPartRecord> notMissingParts = new List<BodyPartRecord>(pawn.health.hediffSet.GetNotMissingParts());
                for (int i = 0; i < notMissingParts.Count; i++)
                {
                    BodyPartRecord part = notMissingParts[i];
                    if (part == null || part.def == null) continue;

                    string organThingDefName = null;
                    if (OrganDefMapping.TryGetValue(part.def.defName, out string mapped))
                    {
                        organThingDefName = mapped;
                    }
                    else
                    {
                        ThingDef direct = DefDatabase<ThingDef>.GetNamedSilentFail(part.def.defName);
                        if (direct != null && direct.category == ThingCategory.Item)
                        {
                            organThingDefName = direct.defName;
                        }
                    }

                    if (!string.IsNullOrEmpty(organThingDefName))
                    {
                        ThingDef organDef = DefDatabase<ThingDef>.GetNamedSilentFail(organThingDefName);
                        if (organDef != null)
                        {
                            if (Rand.Value < successChance)
                            {
                                Thing organ = ThingMaker.MakeThing(organDef);
                                GenSpawn.Spawn(organ, pos, map);
                                extractedCount++;
                            }

                            // Mark part as missing so it cannot be extracted again
                            Hediff_MissingPart missing = (Hediff_MissingPart)HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, pawn, part);
                            missing.IsFresh = false;
                            pawn.health.hediffSet.AddDirect(missing);
                        }
                    }
                }
            }

            if (extractedCount > 0)
            {
                Messages.Message("MegaMod_AutopsySuccess".Translate(billDoer.LabelShort, corpse.LabelShort, extractedCount),
                    MessageTypeDefOf.PositiveEvent, false);
            }
            else
            {
                Messages.Message("MegaMod_AutopsyFailed".Translate(billDoer.LabelShort, corpse.LabelShort),
                    MessageTypeDefOf.NeutralEvent, false);
            }
        }
    }
}
