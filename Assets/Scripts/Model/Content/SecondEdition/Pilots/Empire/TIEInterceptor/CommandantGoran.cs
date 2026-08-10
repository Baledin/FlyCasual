using Abilities.SecondEdition;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.TIEInterceptor
{
    public class CommandantGoran : TIEInterceptor
    {
        public CommandantGoran() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Commandant Goran",
                "Skystrike Superintendent",
                Faction.Imperial,
                4,
                4,
                8,
                isLimited: true,
                abilityType: typeof(CommandantGoranAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.Tie
                },
                skinName: "Skystrike Academy",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            PilotNameCanonical = "commandantgoran";
        }
    }

    public class CommandantGoranXWA : CommandantGoran
    {
        public CommandantGoranXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 10;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 4;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CommandantGoranAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbility;
        }

        public void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskUseAbility);
        }

        public void AskUseAbility(object sender, EventArgs e)
        {
            if (HostShip.Owner.Ships.Values.Any(s => MeetsCriteria(s)))
            {
                SelectTargetForAbility
                  (
                      AssignEvadeToken,
                      MeetsCriteria,
                      GetAiPriority,
                      HostShip.Owner.PlayerNo,
                      HostShip.PilotInfo.PilotName,
                      "You may assign an evade and remove a non-stress red token."
                  );
            }
            else
            {
                Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: There are no targets for ability");
                Triggers.FinishTrigger();
            }
        }

        public void AssignEvadeToken()
        {
            TargetShip.Tokens.AssignToken(new EvadeToken(TargetShip), RemoveRedToken);
        }

        public void RemoveRedToken()
        {
            List<GenericToken> redtokens = TargetShip.Tokens.GetTokensByColor(TokenColors.Red).Where(t => t is not StressToken).ToList();

            if (redtokens.Count > 0)
            {
                CommandantgoranDecisionSubPhase pilotAbilityDecision = (CommandantgoranDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                    HostShip.PilotName,
                    typeof(CommandantgoranDecisionSubPhase),
                    SelectShipSubPhase.FinishSelection
                );

                pilotAbilityDecision.DescriptionShort = $"{HostShip.PilotName} Pilot Ability";
                pilotAbilityDecision.DescriptionLong = "Select a non-stress red token to remove.";
                pilotAbilityDecision.ImageSource = HostShip;

                pilotAbilityDecision.RequiredPlayer = HostShip.Owner.PlayerNo;

                foreach (GenericToken Token in redtokens.OrderBy(t => t switch 
                    {
                        // Target Locks are prioritized by range, times two to give gaps for other priorities without ties
                        RedTargetLockToken => TargetShip.GetRangeToShip((t as RedTargetLockToken).OtherTargetLockTokenOwner as GenericShip) * 2,
                        StrainToken => 3,
                        _ => 7
                    }))
                {
                    string name = Token is RedTargetLockToken ? $"{Token.Name} {(Token as RedTargetLockToken).Letter}" : Token.Name;

                    pilotAbilityDecision.AddDecision(name, delegate { TargetShip.Tokens.RemoveToken(Token, DecisionSubPhase.ConfirmDecision); });
                }

                pilotAbilityDecision.DefaultDecisionName = pilotAbilityDecision.GetDecisions().FirstOrDefault().Name;
                pilotAbilityDecision.ShowSkipButton = true;
                pilotAbilityDecision.Start();
            }
            else
            {
                SelectShipSubPhase.FinishSelection();
            }
        }

        public bool MeetsCriteria(GenericShip ship)
        {
            return HostShip.GetRangeToShip(ship) <= 3 && ship.State.Initiative < HostShip.State.Initiative;
        }

        public int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }

        private class CommandantgoranDecisionSubPhase : DecisionSubPhase { }
    }
}