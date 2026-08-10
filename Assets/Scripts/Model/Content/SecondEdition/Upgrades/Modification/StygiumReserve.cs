using Actions;
using ActionsList;
using Ship;
using System;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class StygiumReserve : GenericUpgrade
    {
        public StygiumReserve()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Stygium Reserve",
                UpgradeType.Modification,
                abilityType: typeof(Abilities.SecondEdition.StygiumReserveAbility),
                charges: 1
            );

            IsHidden = true;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class StygiumReserveAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += CheckAbility;
        }
        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (HostShip.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToUseAbility);
            }
        }

        private void AskToUseAbility(object sender, EventArgs e)
        {
            HostShip.BeforeActionIsPerformed += RegisterSpendChargeTrigger;
            CameraScript.RestoreCamera();

            HostShip.AskPerformFreeAction(
                new BoostAction() { CanBePerformedWhileStressed = true, Color = ActionColor.White },
                CleanUp,
                "Stygium Reserve",
                "After you fully execute a maneuver, you may spend a charge to perform a Boost",
                HostShip
            );
        }

        private void RegisterSpendChargeTrigger(GenericAction action, ref bool isFreeAction)
        {
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