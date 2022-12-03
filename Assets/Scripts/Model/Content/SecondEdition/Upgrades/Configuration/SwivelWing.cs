﻿using Upgrade;
using Ship;
using System;
using SubPhases;
using System.Collections.Generic;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class SwivelWingDown : GenericDualUpgrade
    {
        public SwivelWingDown() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Swivel Wing (Down)",
                UpgradeType.Configuration,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.GauntletFighter.GauntletFighter)),
                abilityType: typeof(Abilities.SecondEdition.SwivelWingDownAbility)
            );

            ImageUrl = "https://infinitearenas.com/xw2/images/upgrades/swivelwing.png";

            AnotherSide = typeof(SwivelWingUp);
        }
    }

    public class SwivelWingUp : GenericDualUpgrade
    {
        public SwivelWingUp() : base()
        {
            IsHidden = true;
            NameCanonical = "SwivelWing-anotherside";

            UpgradeInfo = new UpgradeCardInfo(
                "Swivel Wing (Up)",
                UpgradeType.Configuration,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.GauntletFighter.GauntletFighter)),
                abilityType: typeof(Abilities.SecondEdition.SwivelWingUpAbility)
            );

            ImageUrl = "https://infinitearenas.com/xw2/images/upgrades/swivelwing-sideb.png";

            IsSecondSide = true;

            AnotherSide = typeof(SwivelWingDown);
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SwivelWingUpAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnGameStart += ChangeInitialWingsPosition;
            HostShip.OnMovementExecuted += RegisterAskToUseFlip;
            HostShip.AfterGotNumberOfDefenceDice += DecreaseDice;
            HostShip.Tokens.AssignCondition(new Conditions.SwivelWingCondition(HostShip, HostUpgrade));
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnGameStart -= ChangeInitialWingsPosition;
            HostShip.OnMovementExecuted -= RegisterAskToUseFlip;
            HostShip.AfterGotNumberOfDefenceDice -= DecreaseDice;
            HostShip.Tokens.RemoveCondition(typeof(Conditions.SwivelWingCondition));
        }

        private void DecreaseDice(ref int count)
        {
            Messages.ShowInfo("Pivot Wing Ability: This ship has -1 defense die");
            count--;
        }

        protected void ChangeInitialWingsPosition()
        {
            Phases.Events.OnGameStart -= ChangeInitialWingsPosition;

            //HostShip.WingsOpen();
        }

        protected void RegisterAskToUseFlip(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            if (ship.AssignedManeuver.Bearing == Movement.ManeuverBearing.Stationary) return;

            RegisterAbilityTrigger(TriggerTypes.OnMovementExecuted, AskToFlip);
        }

        protected void AskToFlip(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                DoFlipSide,
                descriptionLong: "Do you want to flip the wing down?",
                imageHolder: HostUpgrade
            );
        }

        private void DoFlipSide(object sender, EventArgs e)
        {
            //HostShip.WingsClose();
            (HostUpgrade as GenericDualUpgrade).Flip();
            DecisionSubPhase.ConfirmDecision();
        }

        public override void ActivateAbilityForSquadBuilder() {}
        public override void DeactivateAbilityForSquadBuilder() {}
    }

    public class SwivelWingDownAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            ChangeInitialWingsPosition();
            //HostShip.OnMovementActivationStart += RegisterAskToFlip;
            HostShip.OnMovementExecuted += RegisterAskToRotate;
        }

        public override void DeactivateAbility()
        {
            //HostShip.WingsOpen();
            //HostShip.OnMovementActivationStart -= RegisterAskToFlip;
            HostShip.OnMovementExecuted -= RegisterAskToRotate;
        }

        protected void ChangeInitialWingsPosition()
        {
            //HostShip.WingsClose();
        }

        protected void RegisterAskToRotate(GenericShip ship)
        {
            if (ship.AssignedManeuver.Bearing == Movement.ManeuverBearing.Stationary)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskToRotate);
            }
        }

        protected void RegisterAskToFlip(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToFlip);
        }

        private void AskToFlip(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                DoFlipSide,
                descriptionLong: "Do you want to flip the wing up?",
                imageHolder: HostUpgrade
            );
        }

        private void DoFlipSide(object sender, EventArgs e)
        {
            //HostShip.WingsOpen();
            (HostUpgrade as GenericDualUpgrade).Flip();
            DecisionSubPhase.ConfirmDecision();
        }

        protected void AskToRotate(object sender, EventArgs e)
        {
            PivotWindDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<PivotWindDecisionSubphase>("Rotate the ship?", Triggers.FinishTrigger);

            subphase.DescriptionShort = "Swivel Wing";
            subphase.DescriptionLong = "Rotate the ship?";
            subphase.ImageSource = HostUpgrade;

            subphase.AddDecision("180", Rotate180, isCentered: true);
            subphase.AddDecision("90 Counterclockwise", Rotate90Counterclockwise);
            subphase.AddDecision("90 Clockwise", Rotate90Clockwise);
            subphase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); }, isCentered: true);

            (HostUpgrade as GenericDualUpgrade).Flip();

            subphase.Start();
        }

        private void Rotate180(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            HostShip.Rotate180(Triggers.FinishTrigger);
        }

        private void Rotate90Clockwise(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            HostShip.Rotate90Clockwise(Triggers.FinishTrigger);
        }

        private void Rotate90Counterclockwise(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            HostShip.Rotate90Counterclockwise(Triggers.FinishTrigger);
        }

        private class PivotWindDecisionSubphase : DecisionSubPhase { };
    }
}

namespace Conditions
{
    public class SwivelWingCondition : GenericToken
    {
        public SwivelWingCondition(GenericShip host, GenericUpgrade source) : base(host)
        {
            Name = ImageName = "Debuff Token";
            TooltipType = source.GetType();
            Temporary = false;
        }
    }
}