using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.LaatIGunship
    {
        public class P212thBattalionPilot : LaatIGunship
        {
            public P212thBattalionPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "212th Battalion Pilot",
                    "",
                    Faction.Republic,
                    2,
                    5,
                    7,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Gunner,
                        UpgradeType.Modification,
                        UpgradeType.Missile,
                        UpgradeType.Missile,
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class P212thBattalionPilotXWA : P212thBattalionPilot
        {
            public P212thBattalionPilotXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 11;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 9;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Crew,
                    UpgradeType.Crew,
                    UpgradeType.Gunner,
                    UpgradeType.Gunner,
                    UpgradeType.Modification,
                    UpgradeType.Missile,
                    UpgradeType.Missile,
                    UpgradeType.Torpedo
                };
            }
        }
    }
}