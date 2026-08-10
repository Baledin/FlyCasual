using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.BTANR2WYWing
{
    public class TezaNasz : BTANR2WYWing
    {
        public TezaNasz() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Teza Nasz",
                "Old Soldier",
                Faction.Resistance,
                4,
                11,
                18,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.TezaNaszAbility),
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

            PilotNameCanonical = "tezanasz-wartime";
        }
    }
}