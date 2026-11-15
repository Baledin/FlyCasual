using Abilities.SecondEdition;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ChaffParticlesBoE : GenericUpgrade
    {
        public ChaffParticlesBoE() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "Chaff Particles",
                UpgradeType.Modification,
                cost: 0,
                abilityType: typeof(ChaffParticlesBoEAbility)
            );

            ImageUrl = "Pilots/gemmersojan-battleoverendor";

            NameCanonical = "chaffparticles-battleoverendor";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ChaffParticlesBoEAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnAfterNeutralizeResults += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAfterNeutralizeResults -= CheckAbility;
        }

        private void CheckAbility()
        {
            if ((HostShip.Tokens.HasTokenByColor(TokenColors.Red) || HostShip.Tokens.HasTokenByColor(TokenColors.Orange)) &&
                Combat.DiceRollDefence.Focuses > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAfterNeutralizeResults, AskToRemove);
            }
        }

        private void AskToRemove(object sender, EventArgs e)
        {

            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                UseAbility,
                descriptionLong: "Do you want to spend 1 Focus result to remove 1 red or orange token?",
                imageHolder: HostUpgrade
            );
        }

        private void UseAbility(object sender, EventArgs e)
        {
            ChaffParticlesAbilitySubphase subphase = Phases.StartTemporarySubPhaseNew<ChaffParticlesAbilitySubphase>(
                "Remove Token",
                DecisionSubPhase.ConfirmDecision
            );

            subphase.Name = HostShip.PilotInfo.PilotName;
            subphase.DescriptionShort = "Select a red or orange token to remove";
            subphase.ImageSource = HostShip;

            subphase.DecisionOwner = HostShip.Owner;
            subphase.ShowSkipButton = true;

            List<GenericToken> tokensToRemove = HostShip.Tokens.GetTokensByColor(TokenColors.Red, TokenColors.Orange);

            foreach (GenericToken token in tokensToRemove)
            {
                subphase.AddDecision(
                    token.Name + ((token.GetType() == typeof(RedTargetLockToken)) ? " \"" + (token as RedTargetLockToken).Letter + "\"" : ""),
                    delegate
                    {
                        tokensToRemove.Add(token);
                        ActionsHolder.RemoveTokens(tokensToRemove, delegate { DecisionSubPhase.ConfirmDecision(); });
                    }
                );
            }

            subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;
            subphase.Start();
        }

        private class ChaffParticlesAbilitySubphase : DecisionSubPhase { }
    }
}

