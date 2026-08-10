using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class ImperialSuperCommando : GauntletFighter
        {
            public ImperialSuperCommando() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Imperial Super Commando",
                    "",
                    Faction.Imperial,
                    2,
                    7,
                    10,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian
                    },
                    skinName: "Gray",
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class ImperialSuperCommandoXWA : ImperialSuperCommando
        {
            public ImperialSuperCommandoXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 15;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 18;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Crew,
                    UpgradeType.Gunner,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Modification,
                    UpgradeType.Device,
                    UpgradeType.Configuration
                };
            }
        }
    }
}