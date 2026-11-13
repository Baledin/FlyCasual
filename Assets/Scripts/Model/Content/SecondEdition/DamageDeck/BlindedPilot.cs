using ActionsList;
using Ship;
using System;

namespace DamageDeckCardSE
{
    public class BlindedPilot : GenericDamageCard
    {
        public BlindedPilot()
        {
            Name = "Blinded Pilot";
            Type = CriticalCardType.Pilot;
            AiAvoids = true;
            ImageUrl = "DamageCards/BlindedPilot";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnTryAddAvailableDiceModification += RestrictActionEffectsToForceOnly;
            Host.OnGenerateActions += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(typeof(Tokens.BlindedPilotCritToken));
            Triggers.FinishTrigger();
        }

        private void RestrictActionEffectsToForceOnly(GenericShip ship, GenericAction diceModification, ref bool canBeUsed)
        {
            if (Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker.ShipId == Host.ShipId
                && diceModification is not ForceAction
                && !diceModification.IsNotRealDiceModification)
            {
                canBeUsed = false;
            }
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Host.OnTryAddAvailableDiceModification -= RestrictActionEffectsToForceOnly;
            Host.OnGenerateActions -= CallAddCancelCritAction;
            Messages.ShowInfo($"{Host.PilotInfo.PilotName} is no longer Blinded and may fully modify their attacks");
            Host.Tokens.RemoveCondition(typeof(Tokens.BlindedPilotCritToken));
        }
    }
}

namespace Tokens
{
    public class BlindedPilotCritToken : CritToken
    {
        public BlindedPilotCritToken(GenericShip host) : base(host)
        {
            Tooltip = "DamageCards/BlindedPilot";
        }
    }
}