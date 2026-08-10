using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.EWing
    {
        public class KnaveSquadronEscort : EWing
        {
            public KnaveSquadronEscort() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Knave Squadron Escort",
                    "",
                    Faction.Rebel,
                    2,
                    5,
                    14,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Tech,
                        UpgradeType.Astromech,
                        UpgradeType.Modification
                    },
                    seImageNumber: 53,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }

        public class KnaveSquadronEscortXWA : KnaveSquadronEscort
        {
            public KnaveSquadronEscortXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 12;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 8;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
                {
                    UpgradeType.Astromech,
                    UpgradeType.Sensor,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Torpedo
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}