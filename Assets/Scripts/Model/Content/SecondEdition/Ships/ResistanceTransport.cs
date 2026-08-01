using System;
using System.Collections.Generic;
using System.Linq;
using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship;
using Ship.CardInfo;
using SubPhases;
using Tokens;
using UnityEngine;

namespace Ship.SecondEdition.ResistanceTransport
{
    public class ResistanceTransport : GenericShip
    {
        public ResistanceTransport() : base()
        {
            ShipInfo = new ShipCardInfo25
            (
                "Resistance Transport",
                BaseSize.Small,
                new FactionData
                (
                    new Dictionary<Faction, Type>
                    {
                        { Faction.Resistance, typeof(CovaNell) }
                    }
                ),
                new ShipArcsInfo(ArcType.Front, 2), 1, 5, 3,
                new ShipActionsInfo
                (
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(TargetLockAction)),
                    new ActionInfo(typeof(CoordinateAction), ActionColor.Red),
                    new ActionInfo(typeof(JamAction), ActionColor.Red)
                )
            );

            ModelInfo = new ShipModelInfo
            (
                "Resistance Transport",
                "Default",
                new Vector3(-3.7f, 8f, 5.55f),
                2.5f
            );

            DialInfo = new ShipDialInfo
            (
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.ReverseStraight, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.ReverseStraight, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed0, ManeuverDirection.Stationary, ManeuverBearing.Stationary, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Complex)
            );

            SoundInfo = new ShipSoundInfo
            (
                new List<string>()
                {
                    "XWing-Fly1",
                    "XWing-Fly2",
                    "XWing-Fly3"
                },
                "XWing-Laser", 3
            );

            ShipIconLetter = '>';
        }
    }
}
namespace Abilities.SecondEdition
{
    // After you perform an action, if you have fewer than 2 stress tokens, you may gain 1 stress token.
    // If you do, another friendly small ship at range 0-1 may gain 1 deplete token to perform a Boost action.
    class LeaveNoOneBehind : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += RegisterLeaveNoOneBehindAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= RegisterLeaveNoOneBehindAbility;
        }

        private void RegisterLeaveNoOneBehindAbility(GenericAction action)
        {
            if (HostShip.Tokens.CountTokensByType<StressToken>() < 2 && HostShip.Owner.Ships.Values.Any(AnotherFriendlySmallShipInRange))
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskGainStress);
            }
        }

        private void AskGainStress(object sender, EventArgs e)
        {
            AskToUseAbility(
                "Leave No One Behind",
                NeverUseByDefault,
                GainStressToAllowFriendlyBoost,
                showAlwaysUseOption: false,
                descriptionLong: "Do you want to gain 1 Stress Token to allow another friendly ship at range 0-1 to gain 1 Deplete and perform a Boost Action?",
                imageHolder: HostShip,
                showSkipButton: true
            );
        }

        private void GainStressToAllowFriendlyBoost(object sender, EventArgs e)
        {
            HostShip.Tokens.AssignToken(new StressToken(HostShip), ChooseTargetShip);
        }

        private void ChooseTargetShip()
        {
            SelectTargetForAbility(
                GainDepleteAndBoost,
                AnotherFriendlySmallShipInRange,
                AiShipPriority,
                HostShip.Owner.PlayerNo,
                "Leave No One Behind",
                "Choose a ship to gain 1 Deplete Token and perform a Boost Action.",
                HostShip,
                true,
                callback: DecisionSubPhase.ConfirmDecision
            );
        }

        private void GainDepleteAndBoost()
        {
            TargetShip.Tokens.AssignToken(new DepleteToken(TargetShip), Boost);
        }

        private void Boost()
        {
            Selection.ThisShip = TargetShip;

            TargetShip.AskPerformFreeAction(
                new BoostAction(),
                BoostCleanup,
                descriptionShort: "Leave No One Behind",
                descriptionLong: $"{TargetShip.PilotInfo.PilotName} may perform Peform a free boost action",
                imageHolder: HostShip
            );
        }

        private bool AnotherFriendlySmallShipInRange(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly })
                && FilterTargetsByRange(ship, 0, 1)
                && ship.ShipBase.Size == BaseSize.Small;
        }

        private int AiShipPriority(GenericShip ship)
        {
            return 1;
        }

        private void BoostCleanup()
        {
            Selection.ThisShip = HostShip;
            SelectShipSubPhase.FinishSelection();
        }
    }
}