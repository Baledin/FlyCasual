using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEPhPhantom
    {
        public class SigmaSquadronAce : TIEPhPhantom
        {
            public SigmaSquadronAce() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Sigma Squadron Ace",
                    "",
                    Faction.Imperial,
                    4,
                    6,
                    9,
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Gunner,
                        UpgradeType.Modification
                    },
                    seImageNumber: 133,
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class SigmaSquadronAceXWA : SigmaSquadronAce
        {
            public SigmaSquadronAceXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 12;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 11;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Gunner,
                    UpgradeType.Modification,
                    UpgradeType.Tech
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}
