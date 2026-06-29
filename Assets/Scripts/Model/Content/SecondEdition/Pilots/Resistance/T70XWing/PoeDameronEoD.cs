using ActionsList;
using Content;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.T70XWing
{
    public class PoeDameronEoD : T70XWingEoD
    {
        public PoeDameronEoD() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Poe Dameron",
                "Evacuation of D'Qar",
                Faction.Resistance,
                6,
                16,
                0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.PoeDameronAbility),
                charges: 1,
                regensCharges: 1,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                    UpgradeType.Tech,
                    UpgradeType.Title
                },
                tags: new List<Tags>
                {
                    Tags.XWing
                },
                skinName: "Black One",
                legality: new List<Legality> { Legality.XWA },
                isStandardLayout: true
            );

            ShipInfo.ActionIcons.RemoveLinkedAction(typeof(BoostAction), typeof(FocusAction));

            MustHaveUpgrades.Add(typeof(Heroic));
            MustHaveUpgrades.Add(typeof(BB8EoD));
            MustHaveUpgrades.Add(typeof(PrimedOverdriveThruster));
            MustHaveUpgrades.Add(typeof(BlackOneEoD));

            PilotNameCanonical = "poedameron-evacuationofdqar";
        }
    }
}