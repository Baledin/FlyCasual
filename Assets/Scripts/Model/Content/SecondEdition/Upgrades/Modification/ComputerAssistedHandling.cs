using ActionsList;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ComputerAssistedHandling : GenericUpgrade
    {
        public ComputerAssistedHandling() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Computer-Assisted Handling",
                UpgradeType.Modification,
                cost: 0,
                charges: 1,
                abilityType: typeof(Abilities.SecondEdition.ComputerAssistedHandlingAbility)
            );

            IsHidden = true;

            ImageUrl = "Pilots/captainyorr-battleoverendor";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a maneuver, you may spend 1 charge to perform a boost or barrel-roll action.
    public class ComputerAssistedHandlingAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            //AI doesn't use ability
            if (HostShip.Owner.UsesHotacAiRules) return;

            if (!HostShip.IsBumped && HostUpgrade.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            HostShip.BeforeActionIsPerformed += RegisterSpendChargeTrigger;
            HostShip.AskPerformFreeAction(
               new List<GenericAction>()
                    {
                        new BoostAction(),
                        new BarrelRollAction()
                    },
                CleanUp,
                HostUpgrade.UpgradeInfo.Name,
                "After you fully execute a maneuver, you may spend 1 charge to perform a boost or barrel-roll action.",
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

