using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.BTANR2WYWing
{
    public class KijimiSpiceRunner : BTANR2WYWing
    {
        public KijimiSpiceRunner() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Kijimi Spice Runner",
                "",
                Faction.Resistance,
                2,
                11,
                16,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Astromech,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Device,
                    UpgradeType.Turret,
                    UpgradeType.Missile,
                    UpgradeType.Torpedo
                },
                tags: new List<Tags>
                {
                    Tags.YWing
                },
                skinName: "Red",
                legality: new List<Legality> { Legality.XWA }
            );

            PilotNameCanonical = "kijimispicerunner-wartime";
        }
    }
}