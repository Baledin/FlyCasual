using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.BTANR2WYWing
{
    public class CorusKapellim : BTANR2WYWing
    {
        public CorusKapellim() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Corus Kapellim",
                "\"Gentleman Flyer\"",
                Faction.Resistance,
                1,
                11,
                18,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.CorusKapellimAbility),
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
                skinName: "Blue",
                legality: new List<Legality> { Legality.XWA }
            );

            PilotNameCanonical = "coruskapellim-wartime";
        }
    }
}