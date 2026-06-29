using Abilities.SecondEdition;
using ActionsList;
using Ship;
using SubPhases;
using System;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class InterferenceArray : GenericUpgrade
    {
        public InterferenceArray() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                name: "Interference Array",
                type: UpgradeType.Tech,
                abilityType: typeof(InterferenceArrayAbility),
                charges: 2
            );

            IsHidden = true;
            ImageUrl = "https://infinitearenas.com/xw2xwa/images/pilots/pettyofficerthanisson-evacuationofdqar.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class InterferenceArrayAbility : GenericAbility
    {
        // After you coordinate a ship, you may spend 1 charge. If you do, assign a jam token to an enemy ship at range 0-1 of the coordinated ship.

        GenericShip coordinateTarget;

        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += RegisterAfterCoordinateAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= RegisterAfterCoordinateAbility;
        }

        private void RegisterAfterCoordinateAbility(GenericAction action)
        {
            if (action is CoordinateAction && HostUpgrade.State.Charges > 0)
            {
                coordinateTarget = action.HostShip;

                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskAssignJam);
            }
        }

        private void AskAssignJam(object sender, EventArgs e)
        {
            if (!HasAvailableTarget())
            {
                Triggers.FinishTrigger();
                return;
            }

            AskToUseAbility(
                descriptionShort: HostUpgrade.UpgradeInfo.Name,
                useByDefault: AlwaysUseByDefault,
                useAbility: SelectTargetForJam,
                descriptionLong: $"Would you like to assign a jam token to an enemy ship at range 0-1 of {coordinateTarget.PilotInfo.PilotName}?",
                imageHolder: HostUpgrade
            );
        }

        private bool HasAvailableTarget()
        {
            return coordinateTarget.Owner.EnemyShips.Values.Any(s => coordinateTarget.GetRangeToShip(s) < 2);
        }

        private void SelectTargetForJam(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                selectTargetAction: AssignJam,
                filterTargets: IsEnemyShipInRange,
                getAiPriority: GetAiPriority,
                subphaseOwnerPlayerNo: HostShip.Owner.PlayerNo,
                name: HostUpgrade.UpgradeInfo.Name,
                description: $"Select a target to assign a Jam token.",
                imageSource: HostUpgrade,
                callback: DecisionSubPhase.ConfirmDecision
            );
        }

        private void AssignJam()
        {
            HostUpgrade.State.SpendCharge();

            TargetShip.Tokens.AssignToken(new JamToken(TargetShip, HostShip.Owner), SelectShipSubPhase.FinishSelection);
        }

        private bool IsEnemyShipInRange(GenericShip ship)
        {
            return Tools.IsAnotherTeam(HostShip, ship) && coordinateTarget.GetRangeToShip(ship) < 2;
        }

        private int GetAiPriority(GenericShip ship)
        {
            int priority = 0;

            foreach (GenericToken token in ship.Tokens.GetAllTokens())
            {
                priority += token switch
                {
                    BlueTargetLockToken => 100,
                    FocusToken => 20,
                    EvadeToken => 10,
                    CalculateToken => 10,
                    _ => 0
                };
            }

            return priority;
        }
    }
}