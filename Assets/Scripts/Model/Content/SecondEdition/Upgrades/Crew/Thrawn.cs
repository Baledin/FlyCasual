using Abilities.SecondEdition;
using Actions;
using ActionsList;
using RulesList;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Thrawn : GenericUpgrade
    {
        public Thrawn() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Thrawn",
                UpgradeType.Crew,
                cost: 6,
                isLimited: true,
                charges: 2,
                regensCharges: true,
                abilityType: typeof(ThrawnAbility),
                restriction: new FactionRestriction(Faction.Imperial),
                addAction: new ActionInfo(typeof(JamAction), ActionColor.Red)
            );

            NameCanonical = "thrawn-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    // While you perform a jam or coordinate action, you may spend 1 charge to increase the range requirement for that action by 1.
    // After you perform a jam or coordinate action, you may spend 2 charges to perform a jam or coordinate action, treating it as red.

    public class ThrawnAbility : GenericAbility
    {
        GenericAction savedAction;

        public override void ActivateAbility()
        {
            HostShip.BeforeActionIsPerformed += RegisterSingleChargeAbility;
            HostShip.OnActionIsPerformed += RegisterDoubleChargeAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeActionIsPerformed -= RegisterSingleChargeAbility;
            HostShip.OnActionIsPerformed -= RegisterDoubleChargeAbility;
        }

        private void RegisterSingleChargeAbility(GenericAction action, ref bool canBePerformed)
        {
            if (HostUpgrade.State.Charges > 0 && (action is JamAction || action is CoordinateAction))
            {
                savedAction = action;

                RegisterAbilityTrigger(TriggerTypes.BeforeActionIsPerformed, AskUseSingleChargeAbility);
            }
        }

        private void AskUseSingleChargeAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                IncreaseActionRange,
                descriptionLong: $"Spend 1 charge to increase the range requirement for the {savedAction.Name} action?",
                callback: Triggers.FinishTrigger,
                imageHolder: HostUpgrade
            );
        }

        private void IncreaseActionRange(object sender, EventArgs e)
        {
            switch (savedAction)
            {
                case JamAction:
                    JamRule.OnCheckJamIsAllowed += CanPerformJam;
                    break;

                case CoordinateAction:
                    HostShip.OnCheckCoordinateModeModification += CanPerformCoordinate;
                    break;
            }

            HostUpgrade.State.SpendCharge();

            DecisionSubPhase.ConfirmDecision();
        }

        private void CanPerformJam(ref List<JamIsNotAllowedReasons> blockers, GenericShip jamSource, ITargetLockable jamTarget)
        {
            JamRule.OnCheckJamIsAllowed -= CanPerformJam;

            if (blockers.Contains(JamIsNotAllowedReasons.NotInRange) &&
                Tools.IsFriendly(jamSource, HostShip) &&
                (jamTarget.GetRangeToShip(HostShip) <= 2 || HostShip.SectorsInfo.RangeToShipBySector((GenericShip)jamTarget, Arcs.ArcType.Bullseye) <= 3))
            {
                blockers.Remove(JamIsNotAllowedReasons.NotInRange);
            }
        }

        private void CanPerformCoordinate(ref CoordinateActionData coordinateActionData)
        {
            HostShip.OnCheckCoordinateModeModification -= CanPerformCoordinate;

            coordinateActionData.MaxRange = 3;
        }

        private void RegisterDoubleChargeAbility(GenericAction action)
        {
            if (HostUpgrade.State.Charges > 1 && (action is JamAction || action is CoordinateAction))
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskPerformJamOrCooridinate);
            }
        }

        private void AskPerformJamOrCooridinate(object sender, EventArgs e)
        {
            HostShip.OnActionIsPerformed += SpendCharges;

            HostShip.AskPerformFreeAction(
                GetAbilityActions(),
                Triggers.FinishTrigger,
                HostUpgrade.UpgradeInfo.Name,
                descriptionLong: $"Would you like to spend 2 charges to perform a Jam or Cooridinate action, treating it as red?",
                imageHolder: HostUpgrade
            );
        }

        private List<GenericAction> GetAbilityActions()
        {
            return new()
            {
                new JamAction() { Color = ActionColor.Red },
                new CoordinateAction() { Color = ActionColor.Red }
            };
        }

        private void SpendCharges(GenericAction action)
        {
            HostUpgrade.State.SpendCharges(2);
        }
    }
}