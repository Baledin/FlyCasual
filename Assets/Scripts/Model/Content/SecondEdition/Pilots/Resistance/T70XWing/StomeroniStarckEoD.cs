using Content;
using Ship.SecondEdition.RZ2AWing;
using Ship.SecondEdition.T70XWing;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.T70XWing
{
    public class StomeroniStarckEoD : T70XWingEoD
    {
        public StomeroniStarckEoD() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Stomeroni Starck",
                "Evacuation of D'Qar",
                Faction.Resistance,
                4,
                13,
                0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.StomeroniStarckT70Ability),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Tech,
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
            MustHaveUpgrades.Add(typeof(AcceleratedSensorArray));
            MustHaveUpgrades.Add(typeof(R5X3XWA));

            PilotNameCanonical = "stomeronistarck-evacuationofdqar";
        }
    }
}

namespace Abilities.SecondEdition
{
    // Setup: Instead of a T-70 X-wing dial, this ship uses an RZ-2 A-wing dial.
    public class StomeroniStarckT70Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.DialInfo = new RZ2AWing().DialInfo;
        }

        public override void DeactivateAbility()
        {
            HostShip.DialInfo = new T70XWing().DialInfo;
        }
    }
}