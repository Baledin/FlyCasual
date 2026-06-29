using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.BTANR2WYWing
{
    public class AftabAckbar : BTANR2WYWing
    {
        public AftabAckbar() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Aftab Ackbar",
                "\"Junior\"",
                Faction.Resistance,
                2,
                10,
                13,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.AftabAckbarAbility),
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
                skinName: "Red",
                legality: new List<Legality> { Legality.XWA }
            );

            PilotNameCanonical = "aftabackbar-wartime";
        }
    }
}