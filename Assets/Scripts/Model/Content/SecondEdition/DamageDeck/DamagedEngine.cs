using Ship;
using System;

namespace DamageDeckCardSE
{
    public class DamagedEngine : GenericDamageCard
    {
        public DamagedEngine()
        {
            Name = "Damaged Engine";
            Type = CriticalCardType.Ship;
            ImageUrl = "DamageCards/DamagedEngine";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.AfterGetManeuverColorIncreaseComplexity += TurnManeuversAreHarder;

            Host.Tokens.AssignCondition(typeof(Tokens.DamagedEngineCritToken));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Messages.ShowInfo($"{Name} has been repaired, {Host.PilotInfo.PilotName}'s turn maneuvers regained normal colors");
            Host.Tokens.RemoveCondition(typeof(Tokens.DamagedEngineCritToken));

            Host.AfterGetManeuverColorIncreaseComplexity -= TurnManeuversAreHarder;
        }

        private void TurnManeuversAreHarder(Ship.GenericShip ship, ref Movement.ManeuverHolder movement)
        {
            if (movement.ColorComplexity != Movement.MovementComplexity.None)
            {
                if (movement.Bearing == Movement.ManeuverBearing.Turn)
                {
                    movement.ColorComplexity = Movement.GenericMovement.IncreaseComplexity(movement.ColorComplexity);
                }
            }
        }
    }
}

namespace Tokens
{
    public class DamagedEngineCritToken : CritToken
    {
        public DamagedEngineCritToken(GenericShip host) : base(host)
        {
            Tooltip = "DamageCards/DamagedEngine";
        }
    }
}