﻿using System;
using System.Collections.Generic;
using System.Linq;
using Obstacles;
using Ship;
using SubPhases;

namespace Obstacles
{
    public class Asteroid : GenericObstacle
    {
        public Asteroid(string name, string shortName) : base(name, shortName)
        {

        }

        public override string GetTypeName => "Asteroid";

        public override void OnHit(GenericShip ship)
        {
            if (Selection.ThisShip.IgnoreObstacleTypes.Contains(typeof(Asteroid))) {
                return;
            }

            if (!Selection.ThisShip.CanPerformActionsWhenOverlapping
                && Editions.Edition.Current.RuleSet.GetType() == typeof(Editions.RuleSets.RuleSet20))
            {
                Messages.ShowErrorToHuman(ship.PilotInfo.PilotName + " hit an asteroid during movement, their action subphase is skipped");
                Selection.ThisShip.IsSkipsActionSubPhase = true;
            }

            if (Editions.Edition.Current.RuleSet.GetType() == typeof(Editions.RuleSets.RuleSet25))
            {
                Messages.ShowErrorToHuman($"{ship.PilotInfo.PilotName} hit an asteroid during movement and suffered damage");
                DealAutoAsteroidDamage(ship, () => StartToRoll(ship));
            }
            else
            {
                StartToRoll(ship);
            }
        }

        private void StartToRoll(GenericShip ship)
        {
            Messages.ShowErrorToHuman(ship.PilotInfo.PilotName + " hit an asteroid during movement, rolling for damage");

            AsteroidHitCheckSubPhase newPhase = (AsteroidHitCheckSubPhase)Phases.StartTemporarySubPhaseNew(
                "Damage from asteroid collision",
                typeof(AsteroidHitCheckSubPhase),
                delegate
                {
                    Phases.FinishSubPhase(typeof(AsteroidHitCheckSubPhase));
                    Triggers.FinishTrigger();
                });
            newPhase.TheShip = ship;
            newPhase.TheObstacle = this;
            newPhase.Start();
        }

        public override void OnShotObstructedExtra(GenericShip attacker, GenericShip defender, ref int result)
        {
            // Only default effect
        }

        private void DealAutoAsteroidDamage(GenericShip ship, Action callback)
        {
            ship.Damage.TryResolveDamage(1, new DamageSourceEventArgs() { DamageType = DamageTypes.ObstacleCollision, Source = this }, callback);
        }

        public override void AfterObstacleRoll(GenericShip ship, DieSide side, Action callback)
        {
            if (side == DieSide.Crit || side == DieSide.Success)
            {
                DealAsteroidDamage(ship, side, callback);
            }
            else
            {
                NoEffect(callback);
            }
        }

        private void DealAsteroidDamage(GenericShip ship, DieSide side, Action callback)
        {
            int normalDamage = 0;
            int criticalDamage = 0;
            if (side == DieSide.Crit && Editions.Edition.Current.RuleSet.GetType() == typeof(Editions.RuleSets.RuleSet20))
            {
                Messages.ShowErrorToHuman($"{ship.PilotInfo.PilotName} suffered critical damage after damage roll");
                criticalDamage = 1;
            }
            else
            {
                Messages.ShowErrorToHuman($"{ship.PilotInfo.PilotName} suffered damage after damage roll");
                normalDamage = 1;
            }
            ship.Damage.TryResolveDamage(normalDamage, criticalDamage, new DamageSourceEventArgs() { DamageType = DamageTypes.ObstacleCollision, Source = this }, callback);
        }

        private void NoEffect(Action callback)
        {
            Messages.ShowInfoToHuman("No damage");
            callback();
        }
    }
}

namespace SubPhases
{

    public class AsteroidHitCheckSubPhase : DiceRollCheckSubPhase
    {
        private GenericShip prevActiveShip = Selection.ActiveShip;
        public GenericObstacle TheObstacle { get; set; }

        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 1;

            AfterRoll = FinishAction;
            Selection.ActiveShip = TheShip;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();
            Selection.ActiveShip = prevActiveShip;

            TheObstacle.AfterObstacleRoll(TheShip, CurrentDiceRoll.DiceList[0].Side, CallBack);
        }
    }
}
