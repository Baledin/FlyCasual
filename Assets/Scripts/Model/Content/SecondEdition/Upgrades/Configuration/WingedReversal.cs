using Abilities.SecondEdition;
using Content;
using Movement;
using Ship;
using Ship.SecondEdition.GauntletFighter;
using Ship.SecondEdition.UT60DUWing;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class WingedReversal : GenericUpgrade
    {
        public WingedReversal() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                name: "Winged Reversal",
                type: UpgradeType.Configuration,
                cost: 0,
                charges: 2,
                abilityType: typeof(WingedReversalAbility),
                restriction: new ShipRestriction(typeof(UT60DUWing), typeof(GauntletFighter)),
                legalityInfo: new List<Legality>() { Legality.XWA }
            );

            NameCanonical = "wingedreversal-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    // After you reveal a turn maneuver, you may spend 1 charge, gain 1 strain token, and increase its difficulty. If you do, perform that maneuver as a tallon roll maneuver in the same direction instead.

    public class WingedReversalAbility : GenericAbility
    {
        string maneuverKey;
        MovementComplexity originalColor;

        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (HostUpgrade.State.Charges > 0 &&
                ship.AssignedManeuver.Bearing == ManeuverBearing.Turn)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, RegisterAbility);
            }
        }

        private void RegisterAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                UseAbility,
                callback: Triggers.FinishTrigger,
                descriptionLong: "Do you want to spend 1 charge, gain 1 strain token, and increase your maneuver difficulty to perform a tallon roll maneuver in the same direction instead?",
                imageHolder: HostUpgrade
            );
        }

        private void UseAbility(object sender, EventArgs e)
        {
            HostShip.OnMovementFinish += SpendCost;
            ChangeManeuver();
        }

        private void SpendCost(GenericShip ship)
        {
            HostShip.OnMovementFinish -= SpendCost;

            HostShip.Tokens.AssignToken(typeof(StrainToken), HostUpgrade.State.SpendCharge);
        }

        private void ChangeManeuver()
        {
            HostShip.OnMovementFinish += RestoreManuvers;

            maneuverKey = HostShip.AssignedManeuver.ToString()[..4] + "E";
            originalColor = HostShip.Maneuvers.ContainsKey(maneuverKey) ? HostShip.Maneuvers[maneuverKey] : MovementComplexity.None;

            HostShip.Maneuvers[maneuverKey] = GenericMovement.IncreaseComplexity(HostShip.AssignedManeuver.ColorComplexity);

            HostShip.SetAssignedManeuver(ShipMovementScript.MovementFromString(maneuverKey));

            DecisionSubPhase.ConfirmDecision();
        }

        private void RestoreManuvers(GenericShip ship)
        {
            HostShip.OnMovementFinish -= RestoreManuvers;

            if (originalColor != MovementComplexity.None)
            {
                HostShip.Maneuvers[maneuverKey] = originalColor;
            }
            else
            {
                HostShip.Maneuvers.Remove(maneuverKey);
            }
        }
    }
}
