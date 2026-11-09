using Ship;
using System;

namespace DamageDeckCardSE
{
    public class StructuralDamage : GenericDamageCard
    {
        public StructuralDamage()
        {
            Name = "Structural Damage";
            Type = CriticalCardType.Ship;
            CancelDiceResults.Add(DieSide.Success);
            ImageUrl = "DamageCards/StructuralDamage";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.AfterGotNumberOfDefenceDice += DebuffDefenceRolls;

            Host.Tokens.AssignCondition(typeof(Tokens.StructuralDamageCritToken));
            Triggers.FinishTrigger();
        }

        private void DebuffDefenceRolls(ref int dice)
        {
            if (Combat.AttackStep == CombatStep.Defence && Combat.Defender.ShipId == Host.ShipId)
            {
                Messages.ShowInfo($"{Name} causes {Combat.Defender.PilotInfo.PilotName} to roll one fewer defense die");
                dice--;
            }
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Host.Tokens.RemoveCondition(typeof(Tokens.StructuralDamageCritToken));
            Host.AfterGotNumberOfDefenceDice -= DebuffDefenceRolls;
        }
    }
}

namespace Tokens
{
    public class StructuralDamageCritToken : CritToken
    {
        public StructuralDamageCritToken(GenericShip host) : base(host)
        {
            Tooltip = "DamageCards/StructuralDamage";
        }
    }
}