using ActionsList;
using Ship;
using System;

namespace DamageDeckCardSE
{
    public class DamagedSensorArray : GenericDamageCard
    {
        public DamagedSensorArray()
        {
            Name = "Damaged Sensor Array";
            Type = CriticalCardType.Ship;
            ImageUrl = "DamageCards/DamagedSensorArray";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnTryAddAction += OnlyCancelCritActionsOrFocus;
            Host.OnGenerateActions += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(typeof(Tokens.DamagedSensorArrayCritToken));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Messages.ShowInfo($"{Name} has been repaired, {Host.PilotInfo.PilotName} can perform actions as usual");
            Host.Tokens.RemoveCondition(typeof(Tokens.DamagedSensorArrayCritToken));

            Host.OnTryAddAction -= OnlyCancelCritActionsOrFocus;
            Host.OnGenerateActions -= CallAddCancelCritAction;
        }

        private void OnlyCancelCritActionsOrFocus(GenericShip ship, GenericAction action, ref bool result)
        {
            if (!action.IsCritCancelAction && !(action is FocusAction))
            {
                result = false;
            }
        }
    }
}

namespace Tokens
{
    public class DamagedSensorArrayCritToken : CritToken
    {
        public DamagedSensorArrayCritToken(GenericShip host) : base(host)
        {
            Tooltip = "DamageCards/DamagedSensorArray";
        }
    }
}