using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESkStriker
    {
        public class PlanetarySentinel : TIESkStriker
        {
            public PlanetarySentinel() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Planetary Sentinel",
                    "",
                    Faction.Imperial,
                    1,
                    4,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 121,
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class PlanetarySentinelXWA : PlanetarySentinel
        {
            public PlanetarySentinelXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 8;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 5;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Gunner,
                    UpgradeType.Modification,
                    UpgradeType.Device
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}
