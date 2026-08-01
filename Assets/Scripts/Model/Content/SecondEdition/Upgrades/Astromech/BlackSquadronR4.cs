using System;
using Abilities.SecondEdition;
using SubPhases;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class BlackSquadronR4 : GenericUpgrade
    {
        public BlackSquadronR4() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Black Squadron R4",
                UpgradeType.Astromech,
                charges: 2,
                abilityType: typeof(BlackSquadronR4Ability)
            );

            IsHidden = true;

            ImageUrl = "https://infinitearenas.com/xw2xwa/images/pilots/pammichnerrogoode-evacuationofdqar.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    // After you fully execute a blue maneuver, you may spend 1 Charge to remove all of your stress tokens.
    public class BlackSquadronR4Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += RegisterAskAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= RegisterAskAbilityTrigger;
        }

        private void RegisterAskAbilityTrigger(Ship.GenericShip ship)
        {
            if (HostShip.GetLastManeuverColor() == Movement.MovementComplexity.Easy && HostUpgrade.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskAbility);
            }
        }

        private void AskAbility(object sender, EventArgs e)
        {
            if (HostUpgrade.State.Charges > 0 && HostShip.Tokens.HasToken<StressToken>())
            {
                AskToUseAbility(
                    "Black Squadron R4",
                    NeverUseByDefault,
                    SpendChargeToRemoveStress,
                    callback: Triggers.FinishTrigger,
                    descriptionLong: "Do you want to spend 1 Charge to remove all your Stress Tokens?",
                    imageHolder: HostUpgrade
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void SpendChargeToRemoveStress(object sender, EventArgs e)
        {
            HostShip.Tokens.RemoveAllTokensByType(typeof(StressToken), SpendCharge);
        }

        private void SpendCharge()
        {
            HostUpgrade.State.SpendCharge();
            DecisionSubPhase.ConfirmDecision();
        }
    }
}