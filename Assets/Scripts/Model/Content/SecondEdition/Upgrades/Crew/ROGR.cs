using System;
using Abilities.SecondEdition;
using ActionsList;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ROGR : GenericUpgrade
    {
        public ROGR() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "RO-GR",
                UpgradeType.Crew,
                abilityType: typeof(ROGRAbility)
            );

            IsHidden = true;

            ImageUrl = "https://infinitearenas.com/xw2xwa/images/pilots/pammichnerrogoode-evacuationofdqar.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    // While you have 2 or fewer stress tokens, you may perform Coordinate and Jam actions, even while stressed.
    // After you perform a Coordinate or Jam action, gain 1 calculate token.
    public class ROGRAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckCanPerformActionsWhileStressed += ConfirmThatItMayBePossible;
            HostShip.OnCanPerformActionWhileStressed += AllowIfConditions;
            HostShip.OnActionIsPerformed += CheckIfGainCalculate;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckCanPerformActionsWhileStressed -= ConfirmThatItMayBePossible;
            HostShip.OnCanPerformActionWhileStressed -= AllowIfConditions;
            HostShip.OnActionIsPerformed -= CheckIfGainCalculate;
        }

        private void ConfirmThatItMayBePossible(ref bool isAllowed)
        {
            isAllowed = HostShip.Tokens.CountTokensByType<StressToken>() <= 2;
        }

        private void AllowIfConditions(GenericAction action, ref bool isAllowed)
        {
            isAllowed = HostShip.Tokens.CountTokensByType<StressToken>() <= 2 && (action is JamAction || action is CoordinateAction);
        }

        private void CheckIfGainCalculate(GenericAction action)
        {
            if (action is JamAction || action is CoordinateAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, GainCalculate);
            }
        }

        private void GainCalculate(object sender, EventArgs e)
        {
            HostShip.Tokens.AssignToken(new CalculateToken(HostShip), Triggers.FinishTrigger);
        }
    }
}