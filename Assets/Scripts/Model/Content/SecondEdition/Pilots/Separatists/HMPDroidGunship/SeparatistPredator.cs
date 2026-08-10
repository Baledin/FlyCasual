using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.HMPDroidGunship
    {
        public class SeparatistPredator : HMPDroidGunship
        {
            public SeparatistPredator() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Separatist Predator",
                    "",
                    Faction.Separatists,
                    3,
                    4,
                    9,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Missile,
                        UpgradeType.Missile,
                        UpgradeType.Device,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>
                    {
                        Tags.Droid
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class SeparatistPredatorXWA : SeparatistPredator
        {
            public SeparatistPredatorXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 10;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 11;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Modification,
                    UpgradeType.Device,
                    UpgradeType.Missile,
                    UpgradeType.Missile,
                    UpgradeType.Configuration
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}