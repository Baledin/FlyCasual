using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AlphaClassStarWing
    {
        public class RhoSquadronPilot : AlphaClassStarWing
        {
            public RhoSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Rho Squadron Pilot",
                    "",
                    Faction.Imperial,
                    3,
                    5,
                    9,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    },
                    seImageNumber: 137,
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class RhoSquadronPilotXWA : RhoSquadronPilot
        {
            public RhoSquadronPilotXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 11;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 19;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Missile,
                    UpgradeType.Modification,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                };
            }
        }
    }
}
