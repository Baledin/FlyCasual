using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.BTANR2WYWing
{
    public class ZoriiBliss : BTANR2WYWing
    {
        public ZoriiBliss() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Zorii Bliss",
                "Corsair of Kijimi",
                Faction.Resistance,
                5,
                12,
                20,
                isLimited: true,
                charges: 1,
                regensCharges: 1,
                abilityType: typeof(Abilities.SecondEdition.ZoriiBlissAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
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
            
            PilotNameCanonical = "zoriibliss-wartime";
        }
    }
}