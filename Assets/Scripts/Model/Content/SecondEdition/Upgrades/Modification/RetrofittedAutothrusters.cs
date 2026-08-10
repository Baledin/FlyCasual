using Abilities.SecondEdition;
using ActionsList;
using Content;
using Movement;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class RetrofittedAutothrusters : GenericUpgrade
    {
        public RetrofittedAutothrusters() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Retrofitted Autothrusters",
                UpgradeType.Modification,
                cost: 6,
                charges: 2,
                restriction: new BaseSizeRestriction(Ship.BaseSize.Small),
                abilityType: typeof(RetrofittedAutothrustersAbility),
                legalityInfo: new List<Legality>() { Legality.XWA }
            );

            NameCanonical = "retrofittedautothrusters-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RetrofittedAutothrustersAbility : GenericAbility
    {
        readonly List<ManeuverBearing> acceptedBearings = new()
            {
                ManeuverBearing.Turn,
                ManeuverBearing.Bank,
                ManeuverBearing.TallonRoll,
                ManeuverBearing.SegnorsLoop
            };

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
            if (HostUpgrade.State.Charges > 0 && acceptedBearings.Contains(HostShip.AssignedManeuver.Bearing) && HostShip.AssignedManeuver.Speed == 3)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            HostShip.OnCanPerformActionWhileStressed += AllowBarrelRollWhileStressed;
            HostShip.OnActionIsPerformed += PayCost;
            HostShip.OnActionIsSkipped += CleanUp;

            HostShip.AskPerformFreeAction(
                new BarrelRollAction(),
                Triggers.FinishTrigger,
                HostUpgrade.UpgradeInfo.Name,
                descriptionLong: $"Spend a charge to perform a barrel roll action, even while stressed?"
            );
        }

        private void PayCost(GenericAction action)
        {
            HostUpgrade.State.SpendCharge();
            CleanUp(HostShip);
        }

        private void AllowBarrelRollWhileStressed(GenericAction action, ref bool canPerform)
        {
            if (action is BarrelRollAction) canPerform = true;
        }

        private void CleanUp(GenericShip ship)
        {
            HostShip.OnCanPerformActionWhileStressed -= AllowBarrelRollWhileStressed;
            HostShip.OnActionIsPerformed -= PayCost;
            HostShip.OnActionIsSkipped -= CleanUp;
        }
    }
}