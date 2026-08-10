using Content;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.T70XWing
{
    public class JaycrisTubbsEoD : T70XWingEoD
    {
        public JaycrisTubbsEoD() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Jaycris Tubbs",
                "Evacuation of D'Qar",
                Faction.Resistance,
                1,
                10,
                0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.JaycrisTubbsAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                },
                tags: new List<Tags>
                {
                    Tags.XWing
                },
                legality: new List<Legality> { Legality.XWA },
                isStandardLayout: true
            );

            MustHaveUpgrades.Add(typeof(ForTheCause));
            MustHaveUpgrades.Add(typeof(R0Astromech));

            PilotNameCanonical = "jaycristubbs-evacuationofdqar";
        }
    }
}
