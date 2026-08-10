using Actions;
using ActionsList;
using Content;
using Movement;
using System.Collections.Generic;
using System.Linq;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.RZ1AWing
{
    public class ArvelCrynydSSP : RZ1AWing
    {
        public ArvelCrynydSSP() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Arvel Crynyd",
                "Green Leader",
                Faction.Rebel,
                3,
                4,
                0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.ArvelCrynydAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                        UpgradeType.Talent,
                        UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                        Tags.AWing
                },
                isStandardLayout: true,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            MustHaveUpgrades.Add(typeof(Predator));
            MustHaveUpgrades.Add(typeof(AfterBurners));

            PilotNameCanonical = "arvelcrynyd-swz106";
        }
    }

    public class ArvelCrynydSSPXWA : ArvelCrynydSSP
    {
        public ArvelCrynydSSPXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 8;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}