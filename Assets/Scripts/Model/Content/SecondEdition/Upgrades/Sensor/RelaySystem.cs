using ActionsList;
using BoardTools;
using Obstacles;
using Ship;
using SubPhases;
using System;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class RelaySystem : GenericUpgrade
    {
        public RelaySystem()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Relay System",
                UpgradeType.Sensor,
                abilityType: typeof(Abilities.SecondEdition.RelaySystemAbility)
            );

            IsHidden = true;
        }
    }
}

namespace Abilities.SecondEdition
{
    // After a friendly ship at range 0-2 performs a target lock action, you may acquire a lock on the same object.
    // After you perform an attack that hits, you may spend a lock you have on the defender.
    // If you do, another friendly ship at range 0-1 may acquire a lock on the defender.
    public class RelaySystemAbility : GenericAbility
    {
        GenericShip friendlyShip;

        public override void ActivateAbility()
        {
            GenericShip.OnActionIsPerformedGlobal += RegisterTargetLockActionCheck;

            HostShip.OnAttackHitAsAttacker += CheckPassOnTargetLockAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnActionIsPerformedGlobal -= RegisterTargetLockActionCheck;

            HostShip.OnAttackHitAsAttacker -= CheckPassOnTargetLockAbility;
        }

        private void RegisterTargetLockActionCheck(GenericAction action)
        {
            if ((action is TargetLockAction) &&
                Tools.IsFriendly(HostShip, action.HostShip) &&
                (HostShip.GetRangeToShip(action.HostShip) < 3))
            {
                friendlyShip = action.HostShip;

                RegisterAbilityTrigger(TriggerTypes.OnTargetLockIsAcquired, AskAcquireTargetLock);
            }
        }

        private void AskAcquireTargetLock(object sender, EventArgs e)
        {
            // Last lock should be the latest
            ITargetLockable lockedObject = friendlyShip.Tokens.GetTokens<BlueTargetLockToken>('*').Last().OtherTargetLockTokenOwner;

            string objectName = null;

            if (lockedObject is GenericShip)
            {
                objectName = (lockedObject as GenericShip).PilotInfo.PilotName;

                if (HostShip.GetRangeToShip((lockedObject as GenericShip)) > HostShip.TargetLockMaxRange)
                {
                    Triggers.FinishTrigger();
                    return;
                }
            }
            else if (lockedObject is GenericObstacle)
            {
                objectName = (lockedObject as GenericObstacle).Name;

                if (new ShipObstacleDistance(HostShip, (lockedObject as GenericObstacle)).Range > HostShip.TargetLockMaxRange)
                {
                    Triggers.FinishTrigger();
                    return;
                }
            }

            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                NeedsTargetlock,
                delegate
                {
                    ActionsHolder.AcquireTargetLock(HostShip, lockedObject, DecisionSubPhase.ConfirmDecision, DecisionSubPhase.ConfirmDecision);
                },
                descriptionLong: $"Do you want to acquire a lock on {objectName}?",
                imageHolder: HostShip,
                callback: Triggers.FinishTrigger
            );
        }

        private bool NeedsTargetlock()
        {
            return !HostShip.Tokens.HasToken<BlueTargetLockToken>('*');
        }

        private void CheckPassOnTargetLockAbility()
        {
            if (!HostShip.GetTargetLockLetterPairsOn(Combat.Defender).Any()) return;

            if (HostShip.Owner.Ships.Values.Any(s => FilterAbilityTargets(s)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackHit, PassTargetLock);
            }
        }

        private void PassTargetLock(Object sender, EventArgs e)
        {
            SelectTargetForAbility(
                TransferTargetLock,
                FilterAbilityTargets,
                GetAiPriorityForTargetLockTransfer,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                description: "You may spend a lock to allow a friendly ship at range 0-1 to acquire a lock on the defender.",
                showSkipButton: true,
                callback: Triggers.FinishTrigger
            );
        }

        private void TransferTargetLock()
        {
            HostShip.Tokens.SpendToken(
                HostShip.Tokens.GetTargetLockToken(HostShip.GetTargetLockLetterPairsOn(Combat.Defender).First()),
                delegate
                {
                    ActionsHolder.AcquireTargetLock(TargetShip, Combat.Defender, DecisionSubPhase.ConfirmDecision, DecisionSubPhase.ConfirmDecision);
                }
            );
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            return Tools.IsAnotherFriendly(HostShip, ship) &&
                    HostShip.GetRangeToShip(ship) < 2 &&
                    ship.GetRangeToShip(Combat.Defender) <= ship.TargetLockMaxRange;
        }

        private int GetAiPriorityForTargetLockTransfer(GenericShip ship)
        {
            int priority = 0;

            // Prefer ships that haven't attacked yet
            if (ship.HasCombatActivation) priority += 100;

            // Prefer ships that don't already have a target lock on the same target
            foreach (BlueTargetLockToken blueLock in ship.Tokens.GetTokens<BlueTargetLockToken>('*'))
            {
                if (blueLock.OtherTargetLockTokenOwner == Combat.Defender) priority -= 50;
            }

            // Prefer ships with lower pilot skill (assuming they benefit more from the target lock)
            priority += (10 - ship.PilotInfo.Initiative);

            return priority;
        }
    }
}