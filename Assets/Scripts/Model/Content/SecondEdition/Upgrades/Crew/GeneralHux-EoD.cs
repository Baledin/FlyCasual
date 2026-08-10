using Abilities.SecondEdition;
using Actions;
using ActionsList;
using BoardTools;
using Content;
using Ship;
using Ship.SecondEdition.TIEFoFighter;
using Ship.SecondEdition.TIESfFighter;
using System;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class GeneralHuxEoD : GenericUpgrade
    {
        public GeneralHuxEoD() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "General Hux",
                UpgradeType.Crew,
                cost: 0,
                isLimited: true,
                abilityType: typeof(GeneralHuxEoDAbility),
                legalityInfo: new() { Legality.XWA }
            );

            IsHidden = true;
            ImageUrl = "https://infinitearenas.com/xw2xwa/images/pilots/pettyofficerthanisson-evacuationofdqar.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GeneralHuxEoDAbility : GenericAbility
    {
        // While you perform a white coordinate action, if you choose a friendly TIE/fo or TIE/sf, you may treat that action as red.
        // If you do, coordinate up to 2 additional friendly TIE/fo or TIE/sf, and each ship you coordinate must perform the same action, treating it as red.

        int minRange;
        int maxRange;

        public override void ActivateAbility()
        {
            HostShip.BeforeActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction action, ref bool isFree)
        {
            if (action is CoordinateAction && action.Color == ActionColor.White)
            {
                RegisterAbilityTrigger(TriggerTypes.BeforeActionIsPerformed, AskToTreatAsRed);
            }
        }

        private void AskToTreatAsRed(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                TreatActionAsRed,
                descriptionLong: "Do you want to treat your Coordinate as Red?\nIf you do, you may coordinate up to 3 TIE/fo or TIE/sf Fighters. Each ship you coordinate must perform the same action, treating that action as red.",
                imageHolder: HostUpgrade
            );
        }

        private void TreatActionAsRed(object sender, EventArgs e)
        {
            HostShip.OnCheckActionColor += TreatThisCoordinateActionAsRed;
            HostShip.OnCheckCoordinateModeModification += SetCustomCoordinateMode;

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void SetCustomCoordinateMode(ref CoordinateActionData coordinateActionData)
        {
            coordinateActionData.Filter = FilterCoordinateTargets;
            coordinateActionData.MaxTargets = 3;
            coordinateActionData.SameShipTypeLimit = false;
            coordinateActionData.SameActionLimit = true;
            coordinateActionData.TreatCoordinatedActionAsRed = true;

            minRange = coordinateActionData.MinRange;
            maxRange = coordinateActionData.MaxRange;

            HostShip.OnCheckCoordinateModeModification -= SetCustomCoordinateMode;
        }

        private bool FilterCoordinateTargets(GenericShip ship)
        {
            return ship.Owner.PlayerNo == Selection.ThisShip.Owner.PlayerNo
                && Board.CheckInRange(Selection.ThisShip, ship, minRange, maxRange, RangeCheckReason.CoordinateAction)
                && IsTieFighter(ship)
                && ship.CanBeCoordinated
                && Selection.ThisShip.CallCheckCanCoordinate(ship);
        }

        private bool IsTieFighter(GenericShip ship)
        {
            return ship is TIEFoFighter || ship is TIESfFighter;
        }

        private void TreatThisCoordinateActionAsRed(GenericAction action, ref ActionColor color)
        {
            if (action is CoordinateAction && color == ActionColor.White)
            {
                color = ActionColor.Red;
                HostShip.OnCheckActionColor -= TreatThisCoordinateActionAsRed;
            }
        }
    }
}