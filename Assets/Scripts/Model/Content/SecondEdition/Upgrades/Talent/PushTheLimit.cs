using ActionsList;
using Ship;
using System;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class PushTheLimit : GenericUpgrade
    {
        public PushTheLimit() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Push The Limit",
                UpgradeType.Talent,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.PushTheLimitBoEAbility)
            );

            IsHidden = true;

            ImageUrl = "Pilots/coloneljendon-battleoverendor";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PushTheLimitBoEAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += CheckMovementAbility;
            HostShip.OnActionIsPerformed += CheckActionAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= CheckMovementAbility;
            HostShip.OnActionIsPerformed -= CheckActionAbility;
        }

        private void CheckMovementAbility(GenericShip ship)
        {
            if (ship.AssignedManeuver != null
                && ship.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Complex
                && ship.Tokens.CountTokensByType<Tokens.StrainToken>() == 0
            )
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToGetStrainToken);
            }
        }

        private void CheckActionAbility(GenericAction action)
        {
            if (action.IsRed
                && action.HostShip.Tokens.CountTokensByType<Tokens.StrainToken>() == 0
            )
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToGetStrainToken);
            }
        }

        private void AskToGetStrainToken(object sender, EventArgs e)
        {
            AskToUseAbility
            (
                HostShip.PilotInfo.PilotName,
                AlwaysUseByDefault,
                GetStrainTokenInstead,
                descriptionLong: "Do you want to gain 1 Strain token to remove 1 Stress token?",
                imageHolder: HostShip,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void GetStrainTokenInstead(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.AssignToken(typeof(Tokens.StrainToken), RemoveStressToken);
        }

        private void RemoveStressToken()
        {
            HostShip.Tokens.RemoveToken(typeof(Tokens.StressToken), Triggers.FinishTrigger);
        }
    }
}