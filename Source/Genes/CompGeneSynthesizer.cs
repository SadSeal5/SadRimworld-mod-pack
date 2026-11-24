using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using SadSeal.MegaMod.Genes.UI;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SadSeal.MegaMod.Genes
{
    public class CompGeneSynthesizer : ThingComp, IThingHolder
    {
        public List<GeneSynthesisOrder> OrderQueue = new List<GeneSynthesisOrder>();
        public ThingOwner innerContainer;

        public static ThingDef NeutroamineDef => GeneThingDefOf.Neutroamine ?? DefDatabase<ThingDef>.GetNamed("Neutroamine", false);
        public static ThingDef ArchiteDef => GeneThingDefOf.ArchiteCapsule ?? DefDatabase<ThingDef>.GetNamed("ArchiteCapsule", false);

        public GeneSynthesisOrder CurrentOrder => OrderQueue != null && OrderQueue.Count > 0 ? OrderQueue[0] : null;
        public bool IsSynthesizing => CurrentOrder != null;

        // Compatibility properties for jobs and inspect
        public GeneDef TargetGene => CurrentOrder?.Genes?.FirstOrDefault();
        public int NeutroamineCost => CurrentOrder?.NeutroamineCost ?? 0;
        public int ArchiteCost => CurrentOrder?.ArchiteCost ?? 0;
        public int NeutroamineStored => GetStoredCount(NeutroamineDef);
        public int ArchiteStored => GetStoredCount(ArchiteDef);
        public int NeutroamineNeeded => CurrentOrder != null ? Mathf.Max(0, CurrentOrder.NeutroamineCost - NeutroamineStored) : 0;
        public int ArchiteNeeded => CurrentOrder != null ? Mathf.Max(0, CurrentOrder.ArchiteCost - ArchiteStored) : 0;
        public bool HasAllMaterials => CurrentOrder != null && NeutroamineNeeded == 0 && ArchiteNeeded == 0;
        public float WorkRequired => CurrentOrder?.WorkRequired ?? 0f;
        public float WorkDone
        {
            get => CurrentOrder?.WorkDone ?? 0f;
            set
            {
                if (CurrentOrder != null)
                {
                    CurrentOrder.WorkDone = value;
                }
            }
        }

        public CompGeneSynthesizer()
        {
            innerContainer = new ThingOwner<Thing>(this);
        }

        public int GetStoredCount(ThingDef def)
        {
            if (def == null || innerContainer == null)
            {
                return 0;
            }

            int total = 0;
            for (int i = 0; i < innerContainer.Count; i++)
            {
                Thing item = innerContainer[i];
                if (item?.def == def)
                {
                    total += item.stackCount;
                }
            }
            return total;
        }

        public ThingOwner GetDirectlyHeldThings() => innerContainer;

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (innerContainer == null)
            {
                innerContainer = new ThingOwner<Thing>(this);
            }
            if (OrderQueue == null)
            {
                OrderQueue = new List<GeneSynthesisOrder>();
            }
        }

        public void EnqueueOrder(GeneSynthesisOrder order)
        {
            if (order == null) return;
            if (OrderQueue == null)
            {
                OrderQueue = new List<GeneSynthesisOrder>();
            }
            OrderQueue.Add(order);

            // If this is the only order and it requires no work and already has all required materials, process immediately
            if (OrderQueue.Count == 1 && order.WorkRequired <= 0f && HasAllMaterials)
            {
                FinishCurrentOrder();
            }
        }

        public void EnqueueSingleGene(GeneDef gene, int neutroCost, float work)
        {
            if (gene == null) return;
            EnqueueOrder(new GeneSynthesisOrder(gene, neutroCost, work));
        }

        public void EnqueueBatchGenes(IEnumerable<GeneDef> genes, int neutroCostPerGene, float workPerGene)
        {
            if (genes == null) return;
            foreach (GeneDef gene in genes)
            {
                if (gene != null)
                {
                    EnqueueSingleGene(gene, neutroCostPerGene, workPerGene);
                }
            }
        }

        public void EnqueueCombinedXenotype(XenotypeDef xenotype, int totalCost, float totalWork)
        {
            if (xenotype == null || xenotype.genes == null || xenotype.genes.Count == 0) return;
            List<GeneDef> validGenes = xenotype.genes.Where(g => g != null).ToList();
            if (validGenes.Count == 0) return;

            string label = $"{xenotype.LabelCap} ({"GeneSynthesis_CombinedPack".Translate()})";
            GeneSynthesisOrder order = new GeneSynthesisOrder(validGenes, label, totalCost, totalWork, isCombinedPack: true, sourceXenotype: xenotype);
            EnqueueOrder(order);
        }

        public void RemoveOrderAt(int index)
        {
            if (OrderQueue == null || index < 0 || index >= OrderQueue.Count) return;

            if (index == 0)
            {
                CancelCurrentOrder();
            }
            else
            {
                OrderQueue.RemoveAt(index);
            }
        }

        public void ClearQueue()
        {
            CancelAllOrders();
        }

        // Backward compatibility
        public void StartSynthesis(GeneDef gene, int neutroCost, float work)
        {
            EnqueueSingleGene(gene, neutroCost, work);
        }

        public void CancelSynthesis()
        {
            CancelCurrentOrder();
        }

        public void FinishSynthesis()
        {
            FinishCurrentOrder();
        }

        public void CancelCurrentOrder()
        {
            if (innerContainer != null && innerContainer.Count > 0 && parent.Spawned && parent.Map != null)
            {
                innerContainer.TryDropAll(parent.Position, parent.Map, ThingPlaceMode.Near);
            }
            innerContainer?.Clear();

            if (OrderQueue != null && OrderQueue.Count > 0)
            {
                OrderQueue.RemoveAt(0);
            }
        }

        public void CancelAllOrders()
        {
            if (innerContainer != null && innerContainer.Count > 0 && parent.Spawned && parent.Map != null)
            {
                innerContainer.TryDropAll(parent.Position, parent.Map, ThingPlaceMode.Near);
            }
            innerContainer?.Clear();

            OrderQueue?.Clear();
        }

        private void ConsumeFromContainer(ThingDef def, int amount)
        {
            if (def == null || innerContainer == null || amount <= 0)
            {
                return;
            }

            int remaining = amount;
            for (int i = innerContainer.Count - 1; i >= 0 && remaining > 0; i--)
            {
                Thing item = innerContainer[i];
                if (item?.def != def) continue;

                int count = Mathf.Min(item.stackCount, remaining);
                item.SplitOff(count).Destroy();
                remaining -= count;
            }
        }

        private void CompleteAndConsumeOrder(GeneSynthesisOrder order, IntVec3 pos, Map map)
        {
            if (order.Genes != null && order.Genes.Count > 0)
            {
                Genepack pack = (Genepack)ThingMaker.MakeThing(ThingDefOf.Genepack);
                pack.Initialize(order.Genes);
                if (map != null && parent.Spawned)
                {
                    GenPlace.TryPlaceThing(pack, pos, map, ThingPlaceMode.Near);
                }

                SoundDefOf.GeneAssembler_Complete?.PlayOneShot(SoundInfo.InMap(parent));

                if (order.IsCombinedPack && order.SourceXenotype != null)
                {
                    Messages.Message("GeneSynthesis_XenotypeFinished".Translate(order.SourceXenotype.LabelCap), pack, MessageTypeDefOf.PositiveEvent);
                }
                else
                {
                    Messages.Message("GeneSynthesis_Finished".Translate(order.Label), pack, MessageTypeDefOf.PositiveEvent);
                }
            }

            ConsumeFromContainer(NeutroamineDef, order.NeutroamineCost);
            ConsumeFromContainer(ArchiteDef, order.ArchiteCost);
        }

        public void FinishCurrentOrder()
        {
            if (CurrentOrder == null || parent == null || !parent.Spawned || parent.Map == null)
            {
                return;
            }

            Map map = parent.Map;
            IntVec3 pos = parent.Position;

            CompleteAndConsumeOrder(CurrentOrder, pos, map);

            if (OrderQueue != null && OrderQueue.Count > 0)
            {
                OrderQueue.RemoveAt(0);
            }

            // If the next order is instant craft and has all required materials, finish it too
            while (CurrentOrder != null && CurrentOrder.WorkRequired <= 0f && HasAllMaterials)
            {
                CompleteAndConsumeOrder(CurrentOrder, pos, map);

                if (OrderQueue != null && OrderQueue.Count > 0)
                {
                    OrderQueue.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (GenesModule.Instance?.IsEnabled != true) yield break;

            foreach (Gizmo g in base.CompGetGizmosExtra())
            {
                yield return g;
            }

            // Always allow opening the synthesizer dialog so players can queue additional genes or presets
            yield return new Command_Action
            {
                defaultLabel = "GeneSynthesis_GizmoSynthesize".Translate(),
                defaultDesc = "GeneSynthesis_GizmoSynthesizeDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Gizmos/RecombineGenes", false) ?? TexCommand.DesirePower,
                action = () =>
                {
                    Find.WindowStack.Add(new Dialog_GeneSynthesizer(this));
                }
            };

            if (IsSynthesizing)
            {
                yield return new Command_Action
                {
                    defaultLabel = "GeneSynthesis_GizmoCancel".Translate(),
                    defaultDesc = "GeneSynthesis_GizmoCancelDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel", true),
                    action = CancelCurrentOrder
                };

                if (OrderQueue.Count > 1)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "GeneSynthesis_GizmoClearAll".Translate(),
                        defaultDesc = "GeneSynthesis_GizmoClearAllDesc".Translate(),
                        icon = ContentFinder<Texture2D>.Get("UI/Commands/ForbidOff", false) ?? ContentFinder<Texture2D>.Get("UI/Designators/Cancel", true),
                        action = CancelAllOrders
                    };
                }

                if (Prefs.DevMode)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "GeneSynthesis_GizmoDevFinish".Translate(),
                        defaultDesc = "GeneSynthesis_GizmoDevFinishDesc".Translate(),
                        icon = TexCommand.DesirePower,
                        action = FinishCurrentOrder
                    };
                }
            }
        }

        public override string CompInspectStringExtra()
        {
            if (GenesModule.Instance?.IsEnabled != true) return null;
            if (!IsSynthesizing || CurrentOrder == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("GeneSynthesis_TargetGene".Translate(CurrentOrder.Label));

            if (OrderQueue.Count > 1)
            {
                sb.AppendLine("GeneSynthesis_QueueProgress".Translate(1, OrderQueue.Count));
            }

            sb.AppendLine("GeneSynthesis_NeutroamineProgress".Translate(NeutroamineStored, CurrentOrder.NeutroamineCost));
            if (CurrentOrder.ArchiteCost > 0)
            {
                string architeLabel = ArchiteDef != null ? ArchiteDef.LabelCap.ToString() : "Archite Capsule";
                sb.AppendLine($"{architeLabel}: {ArchiteStored} / {CurrentOrder.ArchiteCost}");
            }
            float pct = CurrentOrder.WorkRequired > 0f ? (CurrentOrder.WorkDone / CurrentOrder.WorkRequired) : 1f;
            sb.Append("GeneSynthesis_WorkProgress".Translate(pct.ToStringPercent()));

            return sb.ToString().TrimEndNewlines();
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (innerContainer != null && innerContainer.Count > 0 && previousMap != null)
            {
                innerContainer.TryDropAll(parent.Position, previousMap, ThingPlaceMode.Near);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref OrderQueue, "orderQueue", LookMode.Deep);
            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);

            // Backward compatibility loading from older save files
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                GeneDef legacyGene = null;
                Scribe_Defs.Look(ref legacyGene, "targetGene");
                if (legacyGene != null)
                {
                    int legacyNeutro = 0;
                    float legacyDone = 0f;
                    float legacyReq = 0f;
                    bool legacySynth = false;
                    Scribe_Values.Look(ref legacyNeutro, "neutroamineCost", 0);
                    Scribe_Values.Look(ref legacyDone, "workDone", 0f);
                    Scribe_Values.Look(ref legacyReq, "workRequired", 0f);
                    Scribe_Values.Look(ref legacySynth, "isSynthesizing", false);

                    if (legacySynth)
                    {
                        if (OrderQueue == null)
                        {
                            OrderQueue = new List<GeneSynthesisOrder>();
                        }
                        OrderQueue.Add(new GeneSynthesisOrder(legacyGene, legacyNeutro, legacyReq) { WorkDone = legacyDone });
                    }
                }
            }

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (innerContainer == null)
                {
                    innerContainer = new ThingOwner<Thing>(this);
                }
                if (OrderQueue == null)
                {
                    OrderQueue = new List<GeneSynthesisOrder>();
                }
                OrderQueue.RemoveAll(o => o == null || o.Genes == null || o.Genes.Count == 0);
            }
        }
    }
}
