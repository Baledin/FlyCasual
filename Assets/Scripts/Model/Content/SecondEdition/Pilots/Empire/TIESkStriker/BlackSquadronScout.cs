using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESkStriker
    {
        public class BlackSquadronScout : TIESkStriker
        {
            public BlackSquadronScout() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Black Squadron Scout",
                    "",
                    Faction.Imperial,
                    3,
                    4,
                    5,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Gunner,
                        UpgradeType.Device
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 120,
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class BlackSquadronScoutXWA : BlackSquadronScout
        {
            public BlackSquadronScoutXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 9;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 10;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Gunner,
                    UpgradeType.Modification,
                    UpgradeType.Device,
                };
            }
        }
    }
}
