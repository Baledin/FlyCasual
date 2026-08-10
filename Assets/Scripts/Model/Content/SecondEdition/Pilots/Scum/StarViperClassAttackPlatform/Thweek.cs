using Abilities.SecondEdition;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.StarViperClassAttackPlatform
{
    public class Thweek : StarViperClassAttackPlatform
    {
        public Thweek() : base()
        {
            PilotInfo = new PilotCardInfo25(
                pilotName: "Thweek",
                pilotTitle: "Versatile Spy",
                faction: Faction.Scum,
                initiative: 4,
                cost: 12,
                loadoutValue: 11,
                isLimited: true,
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Torpedo
                },
                abilityType: typeof(ThweekAbility),
                legality: new List<Legality>() { Legality.XWA }
            );

            PilotNameCanonical = "thweek-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    // During the System Phase, you may gain a tractor token.
    // At the start of the Engagement Phase, you may gain a deplete token to remove a tractor token.

    public class ThweekAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnSystemsPhaseStart += RegisterSystemPhaseAbility;
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterEngagementPhaseAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsPhaseStart -= RegisterSystemPhaseAbility;
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterEngagementPhaseAbility;
        }

        private void RegisterSystemPhaseAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnSystemsPhaseStart, AskGainTractorToken);
        }

        private void AskGainTractorToken(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                NeverUseByDefault,
                GainTractorToken,
                callback: Triggers.FinishTrigger,
                descriptionLong: $"Would you like to gain a tractor token?",
                imageHolder: HostShip,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void GainTractorToken(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Selection.ChangeActiveShip(HostShip);

            TractorBeamToken tractor = new(HostShip, HostShip.Owner);
            HostShip.Tokens.AssignToken(tractor, Triggers.FinishTrigger);
        }

        private void RegisterEngagementPhaseAbility()
        {
            if (HostShip.Tokens.HasToken<TractorBeamToken>())
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskGainDepleteTokenToRemoveTractor);
            }
        }

        private void AskGainDepleteTokenToRemoveTractor(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                NeverUseByDefault,
                GainDepleteAndRemoveTractorTokens,
                callback: Triggers.FinishTrigger,
                descriptionLong: $"Would you like to gain a deplete to remove a tractor token?",
                imageHolder: HostShip,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void GainDepleteAndRemoveTractorTokens(object sender, EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(DepleteToken), RemoveTractorBeamToken);
        }

        private void RemoveTractorBeamToken()
        {
            HostShip.Tokens.RemoveToken(typeof(TractorBeamToken), DecisionSubPhase.ConfirmDecision);
        }
    }
}