using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.StarViperClassAttackPlatform
    {
        public class BlackSunAssassin : StarViperClassAttackPlatform
        {
            public BlackSunAssassin() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Black Sun Assassin",
                    "",
                    Faction.Scum,
                    3,
                    5,
                    6,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Tech
                    },
                    seImageNumber: 181,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );

                ModelInfo.SkinName = "Black Sun Assassin";
            }
        }

        public class BlackSunAssassinXWA : BlackSunAssassin
        {
            public BlackSunAssassinXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 11;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 11;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Torpedo
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}