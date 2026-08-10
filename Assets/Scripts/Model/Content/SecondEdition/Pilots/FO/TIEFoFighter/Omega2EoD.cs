using Abilities.SecondEdition;
using Actions;
using ActionsList;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.TIEFoFighter
{
    public class Omega2EoD : TIEFoFighter
    {
        public Omega2EoD() : base()
        {
            PilotInfo = new PilotCardInfo25(
                pilotName: "Omega 2",
                pilotTitle: "Evacuation of D'Qar",
                faction: Faction.FirstOrder,
                initiative: 3,
                cost: 7,
                loadoutValue: 0,
                isLimited: true,
                limited: 1,
                abilityType: typeof(Omega2EoDAbility),
                extraUpgradeIcons: new()
                {
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

            PilotNameCanonical = "omega2-evacuationofdqar";

            ShipAbilities.Add(new Merciless());

            MustHaveUpgrades.Add(typeof(Determination));
            MustHaveUpgrades.Add(typeof(TargetingRelay));

            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(EvadeAction), ActionColor.Red));
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Omega2EoDAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            RulesList.TargetLocksRule.OnCheckTargetLockIsAllowed += RegisterTargetLockFilter;
        }

        public override void DeactivateAbility()
        {
            RulesList.TargetLocksRule.OnCheckTargetLockIsAllowed -= RegisterTargetLockFilter;
        }

        private void RegisterTargetLockFilter(ref bool result, GenericShip attacker, ITargetLockable defender)
        {
            result = Tools.IsAnotherFriendly(HostShip, attacker)
                && (attacker.PilotInfo as PilotCardInfo25).Tags.Contains(Tags.Tie)
                && defender.GetRangeToShip(HostShip) <= 3;
        }
    }
}