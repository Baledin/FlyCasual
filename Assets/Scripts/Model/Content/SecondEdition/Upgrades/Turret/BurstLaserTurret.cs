using Abilities.SecondEdition;
using Actions;
using ActionsList;
using Arcs;
using Content;
using SubPhases;
using System;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class BurstLaserTurret : GenericSpecialWeapon
    {
        public BurstLaserTurret() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Burst Laser Turret",
                UpgradeType.Turret,
                cost: 5,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 2,
                    minRange: 1,
                    maxRange: 2,
                    arc: ArcType.SingleTurret,
                    charges: 2,
                    regensCharges: true
                ),
                abilityType: typeof(BurstLaserTurretAbility),
                addArc: new(ArcType.SingleTurret),
                addAction: new ActionInfo(typeof(RotateArcAction)),
                legalityInfo: new() { Legality.XWA }
            );

            NameCanonical = "burstlaserturret-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BurstLaserTurretAbility : GenericAbility
    {
        // Attack: Spend 1 charge. You may spend 1 charge to roll 1 additional attack die to a maximum of 3.
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            // One charge used on attack by default, check if charges still exist to add an extra die.
            if (HostUpgrade.State.Charges > 0 && Combat.ChosenWeapon == HostUpgrade && Combat.Attacker.GetNumberOfAttackDice() < 3)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskToUseAbility);
            }
        }

        private void AskToUseAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                useAbility: UseBurstLaserTurretAbility,
                descriptionLong: "Spend 1 charge to roll additional attack die?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo,
                callback: Triggers.FinishTrigger
            );
        }

        private void UseBurstLaserTurretAbility(object sender, EventArgs e)
        {
            HostUpgrade.State.SpendCharge();

            HostShip.AfterGotNumberOfAttackDice += AddAttackDice;

            DecisionSubPhase.ConfirmDecision();
        }

        private void AddAttackDice(ref int value)
        {
            value++;

            HostShip.AfterGotNumberOfAttackDice -= AddAttackDice;
        }
    }
}