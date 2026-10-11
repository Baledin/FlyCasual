using ActionsList;
using Content;
using Movement;
using Ship;
using SubPhases;
using System;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class AdaptablePowerSystems : GenericUpgrade
    {
        public AdaptablePowerSystems() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "Adaptable Power Systems",
                UpgradeType.Modification,
                charges: 2,
                abilityType: typeof(Abilities.SecondEdition.AdaptablePowerSystemsAbility),
                legalityInfo: new() { Legality.XWA }
            );
            IsHidden = true;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AdaptablePowerSystemsAbility : GenericAbility
    {
        //After you fully execute a red maneuver or perform a red action, you may spend 1 charge. If you do, you may gain 1 deplete token to remove 1 stress token.
        // 
        // Before you engage, you may spend 1 charge. If you do, you may gain 1 strain token to remove 1 deplete token.

        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbilityAction;
            HostShip.OnMovementFinishSuccessfully += CheckAbilityMovement;
            HostShip.OnCombatActivation += CheckAbilityAttack;
        }
        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbilityAction;
            HostShip.OnMovementFinishSuccessfully -= CheckAbilityMovement;
            HostShip.OnCombatActivation -= CheckAbilityAttack;
        }

        private void CheckAbilityAction(GenericAction action)
        {
            if (HostUpgrade.State.Charges > 0 && action.IsRed)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Adaptable Power Systems",
                    TriggerType = TriggerTypes.OnActionIsPerformed,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    EventHandler = UseAdaptablePowerSystemsToRemoveStressToken
                });
            }
        }

        private void CheckAbilityMovement(GenericShip ship)
        {
            if (HostUpgrade.State.Charges > 0 && ship.AssignedManeuver.ColorComplexity == MovementComplexity.Complex)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Adaptable Power Systems",
                    TriggerType = TriggerTypes.OnMovementFinish,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    EventHandler = UseAdaptablePowerSystemsToRemoveStressToken
                });
            }
        }

        private void UseAdaptablePowerSystemsToRemoveStressToken(object sender, EventArgs e)
        {
            AskToUseAbility("Adaptable Power Systems",
                useByDefault: UseForManeuversButNotActions,
                useAbility: RemoveStressTokenAndAddDeplete,
                descriptionLong: "You may spend 1 charge to remove 1 Stress token and gain 1 Deplete token.",
                imageHolder: HostShip,
                callback: Triggers.FinishTrigger
            );
        }

        private bool UseForManeuversButNotActions()
        {
            return Phases.CurrentSubPhase.Name == "Activation Phase";
        }

        private void RemoveStressTokenAndAddDeplete(object sender, EventArgs e)
        {
            HostUpgrade.State.SpendCharge();
            HostShip.Tokens.RemoveToken(typeof(StressToken),
                delegate { HostShip.Tokens.AssignToken(new DepleteToken(HostShip), DecisionSubPhase.ConfirmDecision); });
        }

        private void CheckAbilityAttack(GenericShip ship)
        {
            if (HostUpgrade.State.Charges > 0 && HostShip.Tokens.HasToken<DepleteToken>())
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Adaptable Power Systems",
                    TriggerType = TriggerTypes.OnAttackStart,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    EventHandler = UseAdaptablePowerSystemsToRemoveDepleteToken
                });
            }
        }

        private void UseAdaptablePowerSystemsToRemoveDepleteToken(object sender, EventArgs e)
        {
            AskToUseAbility("Adaptable Power Systems",
                useByDefault: AlwaysUseByDefault,
                useAbility: RemoveDepleteTokenAndAddStrain,
                descriptionLong: "You may spend 1 charge to remove 1 Deplete token and gain 1 Strain token.",
                imageHolder: HostShip,
                callback: Triggers.FinishTrigger
            );
        }

        private void RemoveDepleteTokenAndAddStrain(object sender, EventArgs e)
        {
            HostUpgrade.State.SpendCharge();
            HostShip.Tokens.RemoveToken(typeof(DepleteToken),
                delegate { HostShip.Tokens.AssignToken(new StrainToken(HostShip), DecisionSubPhase.ConfirmDecision); });
        }
    }
}