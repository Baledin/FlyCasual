﻿using Upgrade;
using Ship;
using System.Collections.Generic;
using System;
using ActionsList;

namespace UpgradesList.SecondEdition
{
    public class AfterBurners : GenericUpgrade, IVariableCost
    {
        public AfterBurners() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "AfterBurners",
                UpgradeType.Modification,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.AfterBurnersAbility),
                charges: 2,
                restriction: new BaseSizeRestriction(BaseSize.Small)
            );
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> initiativeToCost = new Dictionary<int, int>()
            {
                {0, 4},
                {1, 4},
                {2, 4},
                {3, 4},
                {4, 5},
                {5, 6},
                {6, 7}
            };

            UpgradeInfo.Cost = initiativeToCost[ship.PilotInfo.Initiative];
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a speed 3-5 maneuver you may spend 1 charge to perform a boost action, even while stressed.
    public class AfterBurnersAbility : GenericAbility
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

            if (HostShip.AssignedManeuver.Speed >= 3 && HostShip.AssignedManeuver.Speed <= 5 && !HostShip.IsBumped && HostUpgrade.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            HostShip.BeforeActionIsPerformed += RegisterSpendChargeTrigger;
            HostShip.AskPerformFreeAction(
                new BoostAction() { CanBePerformedWhileStressed = true },
                CleanUp,
                HostUpgrade.UpgradeInfo.Name,
                "After you fully execute a speed 3-5 maneuver you may spend 1 Charge to perform a Boost action, even while stressed.",
                HostUpgrade
            );
        }

        private void RegisterSpendChargeTrigger(GenericAction action, ref bool isFreeAction)
        {
            HostShip.BeforeActionIsPerformed -= RegisterSpendChargeTrigger;
            RegisterAbilityTrigger(
                TriggerTypes.OnFreeAction,
                delegate {
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