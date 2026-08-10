using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class CrymorahGoon : BTLA4YWing
        {
            public CrymorahGoon() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Crymorah Goon",
                    "",
                    Faction.Scum,
                    1,
                    4,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Illicit,
                        UpgradeType.Device,
                        UpgradeType.Turret,
                        UpgradeType.Missile
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    },
                    seImageNumber: 168,
                    skinName: "Brown",
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class CrymorahGoonXWA : CrymorahGoon
        {
            public CrymorahGoonXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 7;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 4;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Astromech,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Device,
                    UpgradeType.Turret,
                    UpgradeType.Missile,
                };
            }
        }
    }
}
