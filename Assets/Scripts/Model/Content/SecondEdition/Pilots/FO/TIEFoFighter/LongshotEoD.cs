using Abilities.SecondEdition;
using Content;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.TIEFoFighter
{
    public class LongshotEoD : TIEFoFighter
    {
        public LongshotEoD() : base()
        {
            PilotInfo = new PilotCardInfo25(
                pilotName: "\"Longshot\"",
                pilotTitle: "Evacuation of D'Qar",
                faction: Faction.FirstOrder,
                initiative: 3,
                cost: 8,
                loadoutValue: 0,
                isLimited: true,
                limited: 1,
                abilityType: typeof(LongshotAbility),
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

            PilotNameCanonical = "longshot-evacuationofdqar";

            ShipAbilities.Add(new Merciless());

            MustHaveUpgrades.Add(typeof(Determination));
            MustHaveUpgrades.Add(typeof(Fanatical));
            MustHaveUpgrades.Add(typeof(ExperimentalScanners));
        }
    }
}