using Ship;
using System;

namespace DamageDeckCardSE
{
    public class ConsoleFire : GenericDamageCard
    {
        public ConsoleFire()
        {
            Name = "Console Fire";
            Type = CriticalCardType.Ship;
            ImageUrl = "DamageCards/ConsoleFire";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnCombatActivation += PlanRollForDamage;
            Host.OnGenerateActions += CallAddCancelCritAction;
            Host.OnShipIsDestroyed += DiscardEffect;

            Host.Tokens.AssignCondition(typeof(Tokens.ConsoleFireCritToken));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Host.Tokens.RemoveCondition(typeof(Tokens.ConsoleFireCritToken));

            Host.OnCombatActivation -= PlanRollForDamage;
            Host.OnGenerateActions -= CallAddCancelCritAction;
            Host.OnShipIsDestroyed -= DiscardEffect;
        }

        private void DiscardEffect(GenericShip ship, bool flag)
        {
            DiscardEffect();
        }

        protected void PlanRollForDamage(GenericShip ship = null)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "#" + Host.ShipId + ": Console Fire Crit",
                TriggerOwner = Host.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnCombatActivation,
                EventHandler = RollForDamage
            });
        }

        protected void RollForDamage(object sender, EventArgs e)
        {
            Selection.ActiveShip = Host;
            Selection.ThisShip = Host;

            Phases.StartTemporarySubPhaseOld(
                Name,
                typeof(SubPhases.ConsoleFireCheckSubPhase),
                delegate
                {
                    Phases.FinishSubPhase(typeof(SubPhases.ConsoleFireCheckSubPhase));
                    Triggers.FinishTrigger();
                }
            );
        }
    }
}

namespace SubPhases
{
    public class ConsoleFireCheckSubPhase : DiceRollCheckSubPhase
    {
        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 1;

            AfterRoll = FinishAction;

            Name = "#" + Selection.ActiveShip.ShipId + ": " + Name;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if (CurrentDiceRoll.DiceList[0].Side == DieSide.Success)
            {
                SufferDamage();
            }
            else
            {
                NoDamage();
            }
        }

        private void SufferDamage()
        {
            Messages.ShowInfo("Console Fire: This ship suffered 1 damage");

            DamageSourceEventArgs consolefireDamage = new()
            {
                Source = "Critical hit card",
                DamageType = DamageTypes.CriticalHitCard
            };

            Selection.ActiveShip.Damage.TryResolveDamage(1, consolefireDamage, CallBack);
        }

        private void NoDamage()
        {
            Messages.ShowInfoToHuman("Console Fire: No damage was suffered");
            CallBack();
        }
    }
}

namespace Tokens
{
    public class ConsoleFireCritToken : CritToken
    {
        public ConsoleFireCritToken(GenericShip host) : base(host)
        {
            Tooltip = "DamageCards/ConsoleFire";
        }
    }
}