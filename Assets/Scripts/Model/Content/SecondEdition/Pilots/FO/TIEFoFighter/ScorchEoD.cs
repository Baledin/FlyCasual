using Abilities.SecondEdition;
using Content;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.TIEFoFighter
{
    public class ScorchEoD : TIEFoFighter
    {
        public ScorchEoD() : base()
        {
            PilotInfo = new PilotCardInfo25(
                pilotName: "\"Scorch\"",
                pilotTitle: "Evacuation of D'Qar",
                faction: Faction.FirstOrder,
                initiative: 4,
                cost: 9,
                loadoutValue: 0,
                isLimited: true,
                limited: 1,
                abilityType: typeof(ScorchAbility),
                extraUpgradeIcons: new()
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Tech
                },
                tags: new()
                {
                    Tags.Tie
                },
                isStandardLayout: true,
                legality: new List<Legality>() { Legality.XWA }
            );

            PilotNameCanonical = "scorch-evacuationofdqar";

            ShipAbilities.Add(new Merciless());

            MustHaveUpgrades.Add(typeof(Determination));
            MustHaveUpgrades.Add(typeof(Fanatical));
            MustHaveUpgrades.Add(typeof(ThreatSensors));
        }
    }
}