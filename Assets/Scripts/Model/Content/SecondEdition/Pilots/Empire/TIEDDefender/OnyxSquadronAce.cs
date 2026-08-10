using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class OnyxSquadronAce : TIEDDefender
        {
            public OnyxSquadronAce() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Onyx Squadron Ace",
                    "",
                    Faction.Imperial,
                    4,
                    7,
                    6,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Cannon,
                        UpgradeType.Missile,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 125,
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class OnyxSquadronAceXWA : OnyxSquadronAce
        {
            public OnyxSquadronAceXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 16;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 10;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
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
