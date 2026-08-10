using Content;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.DroidTriFighter
{
    public class Dis347SoC : DroidTriFighter
    {
        public Dis347SoC()
        {
            PilotInfo = new PilotCardInfo25
            (
                "DIS-347",
                "Siege of Coruscant",
                Faction.Separatists,
                3,
                3,
                0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.Dis347Ability),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                },
                isStandardLayout: true,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            MustHaveUpgrades.Add(typeof(Marksmanship));
            MustHaveUpgrades.Add(typeof(AfterBurners));
            MustHaveUpgrades.Add(typeof(ContingencyProtocol));

            PilotNameCanonical = "dis347-siegeofcoruscant";
        }
    }

    public class Dis347SoCXWA : Dis347SoC
    {
        public Dis347SoCXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 9;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}