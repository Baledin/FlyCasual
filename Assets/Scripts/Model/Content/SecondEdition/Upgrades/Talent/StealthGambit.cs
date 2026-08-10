using Arcs;
using Ship;
using SubPhases;
using System;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class StealthGambit : GenericUpgrade
    {
        public StealthGambit() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Stealth Gambit",
                UpgradeType.Talent,
                abilityType: typeof(Abilities.SecondEdition.StealthGambitAbility)
            );
            IsHidden = true;
        }
    }
}

namespace Abilities.SecondEdition
{
    // After a friendly ship performs an attack that hits an enemy ship in your front arc, if you are cloaked, you may gain a strain token to remove your cloak token and perform a bonus primary attack targeting the defender.
    // At the end of the Engagement Phase, if you are strained, gain an evade token.

    public class StealthGambitAbility : GenericAbility
    {
        GenericShip defender;

        public bool PerformedRegularAttack { get; set; }

        public override void ActivateAbility()
        {
            GenericShip.OnAttackHitAsAttackerGlobal += RegisterAbility;
            Phases.Events.OnCombatPhaseEnd_NoTriggers += GainEvade;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackHitAsAttackerGlobal -= RegisterAbility;
            Phases.Events.OnCombatPhaseEnd_NoTriggers -= GainEvade;
        }

        private void GainEvade()
        {
            if (HostShip.IsStrained)
            {
                HostShip.Tokens.AssignToken(typeof(EvadeToken), delegate { });
            }
        }

        private void RegisterAbility()
        {
            if (Tools.IsFriendly(Combat.Attacker, HostShip) && BoardTools.Board.IsShipInArcByType(HostShip, Combat.Defender, ArcType.Front) && HostShip.IsCloaked)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackHit, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            AskToUseAbility(HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                descriptionLong: "Gain 1 strain token to remove your cloak token and perform a bonus primary attack targeting the defender?",
                useAbility: UseStealthGambit,
                callback: Triggers.FinishTrigger
            );
        }

        private void UseStealthGambit(object sender, EventArgs e)
        {
            defender = Combat.Defender;

            PerformedRegularAttack = HostShip.IsAttackPerformed;

            HostShip.OnCombatCheckExtraAttack += StartBonusAttack;

            HostShip.Tokens.AssignToken(new StrainToken(HostShip), delegate
            {
                HostShip.Tokens.RemoveToken(typeof(CloakToken), delegate
                {
                    DecisionSubPhase.ConfirmDecision();
                });
            });
        }

        private void StartBonusAttack(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= StartBonusAttack;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, RegisterBonusAttack);
        }

        private void RegisterBonusAttack(object sender, System.EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartSelectAttackTarget(
                    HostShip,
                    FinishBonusAttack,
                    BonusPrimaryAttackFilter,
                    HostShip.PilotInfo.PilotName,
                    "Select a target for bonus primary attack",
                    HostShip
                );
            }
            else
            {
                Messages.ShowErrorToHuman($"{HostShip.PilotInfo.PilotName} cannot perform second bonus attack");
                FinishBonusAttack();
            }
        }

        private bool BonusPrimaryAttackFilter(GenericShip ship, IShipWeapon weapon, bool isSilent)
        {
            return ship == defender && weapon is PrimaryWeaponClass;
        }

        private void FinishBonusAttack()
        {
            // Restore previous value of "is already attacked" flag
            HostShip.IsAttackPerformed = PerformedRegularAttack;

            //if bonus attack was skipped, allow bonus attacks again
            if (HostShip.IsAttackSkipped) HostShip.IsCannotAttackSecondTime = false;

            Triggers.FinishTrigger();
        }
    }
}