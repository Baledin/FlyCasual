using Content;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.RZ2AWing
{
    public class ZiziTloEoD : RZ2AWing
    {
        public ZiziTloEoD() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Zizi Tlo",
                "Evacuation of D'Qar",
                Faction.Resistance,
                5,
                11,
                0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.ZiziTloAbility),
                charges: 1,
                regensCharges: 1,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Tech,
                },
                tags: new List<Tags>
                {
                    Tags.AWing
                },
                skinName: "Red",
                legality: new List<Legality> { Legality.XWA },
                isStandardLayout: true
            );

            MustHaveUpgrades.Add(typeof(ForTheCause));
            MustHaveUpgrades.Add(typeof(Heroic));
            MustHaveUpgrades.Add(typeof(PrecisionHoloTargeter));

            PilotNameCanonical = "zizitlo-evacuationofdqar";
        }
    }
}