using ActionsList;
using Conditions;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;
using UpgradesList.SecondEdition;

namespace UpgradesList.SecondEdition
{
    public class TrackingFob : GenericUpgrade
    {
        public TrackingFob() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Tracking Fob",
                UpgradeType.Illicit,
                cost: 3,
                limited: 3,
                abilityType: typeof(Abilities.SecondEdition.TrackingFobAbility),
                restriction: new TagRestriction(Content.Tags.BountyHunter)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TrackingFobAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnSetupEnd += RegisterAskToAssignCondition;
            RulesList.TargetLocksRule.OnCheckTargetLockIsAllowed += RegisterTargetLockFilter;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupEnd -= RegisterAskToAssignCondition;
            RulesList.TargetLocksRule.OnCheckTargetLockIsAllowed -= RegisterTargetLockFilter;
        }

        private void RegisterTargetLockFilter(ref bool result, GenericShip attacker, ITargetLockable defender)
        {
            result = result || (Tools.IsAnotherTeam(HostShip, (GenericShip)defender) && defender.GetTokens().HasToken(typeof(MarkedForEliminationCondition)));
        }

        private void RegisterAskToAssignCondition()
        {
            Phases.Events.OnSetupEnd -= RegisterAskToAssignCondition;
            RegisterAbilityTrigger(TriggerTypes.OnSetupEnd, AskOpponentToSelectTarget);
        }

        private void AskOpponentToSelectTarget(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                selectTargetAction: AssignCondition,
                filterTargets: FilterSelection,
                getAiPriority: GetAiPriority,
                subphaseOwnerPlayerNo: HostShip.Owner.AnotherPlayer.PlayerNo
            );
        }

        private void AssignCondition()
        {
            Messages.ShowInfoToHuman($"Marked for Elimination condition is assigned to {TargetShip.PilotInfo.PilotName}");
            TargetShip.Tokens.AssignCondition(new MarkedForEliminationCondition(TargetShip));
            DecisionSubPhase.ConfirmDecision();
        }

        private bool FilterSelection(GenericShip ship)
        {
            if (HostShip.Owner.AnotherPlayer.Ships.Values.Any(s => !s.Tokens.HasToken(typeof(MarkedForEliminationCondition)) && s.PilotInfo.IsLimited))
            {
                return Tools.IsAnotherTeam(HostShip, ship) && !ship.Tokens.HasToken(typeof(MarkedForEliminationCondition)) && ship.PilotInfo.IsLimited;
            }
            else
            {
                return Tools.IsAnotherTeam(HostShip, ship) && !ship.Tokens.HasToken(typeof(MarkedForEliminationCondition));
            }
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 100 - ship.PilotInfo.Cost; // Priority assigned to cheaper ships with the assumption that they are generally less powerful
        }
    }
}

namespace Conditions
{
    public class MarkedForEliminationCondition : GenericToken
    {
        readonly List<Type> disallowedTypes = new() {
                typeof(FocusToken),
                typeof(CalculateToken),
                typeof(EvadeToken),
                typeof(GenericReinforceToken)
            };

        public MarkedForEliminationCondition(GenericShip host) : base(host)
        {
            Name = ImageName = "Marked for Elimination Condition";
            Temporary = false;

            Tooltip = "https://infinitearenas.com/xw2/images/conditions/markedforelimination.png";
        }

        public override void WhenAssigned()
        {
            Host.OnTryAddAvailableDiceModification += PreventGreenTokens;
        }

        public override void WhenRemoved()
        {
            Host.OnTryAddAvailableDiceModification -= PreventGreenTokens;
        }

        private void PreventGreenTokens(GenericShip ship, GenericAction action, ref bool data)
        {
            if (Combat.Defender is null || Combat.Defender != Host) return; // Not in combat/defender

            if (Combat.Attacker.UpgradeBar.HasUpgradeInstalled(typeof(TrackingFob))
                && ActionsHolder.HasTargetLockOn(Combat.Attacker, Combat.Defender)
                && action.TokensSpend.Any(tt => disallowedTypes.Any(dt => tt.IsAssignableFrom(dt))))
            {
                data = false;
            }
        }
    }
}