using Content;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class TargetingRelay : GenericUpgrade
    {
        public TargetingRelay() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Targeting Relay",
                UpgradeType.Tech,
                cost: 3,
                charges: 2,
                abilityType: typeof(Abilities.SecondEdition.TargetingRelayAbility),
                legalityInfo: new List<Legality>() { Legality.XWA }
            );

            NameCanonical = "targetingrelay-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TargetingRelayAbility : GenericAbility
    {
        // After a friendly ship acquires a lock on an enemy ship at range 0-1 of you, you may spend 1 charge and gain a stress token. If you do, that friendly ship may gain a calculate token
        GenericShip friendlyShip;

        public override void ActivateAbility()
        {
            GenericShip.OnTargetLockIsAcquiredGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTargetLockIsAcquiredGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, ITargetLockable target)
        {
            if (HostUpgrade.State.Charges > 0
                && Tools.IsFriendly(HostShip, ship)
                && target is GenericShip targetShip
                && HostShip.GetRangeToShip(targetShip) < 2)
            {
                friendlyShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnTargetLockIsAcquired, AskToUseAbility);
            }
        }

        private void AskToUseAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                CheckConditionsForAIUse,
                useAbility: UseAbility,
                descriptionLong: $"Do you want to spend 1 charge and gain a stress token to allow {friendlyShip.PilotInfo.PilotName} to gain a calculate token?",
                imageHolder: HostUpgrade,
                callback: Triggers.FinishTrigger
            );
        }

        private bool CheckConditionsForAIUse()
        {
            return !HostShip.IsStressed && !friendlyShip.Tokens.HasTokenByColor(TokenColors.Green);
        }

        private void UseAbility(object sender, EventArgs e)
        {
            HostUpgrade.State.SpendCharge();

            HostShip.Tokens.AssignToken(typeof(StressToken), delegate
            {
                friendlyShip.Tokens.AssignToken(typeof(CalculateToken), delegate
                {
                    SubPhases.DecisionSubPhase.ConfirmDecision();
                });
            });
        }
    }
}