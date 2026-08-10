using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.BTANR2WYWing
{
    public class LegaFossang : BTANR2WYWing
    {
        public LegaFossang() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Lega Fossang",
                "Hero of Humbarine",
                Faction.Resistance,
                3,
                10,
                10,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.LegaFossangAbility),
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

            PilotNameCanonical = "legafossang-wartime";
        }
    }
}