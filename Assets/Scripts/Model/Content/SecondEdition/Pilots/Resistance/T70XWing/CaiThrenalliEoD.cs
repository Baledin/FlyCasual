using Content;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.T70XWing
{
    public class CaiThrenalliEoD : T70XWingEoD
    {
        public CaiThrenalliEoD() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "C’ai Threnalli",
                "Evacuation of D'Qar",
                Faction.Resistance,
                4,
                11,
                0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.CaiThrenalliAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                    UpgradeType.Modification,
                },
                tags: new List<Tags>
                {
                    Tags.XWing
                },
                legality: new List<Legality> { Legality.XWA },
                isStandardLayout: true
            );

            MustHaveUpgrades.Add(typeof(ForTheCause));
            MustHaveUpgrades.Add(typeof(Heroic));
            MustHaveUpgrades.Add(typeof(BBAstromech));
            MustHaveUpgrades.Add(typeof(RepulsorliftEngines));

            PilotNameCanonical = "caithrenalli-evacuationofdqar";
        }
    }
}