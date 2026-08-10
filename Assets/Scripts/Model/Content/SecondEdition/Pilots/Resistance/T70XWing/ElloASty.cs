using Content;
using Movement;
using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.T70XWing
{
    public class ElloAsty : T70XWing
    {
        public ElloAsty() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Ello Asty",
                "Born to Ill",
                Faction.Resistance,
                5,
                4,
                4,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.ElloAstyAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Tech,
                    UpgradeType.Astromech,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.XWing
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class ElloAstyXWA : ElloAsty
    {
        public ElloAstyXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 13;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 12;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Talent,
                UpgradeType.Astromech,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ElloAstyAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity += CheckElloAstyAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity -= CheckElloAstyAbility;
        }

        private void CheckElloAstyAbility(GenericShip ship, ref ManeuverHolder movement)
        {
            if (movement.Bearing == ManeuverBearing.TallonRoll)
            {
                if (HostShip.Tokens.CountTokensByType(typeof(StressToken)) <= 2)
                {
                    movement.ColorComplexity = MovementComplexity.Normal;
                }
            }
        }
    }
}