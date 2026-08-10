using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class DeltaSquadronPilot : TIEDDefender
        {
            public DeltaSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Delta Squadron Pilot",
                    "",
                    Faction.Imperial,
                    1,
                    7,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Cannon,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 126,
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class DeltaSquadronPilotXWA : DeltaSquadronPilot
        {
            public DeltaSquadronPilotXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 15;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 9;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Sensor,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Cannon,
                    UpgradeType.Missile,
                    UpgradeType.Configuration
                };
            }
        }
    }
}
