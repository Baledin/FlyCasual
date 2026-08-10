using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.BTANR2WYWing
{
    public class WilsaTeshlo : BTANR2WYWing
    {
        public WilsaTeshlo() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Wilsa Teshlo",
                "Veiled Sorority Privateer",
                Faction.Resistance,
                4,
                11,
                17,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.WilsaTeshloAbility),
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
                skinName: "Orange",
                legality: new List<Legality> { Legality.XWA }
            );

            PilotNameCanonical = "wilsateshlo-wartime";
        }
    }
}