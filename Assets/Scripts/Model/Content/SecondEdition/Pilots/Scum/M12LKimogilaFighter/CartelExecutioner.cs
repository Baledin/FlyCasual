using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.M12LKimogilaFighter
    {
        public class CartelExecutioner : M12LKimogilaFighter
        {
            public CartelExecutioner() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Cartel Executioner",
                    "",
                    Faction.Scum,
                    3,
                    5,
                    3,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Missile
                    },
                    seImageNumber: 209,
                    legality: new List<Legality>() { Legality.ExtendedLegal },
                    skinName: "Cartel Executioner"
                );
            }
        }

        public class CartelExecutionerXWA : CartelExecutioner
        {
            public CartelExecutionerXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 10;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 8;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Astromech,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Missile,
                    UpgradeType.Torpedo
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}