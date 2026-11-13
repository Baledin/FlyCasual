using Abilities.SecondEdition;
using ActionsList;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class R2A3BoE : GenericUpgrade
    {
        public R2A3BoE() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "R2-A3",
                UpgradeType.Astromech,
                cost: 0,
                isLimited: true,
                abilityType: typeof(R2A3BoEAbility),
                charges: 1
            );

            NameCanonical = "r2a3-battleoverendor";

            ImageUrl = "Pilots/wedgeantilles-battleoverendor";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform an action, you may spend 1 charge to acquire a lock
    public class R2A3BoEAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
        }

        private void CheckConditions(GenericAction action)
        {
            if (HostUpgrade.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskAcquireLock);
            }
        }

        private void AskAcquireLock(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                AcquireLock,
                descriptionLong: "Do you want to spend one charge to acquire a lock?",
                imageHolder: HostUpgrade
            );
        }

        private void AcquireLock(object sender, System.EventArgs e)
        {
            HostUpgrade.State.SpendCharge();

            HostShip.ChooseTargetToAcquireTargetLock(
                SubPhases.DecisionSubPhase.ConfirmDecision,
                "You may acquire a lock",
                HostUpgrade
            );
        }
    }
}