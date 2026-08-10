using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class AutomatedLoaders : GenericUpgrade
    {
        public AutomatedLoaders() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Automated Loaders",
                UpgradeType.Modification,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.AutomatedLoadersAbility),
                charges: 1,
                legalityInfo: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            IsHidden = true;

            NameCanonical = "automatedloaders-legendsandrelics";
        }
    }

    public class AutomatedLoadersXwa : AutomatedLoaders
    {
        public AutomatedLoadersXwa() : base()
        {
            UpgradeInfo.Cost = 3;
            UpgradeInfo.LegalityInfo = new List<Legality> { Legality.XWA };
            UpgradeInfo.Restrictions.AddRestriction(new ActionBarRestriction(typeof(ReloadAction)));
            IsHidden = false;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AutomatedLoadersAbility : GenericAbility
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
            if (Combat.ShotInfo.Weapon.WeaponType != WeaponTypes.PrimaryWeapon) return;

            if (HostUpgrade.State.Charges > 0 && HostShip.UpgradeBar.GetRechargableUpgrades().Any())
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            HostShip.BeforeActionIsPerformed += RegisterSpendChargeTrigger;

            //TODO Would be neat if this could check whether any ordnance upgrades are currently reloadable


            HostShip.AskPerformFreeAction(
                new ReloadAction() { CanBePerformedWhileStressed = false },
                CleanUp,
                HostUpgrade.UpgradeInfo.Name,
                "After you perform a primary attack you may spend 1 Charge to perform a Reload action.",
                HostUpgrade
            );
        }

        private void RegisterSpendChargeTrigger(GenericAction action, ref bool isFreeAction)
        {
            HostShip.BeforeActionIsPerformed -= RegisterSpendChargeTrigger;
            RegisterAbilityTrigger(
                TriggerTypes.OnFreeAction,
                delegate
                {
                    HostUpgrade.State.SpendCharge();
                    Triggers.FinishTrigger();
                }
            );
        }

        private void CleanUp()
        {
            HostShip.BeforeActionIsPerformed -= RegisterSpendChargeTrigger;
            Triggers.FinishTrigger();
        }
    }
}