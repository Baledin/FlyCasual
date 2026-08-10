using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.UpsilonClassCommandShuttle
{
    public class StarkillerBasePilot : UpsilonClassCommandShuttle
    {
        public StarkillerBasePilot() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Starkiller Base Pilot",
                "",
                Faction.FirstOrder,
                2,
                7,
                8,
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Sensor,
                    UpgradeType.Tech,
                    UpgradeType.Tech,
                    UpgradeType.Cannon,
                    UpgradeType.Crew,
                    UpgradeType.Crew,
                    UpgradeType.Modification
                },
                legality: new List<Legality>() { Legality.ExtendedLegal }
            );
        }
    }

    public class StarkillerBasePilotXWA : StarkillerBasePilot
    {
        public StarkillerBasePilotXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 15;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 18;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Crew,
                UpgradeType.Crew,
                UpgradeType.Crew,
                UpgradeType.Sensor,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Tech,
                UpgradeType.Cannon
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}