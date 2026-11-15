using System;
using Tokens;

namespace DamageDeckCardSE
{
    public class PanickedPilot : GenericDamageCard
    {
        public PanickedPilot()
        {
            Name = "Panicked Pilot";
            Type = CriticalCardType.Pilot;
            ImageUrl = "DamageCards/PanickedPilot";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.Tokens.AssignTokens(CreateStressToken, 2, FinishAndDiscard);
        }

        private GenericToken CreateStressToken()
        {
            return new StressToken(Host);
        }

        private void FinishAndDiscard()
        {
            Triggers.FinishTrigger();
            DiscardEffect();
        }
    }
}