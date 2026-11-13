using Ship;
using System;

namespace DamageDeckCardSE
{
    public class StunnedPilot : GenericDamageCard
    {
        public StunnedPilot()
        {
            Name = "Stunned Pilot";
            Type = CriticalCardType.Pilot;
            ImageUrl = "DamageCards/StunnedPilot";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnMovementFinish += RegisterCheckCollisionDamage;
            Host.Tokens.AssignCondition(typeof(Tokens.StunnedPilotCritToken));
            Triggers.FinishTrigger();
        }

        private void RegisterCheckCollisionDamage(GenericShip ship)
        {
            if (Host.IsLandedOnObstacle || Host.IsHitObstacles)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Stunned Pilot crit",
                    TriggerType = TriggerTypes.OnMovementFinish,
                    TriggerOwner = Host.Owner.PlayerNo,
                    EventHandler = CheckCollisionDamage
                });
            }
        }

        private void CheckCollisionDamage(object sender, System.EventArgs e)
        {
            Messages.ShowInfo($"{Name} has caused {Host.PilotInfo.PilotName} to suffer 1 Hit");

            DamageSourceEventArgs stunnedpilotDamage = new DamageSourceEventArgs()
            {
                Source = this,
                DamageType = DamageTypes.CriticalHitCard
            };

            Host.Damage.TryResolveDamage(1, stunnedpilotDamage, Triggers.FinishTrigger);
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Host.OnMovementFinish -= RegisterCheckCollisionDamage;
            Host.Tokens.RemoveCondition(typeof(Tokens.StunnedPilotCritToken));
        }
    }
}

namespace Tokens
{
    public class StunnedPilotCritToken : CritToken
    {
        public StunnedPilotCritToken(GenericShip host) : base(host)
        {
            Tooltip = "DamageCards/StunnedPilot";
        }
    }
}