using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class TradeFederationDrone : VultureClassDroidFighter
    {
        public TradeFederationDrone()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Trade Federation Drone",
                "",
                Faction.Separatists,
                1,
                2,
                0,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class TradeFederationDroneXWA : TradeFederationDrone
    {
        public TradeFederationDroneXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 5;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 3;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Modification,
                UpgradeType.Missile,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}