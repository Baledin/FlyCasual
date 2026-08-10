using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class ShadowCollectiveOperator : GauntletFighter
        {
            public ShadowCollectiveOperator() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Shadow Collective Operator",
                    "Stoic Super Commando",
                    Faction.Scum,
                    1,
                    7,
                    10,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
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
                    skinName: "Red",
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class ShadowCollectiveOperatorXWA : ShadowCollectiveOperator
        {
            public ShadowCollectiveOperatorXWA() : base()
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