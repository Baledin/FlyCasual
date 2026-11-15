using Abilities.SecondEdition;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class TrueGrit : GenericUpgrade
    {
        public TrueGrit()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "True Grit",
                UpgradeType.Talent,
                cost: 0,
                abilityType: typeof(TrueGritAbility)
            );

            IsHidden = true;

            ImageUrl = "Pilots/tomaxbren-swz98";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TrueGritAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnActivationPhaseEnd_Triggers += CheckAbility;

        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnActivationPhaseEnd_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            var tokens = HostShip.Tokens;
            if (tokens.HasToken<StrainToken>()) return;


            if (tokens.GetNonLockRedOrangeTokens().Any())
            {
                RegisterAbilityTrigger(TriggerTypes.OnAbilityDirect, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            AskToUseAbility("True Grit", AlwaysUseByDefault, UseAbility,
                descriptionLong:
                "If you have no Strain tokens, you may gain 1 Strain token to discard a red non-Lock token or an orange token.");
        }

        private void UseAbility(object sender, EventArgs e)
        {
            IsAbilityUsed = true;
            var tokens = HostShip.Tokens;

            var decisions = new Dictionary<string, EventHandler>();
            var tooltips = new Dictionary<string, string>();
            foreach (var token in tokens.GetNonLockRedOrangeTokens())
            {
                decisions.Add(token.Name, delegate { DiscardToken(token); });
                tooltips.Add(token.Name, $"Discard one {token.Name} token");
            }

            AskForDecision("Choose token to discard", "Select which token you want to discard",
                decisions: decisions, tooltips: tooltips);
        }

        private void DiscardToken(GenericToken token)
        {
            HostShip.Tokens.RemoveToken(token, GainOneStrainToken);
        }

        private void GainOneStrainToken()
        {
            // TODO Locks up here :-(
            HostShip.Tokens.AssignToken(typeof(StrainToken), Finish);
        }

        private void Finish()
        {
            DecisionSubPhase.ConfirmDecision();
        }
    }
}
