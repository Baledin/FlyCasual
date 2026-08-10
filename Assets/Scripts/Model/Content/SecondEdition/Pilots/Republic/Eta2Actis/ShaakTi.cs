using Abilities.Parameters;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.Eta2Actis
{
    public class ShaakTi : Eta2Actis
    {
        public ShaakTi()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Shaak Ti",
                "Compassionate Mentor",
                Faction.Republic,
                4,
                4,
                10,
                isLimited: true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.ShaakTiAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.ForcePower,
                    UpgradeType.Talent,
                    UpgradeType.Cannon,
                    UpgradeType.Astromech,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Red",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class ShaakTiXWA : ShaakTi
    {
        public ShaakTiXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 11;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 11;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new () { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ShaakTiAbility : TriggeredAbility
    {
        private readonly List<GenericToken> tokensToKeep = new ();

        public override TriggerForAbility Trigger => new AtTheStartOfPhase(typeof(EndStartSubPhase));

        public override AbilityPart Action => new EachShipCanDoAction
        (
            eachShipAction: SelectFocusOrEvadeToken,
            conditions: new ConditionsBlock
            (
                new TeamCondition(ShipTypes.Friendly),
                new RangeToHostCondition(minRange: 0, maxRange: 2),
                new CanSpendForceCondition(),
                new HasTokenCondition(tokensList: new List<Type>() { typeof(FocusToken), typeof(EvadeToken) })
            ),
            description: new AbilityDescription
            (
                "Shaak Ti",
                "Spend Force - friendly ships may choose Focus or Evade token to keep",
                imageSource: HostShip
            )
        );

        private void SelectFocusOrEvadeToken(GenericShip ship, Action callback)
        {
            if (HostShip.State.Force < 1)
            {
                Messages.ShowErrorToHuman("Not enough force to perform this action.");
                callback();
                return;
            }

            HostShip.State.SpendForce(
                1,
                delegate
                {
                    if (TargetShip.Tokens.HasToken<FocusToken>() && TargetShip.Tokens.HasToken<EvadeToken>())
                    {
                        AskToUseAbility(
                            "Choose token to keep",
                            AlwaysUseByDefault,
                            useAbility: delegate { KeepFocus(callback); },
                            dontUseAbility: delegate { KeepEvade(callback); },
                            descriptionLong: "Do you want to keep Focus token (otherwise Evade token will be kept)",
                            showSkipButton: false,
                            requiredPlayer: HostShip.Owner.PlayerNo
                        );
                    }
                    else
                    {
                        if (TargetShip.Tokens.HasToken<FocusToken>())
                        {
                            KeepToken(TargetShip.Tokens.GetToken<FocusToken>());
                        }
                        else if (TargetShip.Tokens.HasToken<EvadeToken>())
                        {
                            KeepToken(TargetShip.Tokens.GetToken<EvadeToken>());
                        }

                        callback();
                    }
                }
            );
        }

        private void KeepEvade(Action callback)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            KeepToken(TargetShip.Tokens.GetToken<EvadeToken>());
            callback();
        }

        private void KeepFocus(Action callback)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            KeepToken(TargetShip.Tokens.GetToken<FocusToken>());
            callback();
        }

        private void KeepToken(GenericToken token)
        {
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: {token.Host.PilotInfo.PilotName} doesn't remove {token.Name} during End Phase");

            token.Temporary = false;
            tokensToKeep.Add(token);

            Phases.Events.OnPlanningPhaseStart += Cleanup;
        }

        private void Cleanup()
        {
            Phases.Events.OnPlanningPhaseStart -= Cleanup;

            foreach (GenericToken token in new List<GenericToken>(tokensToKeep))
            {
                token.Temporary = true;
                tokensToKeep.Remove(token);
            }
        }
    }
}
