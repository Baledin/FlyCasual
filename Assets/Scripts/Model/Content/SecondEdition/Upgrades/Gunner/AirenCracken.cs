using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class AirenCracken : GenericUpgrade
    {
        public AirenCracken() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "Airen Cracken",
                types: new List<UpgradeType>() { UpgradeType.Gunner },
                cost: 0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.AirenCrackenGunnerAbility)
            );

            NameCanonical = "airencracken-boe";
            ImageUrl = "Pilots/landocalrissian-battleoverendor";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After defending, if the attack hit, you may gain 1 deplete token to perform a bonus primary attack targeting the attacker.
    public class AirenCrackenGunnerAbility : GenericAbility
    {
        private bool IsPerformedRegularAttack;
        private GenericShip ShipToPunish;
        public override void ActivateAbility()
        {
            HostShip.OnAttackHitAsDefender += PlanBonusAttack;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackHitAsDefender -= PlanBonusAttack;
            HostShip.OnAttackFinishAsDefender -= AddBonusAttackAbility;
        }

        private void PlanBonusAttack()
        {
            HostShip.OnAttackFinishAsDefender += AddBonusAttackAbility;
        }

        private void AddBonusAttackAbility(GenericShip ship)
        {
            HostShip.OnAttackFinishAsDefender -= AddBonusAttackAbility;
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskBonusAttack);
        }

        private void AskBonusAttack(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                ConfirmExtraAttackPunish,
                descriptionLong: "Do you want to gain 1 deplete token to perform a bonus primary attack against the attacker?",
                imageHolder: HostReal as IImageHolder,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void ConfirmExtraAttackPunish(object sender, EventArgs e)
        {
            if (IsAbilityUsed) return;

            if (HostShip.IsCannotAttackSecondTime) return;

            HostShip.Tokens.AssignToken(typeof(DepleteToken), delegate { });

            // Save his attacker, becuase combat data will be cleared
            ShipToPunish = Combat.Attacker;

            Combat.Attacker.OnCombatCheckExtraAttack += RegisterExtraAttackAbility;

            DecisionSubPhase.ConfirmDecision();
        }

        private void RegisterExtraAttackAbility(GenericShip ship)
        {
            ship.OnCombatCheckExtraAttack -= RegisterExtraAttackAbility;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, DoCounterAttack);
        }

        private void DoCounterAttack(object sender, EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                // Save his "is already attacked" flag
                IsPerformedRegularAttack = HostShip.IsAttackPerformed;

                // Plan to set IsAbilityUsed only after attack that was successfully started
                HostShip.OnAttackStartAsAttacker += MarkAbilityAsUsed;

                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartSelectAttackTarget(
                    HostShip,
                    FinishExtraAttack,
                    CounterAttackFilter,
                    HostUpgrade.UpgradeInfo.Name,
                    "You may perform an additional attack against " + ShipToPunish.PilotInfo.PilotName,
                    HostReal as IImageHolder
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack an additional time", HostShip.PilotInfo.PilotName));
                Triggers.FinishTrigger();
            }
        }

        protected virtual void MarkAbilityAsUsed()
        {
            IsAbilityUsed = true;
        }
        private void FinishExtraAttack()
        {
            // Restore previous value of "is already attacked" flag
            HostShip.IsAttackPerformed = IsPerformedRegularAttack;

            // Set IsAbilityUsed only after attack that was successfully started
            HostShip.OnAttackStartAsAttacker -= MarkAbilityAsUsed;

            //if bonus attack was skipped, allow bonus attacks again
            if (HostShip.IsAttackSkipped) HostShip.IsCannotAttackSecondTime = false;

            Triggers.FinishTrigger();
        }

        private bool CounterAttackFilter(GenericShip targetShip, IShipWeapon weapon, bool isSilent)
        {
            bool result = true;

            if (targetShip != ShipToPunish)
            {
                if (!isSilent) Messages.ShowErrorToHuman(string.Format("{0} can only attack {1}", HostShip.PilotInfo.PilotName, ShipToPunish.PilotInfo.PilotName));
                result = false;
            }

            if (weapon.WeaponType != WeaponTypes.PrimaryWeapon)
            {
                if (!isSilent) Messages.ShowError("Your bonus attack must be a primary weapon attack");
                return false;
            }

            return result;
        }
    }
}