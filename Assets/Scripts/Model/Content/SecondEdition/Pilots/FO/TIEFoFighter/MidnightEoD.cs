using Abilities.SecondEdition;
using Content;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.TIEFoFighter
{
    public class MidnightEoD : TIEFoFighter
    {
        public MidnightEoD() : base()
        {
            PilotInfo = new PilotCardInfo25(
                pilotName: "\"Midnight\"",
                pilotTitle: "Evacuation of D'Qar",
                faction: Faction.FirstOrder,
                initiative: 6,
                cost: 9,
                loadoutValue: 0,
                isLimited: true,
                limited: 1,
                abilityType: typeof(MidnightAbility),
                extraUpgradeIcons: new()
                {
                    UpgradeType.Talent,
                    UpgradeType.Tech,
                    UpgradeType.Modification
                },
                tags: new()
                {
                    Tags.Tie
                },
                isStandardLayout: true,
                legality: new List<Legality>() { Legality.XWA }
            );

            PilotNameCanonical = "midnight-evacuationofdqar";

            ShipAbilities.Add(new Merciless());

            MustHaveUpgrades.Add(typeof(Determination));
            MustHaveUpgrades.Add(typeof(TargetingSynchronizer));
            MustHaveUpgrades.Add(typeof(AdvancedWarningSystems));
        }
    }
}