using ActionsList;
using Bombs;
using Content;
using Ship;
using SubPhases;
using System;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class PaigeTicoEoD : GenericUpgrade
    {
        public PaigeTicoEoD() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Paige Tico",
                UpgradeType.Gunner,
                cost: 0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.PaigeTicoEoDAbility),
                legalityInfo: new() { Legality.XWA }
            );

            IsHidden = true;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PaigeTicoEoDAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnWeaponsDisabledCheck += AllowTurretAttacks;
            HostShip.OnAttackFinishAsAttacker += RegisterDropOrRotateAbility;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnWeaponsDisabledCheck -= AllowTurretAttacks;
            HostShip.OnAttackFinishAsAttacker -= RegisterDropOrRotateAbility;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void RegisterDropOrRotateAbility(GenericShip ship)
        {
            if (!IsAbilityUsed)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskDropOrRotate);
            }
        }

        private void AllowTurretAttacks(ref bool result)
        {
            if(HostShip.Tokens.GetTokens<WeaponsDisabledToken>().Count == 1)
            {
                result = false;
                HostShip.OnCheckIsForbiddenWeapon += AllowTurretsOnly;
                Phases.Events.OnCombatPhaseEnd_NoTriggers += ClearWeaponLocks;
            }
        }

        private void AllowTurretsOnly(GenericShip ship, IShipWeapon weapon, ref bool isAllowed)
        {
            // TODO: Confirm this works on PrimaryWeapon turrets as well
            isAllowed = weapon.WeaponType == WeaponTypes.Turret;
        }

        private void ClearWeaponLocks()
        {
            HostShip.OnCheckIsForbiddenWeapon -= AllowTurretsOnly;
            Phases.Events.OnCombatPhaseEnd_NoTriggers -= ClearWeaponLocks;
        }

        private void AskDropOrRotate(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            PageTicoAbilityEoDDecision subphase = Phases.StartTemporarySubPhaseNew<PageTicoAbilityEoDDecision>("Paige Tico's ability", Triggers.FinishTrigger);

            subphase.DescriptionShort = "Paige Tico";
            subphase.DescriptionLong = "You may drop a bomb or rotate arc";
            subphase.ImageSource = HostUpgrade;

            subphase.AddDecision("Drop a bomb", DropBomb);
            subphase.AddDecision("Rotate Arc", AskRotateArc);

            subphase.DecisionOwner = HostShip.Owner;
            subphase.DefaultDecisionName = "Rotate Arc";
            subphase.ShowSkipButton = true;

            subphase.Start();
        }

        private void AskRotateArc(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            IsAbilityUsed = true;

            new RotateArcAction().DoOnlyEffect(Triggers.FinishTrigger);
        }

        private bool HasBombsToDrop()
        {
            return HostShip.UpgradeBar.GetUpgradesAll().Any(n =>
                n is GenericBomb
                && (n as GenericBomb).UpgradeInfo.SubType == UpgradeSubType.Bomb
                && n.State.Charges > 0
            );
        }

        private void AskDropBomb(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                DropBomb,
                descriptionLong: "Do you want to drop a bomb?",
                imageHolder: HostUpgrade
            );
        }

        private void DropBomb(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            IsAbilityUsed = true;

            BombsManager.RegisterBombDropTriggerIfAvailable(
                HostShip,
                TriggerTypes.OnAbilityDirect,
                subType: UpgradeSubType.Bomb,
                onlyDrop: true,
                isRealDrop: false
            );

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Triggers.FinishTrigger);
        }

        private class PageTicoAbilityEoDDecision : DecisionSubPhase { };
    }
};