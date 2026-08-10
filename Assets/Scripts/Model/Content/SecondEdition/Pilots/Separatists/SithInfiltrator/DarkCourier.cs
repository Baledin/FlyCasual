using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.SithInfiltrator
{
    public class DarkCourier : SithInfiltrator
    {
        public DarkCourier()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Dark Courier",
                "",
                Faction.Separatists,
                2,
                6,
                9,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Torpedo,
                    UpgradeType.Cannon,
                    UpgradeType.Device,
                    UpgradeType.Modification
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class DarkCourierXWA : DarkCourier
    {
        public DarkCourierXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 12;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 6;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Crew,
                UpgradeType.Crew,
                UpgradeType.Sensor,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Device,
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}