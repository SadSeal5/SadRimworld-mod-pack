using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SadSeal.MegaMod.QoL.Alerts
{
    public class Alert_TraderVisiting : Alert
    {
        public Alert_TraderVisiting()
        {
            defaultLabel = "MegaMod_AlertTraderTitle".Translate();
            defaultExplanation = "MegaMod_AlertTraderDesc".Translate();
        }

        public override AlertReport GetReport()
        {
            if (MegaMod.Settings?.QoL?.traderAlerts != true) return AlertReport.Inactive;

            List<Map> maps = Find.Maps;
            if (maps == null) return AlertReport.Inactive;

            for (int i = 0; i < maps.Count; i++)
            {
                Map map = maps[i];
                if (!map.IsPlayerHome) continue;

                var lords = map.lordManager?.lords;
                if (lords != null)
                {
                    for (int j = 0; j < lords.Count; j++)
                    {
                        Lord lord = lords[j];
                        if (lord.LordJob is LordJob_TradeWithColony)
                        {
                            Verse.Pawn leader = null;
                            for (int k = 0; k < lord.ownedPawns.Count; k++)
                            {
                                if (lord.ownedPawns[k].TraderKind != null)
                                {
                                    leader = lord.ownedPawns[k];
                                    break;
                                }
                            }

                            if (leader != null) return AlertReport.CulpritIs(leader);
                            if (lord.ownedPawns.Count > 0) return AlertReport.CulpritIs(lord.ownedPawns[0]);
                        }
                    }
                }

                if (map.passingShipManager?.passingShips != null && map.passingShipManager.passingShips.Count > 0)
                {
                    return AlertReport.Active;
                }
            }

            return AlertReport.Inactive;
        }
    }
}
