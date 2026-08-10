using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NabooRoyalN1Starfighter
    {
        public class BravoFlightOfficer : NabooRoyalN1Starfighter
        {
            public BravoFlightOfficer() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Bravo Flight Officer",
                    "",
                    Faction.Republic,
                    2,
                    4,
                    8,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Astromech,
                        UpgradeType.Sensor,
                        UpgradeType.Torpedo,
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class BravoFlightOfficerXWA : BravoFlightOfficer
        {
            public BravoFlightOfficerXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 7;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 3;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                    UpgradeType.Sensor,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Torpedo
                };
            }
        }
    }
}
