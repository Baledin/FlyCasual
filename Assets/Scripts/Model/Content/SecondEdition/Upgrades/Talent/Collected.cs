using Ship;
using SubPhases;
using System;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Collected : GenericUpgrade
    {
        public Collected() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Collected",
                UpgradeType.Talent,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.CollectedAbility)
            );

            IsHidden = true;

            ImageUrl = "Pilots/lieutenanthebsly-battleoverendor";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CollectedAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (HostShip.Tokens.CountTokensByType<Tokens.FocusToken>() == 0) return;

            if (Combat.ChosenWeapon.WeaponType != WeaponTypes.PrimaryWeapon) return;

            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskUseCollectedAbility);
        }

        private void AskUseCollectedAbility(object sender, EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    AlwaysUseByDefault,
                    UseAbilityDecision,
                    descriptionLong: "Do you want to spend 1 focus token to gain 2 evade tokens?",
                    showAlwaysUseOption: true,
                    imageHolder: HostUpgrade
                );
            }
            else
            {
                SpendToken();
                Triggers.FinishTrigger();
            }
        }

        private void UseAbilityDecision(object sender, EventArgs e)
        {
            if (HostShip.Tokens.CountTokensByType<Tokens.FocusToken>() > 0)
            {
                SpendToken();
            }
            DecisionSubPhase.ConfirmDecision();
        }

        private void SpendToken()
        {
            HostShip.Tokens.SpendToken(typeof(FocusToken), delegate { });
            HostShip.Tokens.AssignTokens(CreateEvadeToken, 2, delegate { });
        }

        private GenericToken CreateEvadeToken()
        {
            return new EvadeToken(HostShip);
        }
    }
}