using ActionsList;
using Movement;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class IonManeuveringJet : GenericUpgrade
    {
        public IonManeuveringJet() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ion Maneuvering Jet",
                UpgradeType.Modification,
                cost: 0,
                charges: 2,
                abilityType: typeof(Abilities.SecondEdition.IonManeuveringJetAbility)
            );

            IsHidden = true;

            ImageUrl = "Pilots/scythe6-battleoverendor";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IonManeuveringJetAbility : GenericAbility
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
            if (ship.AssignedManeuver != null && ship.AssignedManeuver.Bearing == ManeuverBearing.KoiogranTurn)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, UseAbility);
            }
        }

        private void UseAbility(object sender, EventArgs e)
        {
            if (HostShip.AssignedManeuver.Bearing == ManeuverBearing.KoiogranTurn && HostUpgrade.State.Charges > 0)
            {
                HostShip.BeforeActionIsPerformed += SpendCharge;

                HostShip.OnCanPerformActionWhileStressed += TemporaryAllowAnyActionsWhileStressed;
                HostShip.OnCheckCanPerformActionsWhileStressed += TemporaryAllowActionsWhileStressed;
                HostShip.OnActionIsPerformed += DisallowActionsWhileStressed;
                HostShip.OnActionIsSkipped += DisallowActionsWhileStressedAlt;

                List<GenericAction> actions = HostShip.GetAvailableActions();

                HostShip.AskPerformFreeAction(
                    actions,
                    delegate
                    {
                        Triggers.FinishTrigger();
                    },
                    HostUpgrade.UpgradeInfo.Name,
                    "After you fully execute a Koiogran Turn, you may spend 1 Charge to perform an action, even while stressed",
                    HostUpgrade
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void SpendCharge(GenericAction action, ref bool isFreeAction)
        {
            HostUpgrade.State.SpendCharge();
            HostShip.BeforeActionIsPerformed -= SpendCharge;
        }

        private void DisallowActionsWhileStressed(GenericAction action)
        {
            HostShip.OnCanPerformActionWhileStressed -= TemporaryAllowAnyActionsWhileStressed;
            HostShip.OnCheckCanPerformActionsWhileStressed -= TemporaryAllowActionsWhileStressed;
            HostShip.OnActionIsPerformed -= DisallowActionsWhileStressed;
        }

        private void DisallowActionsWhileStressedAlt(GenericShip ship)
        {
            HostShip.OnCanPerformActionWhileStressed -= TemporaryAllowAnyActionsWhileStressed;
            HostShip.OnCheckCanPerformActionsWhileStressed -= TemporaryAllowActionsWhileStressed;
            HostShip.OnActionIsPerformed -= DisallowActionsWhileStressed;
            HostShip.OnActionIsSkipped -= DisallowActionsWhileStressedAlt;
        }

        private void TemporaryAllowAnyActionsWhileStressed(GenericAction action, ref bool isAllowed)
        {
            isAllowed = true;
        }

        private void TemporaryAllowActionsWhileStressed(ref bool isAllowed)
        {
            isAllowed = true;
        }

    }
}

