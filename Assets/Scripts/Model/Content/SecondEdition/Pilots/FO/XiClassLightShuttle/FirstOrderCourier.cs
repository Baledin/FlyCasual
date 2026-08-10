using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.XiClassLightShuttle
{
    public class FirstOrderCourier : XiClassLightShuttle
    {
        public FirstOrderCourier() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "First Order Courier",
                "",
                Faction.FirstOrder,
                2,
                4,
                10,
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Tech,
                    UpgradeType.Tech,
                    UpgradeType.Crew,
                    UpgradeType.Modification
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class FirstOrderCourierXWA : FirstOrderCourier
    {
        public FirstOrderCourierXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 9;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 10;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Crew,
                UpgradeType.Crew,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Tech
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}