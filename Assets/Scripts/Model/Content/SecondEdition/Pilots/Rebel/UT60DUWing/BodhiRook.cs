using Abilities.SecondEdition;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.UT60DUWing
{
    public class BodhiRook : UT60DUWing
    {
        public BodhiRook() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Bodhi Rook",
                "Imperial Defector",
                Faction.Rebel,
                4,
                5,
                10,
                isLimited: true,
                abilityType: typeof(BodhiRookAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Crew,
                    UpgradeType.Crew,
                    UpgradeType.Sensor,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                seImageNumber: 54,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class BodhiRookXWA : BodhiRook
    {
        public BodhiRookXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 11;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 12;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Crew,
                UpgradeType.Crew,
                UpgradeType.Sensor,
                UpgradeType.Modification,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BodhiRookAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            RulesList.TargetLocksRule.OnCheckTargetLockIsAllowed += CanPerformTargetLock;
        }

        public override void DeactivateAbility()
        {
            RulesList.TargetLocksRule.OnCheckTargetLockIsAllowed -= CanPerformTargetLock;
        }

        private void CanPerformTargetLock(ref bool result, GenericShip ship, ITargetLockable defender)
        {
            if (ship.Owner.PlayerNo != HostShip.Owner.PlayerNo) return;

            foreach (GenericShip friendlyShip in HostShip.Owner.Ships.Values)
            {
                if (defender.GetRangeToShip(friendlyShip) < 4)
                {
                    result = true;
                    return;
                }
            }
        }
    }
}
