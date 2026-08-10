using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ARC170Starfighter
    {
        public class P104thBattalionPilot : ARC170Starfighter
        {
            public P104thBattalionPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "104th Battalion Pilot",
                    "",
                    Faction.Republic,
                    2,
                    5,
                    8,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech,
                        UpgradeType.Gunner,
                        UpgradeType.Gunner,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    },
                    skinName: "Red",
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class P104thBattalionPilotXWA : P104thBattalionPilot
        {
            public P104thBattalionPilotXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 10;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 3;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Astromech,
                    UpgradeType.Crew,
                    UpgradeType.Gunner,
                    UpgradeType.Modification,
                    UpgradeType.Torpedo
                };
                (PilotInfo as PilotCardInfo25).Limited = 2;
            }
        }
    }
}
