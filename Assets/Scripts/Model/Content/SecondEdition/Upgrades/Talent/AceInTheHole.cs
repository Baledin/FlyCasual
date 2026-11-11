using ActionsList;
using System;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class AceInTheHole : GenericUpgrade
    {
        public AceInTheHole() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "Ace In The Hole",
                UpgradeType.Talent,
                cost: 0,
                charges: 2,
                abilityType: typeof(Abilities.SecondEdition.AceInTheHoleAbility)
            );

            IsHidden = true;

            ImageUrl = "Pilots/landocalrissian-battleoverendor";
        }
    }
}

namespace Abilities.SecondEdition
{
    //At the start of the Engagement Phase, you may spend 1 charge and gain 1 jam token to perform a barrel roll action.
    public class AceInTheHoleAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbilityTrigger;
        }

        private void RegisterAbilityTrigger()
        {
            if (HostUpgrade.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            if (HostShip.CanPerformFreeAction(new BarrelRollAction()))
            {
                Selection.ChangeActiveShip(HostShip);

                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    NeverUseByDefault,
                    UseAceInTheHoleAbility,
                    DontUseAceIntheHoleAbility,
                    descriptionLong: "At the start of the Engagement Phase, you may spend 1 charge and gain 1 jam token to perform a barrel roll action.",
                    imageHolder: HostUpgrade,
                    showSkipButton: true,
                    requiredPlayer: HostShip.Owner.PlayerNo
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void DontUseAceIntheHoleAbility(object sender, System.EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();
            Triggers.FinishTrigger();
        }

        private void UseAceInTheHoleAbility(object sender, System.EventArgs e)
        {

            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            HostUpgrade.State.SpendCharge();
            HostShip.Tokens.AssignToken(new JamToken(HostShip, HostShip.Owner), PerformBarrelRollAction);
        }

        private void PerformBarrelRollAction()
        {
            HostShip.AskPerformFreeAction(
                new BarrelRollAction(),
                Triggers.FinishTrigger,
                HostUpgrade.UpgradeInfo.Name
            );
        }
    }


}