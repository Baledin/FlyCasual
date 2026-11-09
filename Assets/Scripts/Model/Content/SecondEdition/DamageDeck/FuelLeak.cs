using Ship;
using System;

namespace DamageDeckCardSE
{
    public class FuelLeak : GenericDamageCard
    {
        private bool IgnoredSelf;

        public FuelLeak()
        {
            Name = "Fuel Leak";
            Type = CriticalCardType.Ship;
            ImageUrl = "DamageCards/FuelLeak";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnGenerateActions += CallAddCancelCritAction;
            Host.OnDamageWasSuccessfullyDealt += CheckToSufferAdditionalDamageAndRepair;
            Host.Tokens.AssignCondition(typeof(Tokens.FuelLeakCritToken));
            Triggers.FinishTrigger();
        }

        private void CheckToSufferAdditionalDamageAndRepair(GenericShip ship, bool isCritical)
        {
            if (isCritical)
            {
                if (!IgnoredSelf)
                {
                    IgnoredSelf = true;
                }
                else
                {
                    DiscardEffect();

                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Fuel Leak",
                        TriggerType = TriggerTypes.OnDamageWasSuccessfullyDealt,
                        TriggerOwner = ship.Owner.PlayerNo,
                        EventHandler = SufferAdditionalDamage,
                    });
                }
            }
        }

        private void SufferAdditionalDamage(object sender, System.EventArgs e)
        {
            Messages.ShowInfo($"{Name} causes {Host.PilotInfo.PilotName} to suffer 1 additional Hit");

            DamageSourceEventArgs fuelleakDamage = new DamageSourceEventArgs()
            {
                Source = "Critical hit card",
                DamageType = DamageTypes.CriticalHitCard
            };

            Host.Damage.TryResolveDamage(1, fuelleakDamage, Triggers.FinishTrigger);
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Host.OnGenerateActions -= CallAddCancelCritAction;
            Host.OnDamageWasSuccessfullyDealt -= CheckToSufferAdditionalDamageAndRepair;
            Host.Tokens.RemoveCondition(typeof(Tokens.FuelLeakCritToken));
        }
    }
}

namespace Tokens
{
    public class FuelLeakCritToken : CritToken
    {
        public FuelLeakCritToken(GenericShip host) : base(host)
        {
            Tooltip = "DamageCards/FuelLeak";
        }
    }
}