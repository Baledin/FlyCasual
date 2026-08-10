using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NantexClassStarfighter
    {
        public class StalgasinHiveGuard : NantexClassStarfighter
        {
            public StalgasinHiveGuard() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Stalgasin Hive Guard",
                    "",
                    Faction.Separatists,
                    3,
                    4,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class StalgasinHiveGuardXWA : StalgasinHiveGuard
        {
            public StalgasinHiveGuardXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 9;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 5;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}