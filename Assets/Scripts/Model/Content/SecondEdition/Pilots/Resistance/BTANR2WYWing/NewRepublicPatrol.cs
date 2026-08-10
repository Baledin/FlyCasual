using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.BTANR2WYWing
{
    public class NewRepublicPatrol : BTANR2WYWing
    {
        public NewRepublicPatrol() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "New Republic Patrol",
                "",
                Faction.Resistance,
                3,
                10,
                11,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Astromech,
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
                skinName: "Blue",
                legality: new List<Legality> { Legality.XWA }
            );

            PilotNameCanonical = "newrepublicpatrol-wartime";
        }
    }
}