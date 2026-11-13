using Ship;
using System;

namespace DamageDeckCardSE
{
    public class WeaponsFailure : GenericDamageCard
    {
        public WeaponsFailure()
        {
            Name = "Weapons Failure";
            Type = CriticalCardType.Ship;
            ImageUrl = "DamageCards/WeaponsFailure";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.AfterGotNumberOfAttackDice += ReduceNumberOfAttackDice;
            Host.OnGenerateActions += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(typeof(Tokens.WeaponsFailureCritToken));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Messages.ShowInfo($"{Name} has been repaired, {Host.PilotInfo.PilotName}'s attack dice have been restored");

            Host.Tokens.RemoveCondition(typeof(Tokens.WeaponsFailureCritToken));
            Host.AfterGotNumberOfAttackDice -= ReduceNumberOfAttackDice;
            Host.OnGenerateActions -= CallAddCancelCritAction;
        }

        private void ReduceNumberOfAttackDice(ref int value)
        {
            Messages.ShowInfo($"{Name}: {Host.PilotInfo.PilotName}'s attack dice have been reduced by 1");

            value--;
        }
    }
}

namespace Tokens
{
    public class WeaponsFailureCritToken : CritToken
    {
        public WeaponsFailureCritToken(GenericShip host) : base(host)
        {
            Tooltip = "DamageCards/WeaponsFailure";
        }
    }
}