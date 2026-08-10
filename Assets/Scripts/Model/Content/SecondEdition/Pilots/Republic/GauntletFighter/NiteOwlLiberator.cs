using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class NiteOwlLiberator : GauntletFighter
        {
            public NiteOwlLiberator() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Nite Owl Liberator",
                    "Resolute Warrior",
                    Faction.Republic,
                    2,
                    7,
                    12,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Device,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian
                    },
                    skinName: "Blue",
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class NiteOwlLiberatorXWA : NiteOwlLiberator
        {
            public NiteOwlLiberatorXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 15;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 18;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
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