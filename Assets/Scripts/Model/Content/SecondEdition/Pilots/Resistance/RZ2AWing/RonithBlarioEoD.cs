using Content;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.RZ2AWing
{
    public class RonithBlarioEoD : RZ2AWing
    {
        public RonithBlarioEoD() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Ronith Blario",
                "Evacuation of D'Qar",
                Faction.Resistance,
                2,
                9,
                0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.RonithBlarioAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Talent
                },
                tags: new List<Tags>
                {
                    Tags.AWing
                },
                skinName: "Red",
                legality: new List<Legality> { Legality.XWA },
                isStandardLayout: true
            );

            MustHaveUpgrades.Add(typeof(EscortFighter));
            MustHaveUpgrades.Add(typeof(StarbirdSlash));
            MustHaveUpgrades.Add(typeof(Heroic));

            PilotNameCanonical = "ronithblario-evacuationofdqar";
        }
    }
}