using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.BTANR2WYWing
{
    public class ShasaZaro : BTANR2WYWing
    {
        public ShasaZaro() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Shasa Zaro",
                "Artistic Ace",
                Faction.Resistance,
                3,
                11,
                15,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.ShasaZaroAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
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

            PilotNameCanonical = "shasazaro-wartime";
        }
    }
}