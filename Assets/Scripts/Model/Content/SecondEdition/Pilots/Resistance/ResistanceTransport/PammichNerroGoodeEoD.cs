using Abilities.SecondEdition;
using Content;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.ResistanceTransport
{
    public class PammichNerroGoodeEoD : ResistanceTransport
    {
        public PammichNerroGoodeEoD()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Pammich Nerro Goode",
                "Evacuation of D'Qar",
                Faction.Resistance,
                3,
                9,
                0,
                isLimited: true,
                abilityType: typeof(PammichNerroGoodeAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Crew,
                    UpgradeType.Astromech,
                },
                legality: new List<Legality> { Legality.XWA },
                isStandardLayout: true
            );

            PilotNameCanonical = "pammichnerrogoode-evacuationofdqar";

            ShipAbilities.Add(new LeaveNoOneBehind());

            MustHaveUpgrades.Add(typeof(ForTheCause));
            MustHaveUpgrades.Add(typeof(ROGR));
            MustHaveUpgrades.Add(typeof(BlackSquadronR4));
        }
    }
}