using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.ASF01BWing
{
    public class BlueSquadronPilot : ASF01BWing
    {
        public BlueSquadronPilot() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Blue Squadron Pilot",
                "",
                Faction.Rebel,
                2,
                4,
                4,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Sensor,
                    UpgradeType.Cannon,
                    UpgradeType.Cannon,
                    UpgradeType.Device,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.BWing
                },
                seImageNumber: 26,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class BlueSquadronPilotXWA : BlueSquadronPilot
    {
        public BlueSquadronPilotXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 10;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 10;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Sensor,
                UpgradeType.Modification,
                UpgradeType.Cannon,
                UpgradeType.Cannon,
                UpgradeType.Missile,
                UpgradeType.Torpedo,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}
