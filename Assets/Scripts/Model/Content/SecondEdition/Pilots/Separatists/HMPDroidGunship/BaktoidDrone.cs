using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.HMPDroidGunship
    {
        public class BaktoidDrone : HMPDroidGunship
        {
            public BaktoidDrone() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Baktoid Drone",
                    "",
                    Faction.Separatists,
                    1,
                    4,
                    8,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
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

        public class BaktoidDroneXWA : BaktoidDrone
        {
            public BaktoidDroneXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 9;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 7;
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