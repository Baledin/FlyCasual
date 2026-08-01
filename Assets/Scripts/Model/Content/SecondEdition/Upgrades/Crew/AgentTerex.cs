using ActionsList;
using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class AgentTerex : GenericDualUpgrade
    {
        public AgentTerex() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Agent Terex",
                UpgradeType.Crew,
                cost: 7,
                restriction: new FactionRestriction(Faction.FirstOrder),
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.AgentTerexCrewAbility)
            );

            SelectSideOnSetup = false;
            AnotherSide = typeof(AgentTerexCyborg);
            NameCanonical = "agentterex";

            Avatar = new AvatarInfo(
                Faction.FirstOrder,
                new Vector2(294, 0)
            );
        }
    }

    public class AgentTerexCyborg : GenericDualUpgrade
    {
        public AgentTerexCyborg() : base()
        {
            IsHidden = true; // Hidden in Squad Builder only

            UpgradeInfo = new UpgradeCardInfo(
                "Agent Terex (Cyborg)",
                UpgradeType.Crew,
                cost: 7,
                addAction: new Actions.ActionInfo(typeof(Abilities.SecondEdition.AgentTerexCyborgAction)),
                abilityType: typeof(Abilities.SecondEdition.AgentTerexCyborgAbility)
            );

            AnotherSide = typeof(AgentTerex);
            IsSecondSide = true;
            NameCanonical = "agentterex-sideb";

            Avatar = new AvatarInfo(
                Faction.FirstOrder,
                new Vector2(281, 0)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AgentTerexCrewAbility : GenericAbility
    {
        private int StoredTokens = 3;

        public override void ActivateAbility()
        {
            HostShip.OnGameStart += UpdateInfoPanel;
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAsktoGrantCalculateToken;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGameStart -= UpdateInfoPanel;
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAsktoGrantCalculateToken;
        }

        private void UpdateInfoPanel()
        {
            HostUpgrade.NamePostfix = $"(Tokens: {StoredTokens})";
            Roster.UpdateUpgradesPanel(HostShip, HostShip.InfoPanel);
        }

        private void RegisterAsktoGrantCalculateToken()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskToGrantCalculateToken);
        }

        private void AskToGrantCalculateToken(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                GrantCalculateToken,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: "Agent Terex",
                description: "You may assign Calculate token from \"Agent Terex\" card to a ship",
                imageSource: HostUpgrade,
                showSkipButton: true
            );
        }

        private void GrantCalculateToken()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            StoredTokens--;
            UpdateInfoPanel();

            Messages.ShowInfo($"Agent Terex: {TargetShip.PilotInfo.PilotName} gains Calculate Token");
            TargetShip.Tokens.AssignToken(typeof(CalculateToken), CheckFlip);
        }

        private void CheckFlip()
        {
            if (StoredTokens == 0)
            {
                (HostUpgrade as GenericDualUpgrade).Flip();
                Triggers.FinishTrigger();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.OtherFriendly, TargetTypes.This)
                && FilterTargetsByRange(ship, minRange: 0, maxRange: 3);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ActionsHolder.CountEnemiesTargeting(ship) * ship.PilotInfo.Cost;
        }
    }

    public class AgentTerexCyborgAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnSystemsPhaseStart += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsPhaseStart -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnSystemsPhaseStart, RollDie);
        }

        private void RollDie(object sender, EventArgs e)
        {
            DiceRollCheckSubPhase subphase = Phases.StartTemporarySubPhaseNew<DiceRollCheckSubPhase>("Agent Terex Dice Check", Triggers.FinishTrigger);

            subphase.DiceCount = 1;
            subphase.DiceKind = DiceKind.Attack;

            subphase.AfterRoll = delegate { CheckDieResult(subphase); };
            subphase.RequiredPlayer = HostShip.Owner.PlayerNo;
            subphase.Start();
        }

        private void CheckDieResult(DiceRollCheckSubPhase subphase)
        {
            GenericToken tokenToAssign = null;

            if (subphase.CurrentDiceRoll.HasResult(DieSide.Crit) || subphase.CurrentDiceRoll.HasResult(DieSide.Success))
            {
                tokenToAssign = new CalculateToken(HostShip);
            }
            else
            {
                tokenToAssign = new JamToken(HostShip, HostShip.Owner);
            }

            Messages.ShowInfo($"Agent Terex (Cyborg): {HostShip.PilotInfo.PilotName} gains {tokenToAssign.Name}");

            HostShip.Tokens.AssignToken(
                tokenToAssign,
                delegate
                {
                    (Phases.CurrentSubPhase as DiceRollCheckSubPhase).HideDiceResultMenu();
                    Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
                    Triggers.FinishTrigger();
                }
            );
        }
    }

    public class AgentTerexCyborgAction : GenericAction
    {
        public AgentTerexCyborgAction()
        {
            Name = DiceModificationName = "Agent Terex (Cyborg)";
        }

        public override void ActionTake()
        {
            if (Selection.ThisShip.Tokens.HasToken<CalculateToken>())
            {
                StartSelectionSubphase(Selection.ThisShip.Tokens.GetToken<CalculateToken>());
            }
            else if (Selection.ThisShip.Tokens.HasToken<JamToken>())
            {
                StartSelectionSubphase(Selection.ThisShip.Tokens.GetToken<JamToken>());
            }
        }

        private static void StartSelectionSubphase(GenericToken token)
        {
            AgentTerexCyborgActionSubPhase subPhase = Phases.StartTemporarySubPhaseNew<AgentTerexCyborgActionSubPhase>(
                "Agent Terex Target Selection",
                Phases.CurrentSubPhase.CallBack
            );

            subPhase.TokenToAssign = token;

            subPhase.Start();
        }

        public override int GetActionPriority()
        {
            int result = 0;

            foreach (GenericShip enemyShip in Selection.ThisShip.Owner.EnemyShips.Values)
            {
                DistanceInfo distInfo = new(Selection.ThisShip, enemyShip);

                if (distInfo.Range <= 3) result = 100;
            }

            return result;
        }
        public override bool IsActionAvailable()
        {
            return (Selection.ThisShip.Tokens.HasToken<CalculateToken>()
                || Selection.ThisShip.Tokens.HasToken<JamToken>())
                && Roster.AllShips.Values.Any(s => !Tools.IsSameShip(HostShip, s) && HostShip.GetRangeToShip(s) <= 3);
        }
    }
}

namespace SubPhases
{
    public class AgentTerexCyborgActionSubPhase : SelectShipSubPhase
    {
        public GenericToken TokenToAssign { get; set; }

        public override void Prepare()
        {
            PrepareByParameters(
                SelectTokenTarget,
                FilterTargets,
                GetTokenAiPriority,
                Selection.ThisShip.Owner.PlayerNo,
                abilityName: "Agent Terex (Cyborg)",
                description: $"Select a ship to assign {TokenToAssign.Name}",
                imageSource: new UpgradesList.SecondEdition.AgentTerexCyborg(),
                showSkipButton: false
            );
        }

        private int GetTokenAiPriority(GenericShip ship)
        {
            int result = 0;

            if (TokenToAssign is CalculateToken && ship.Owner.PlayerNo == Selection.ThisShip.Owner.PlayerNo)
            {
                result = ship.PilotInfo.Cost;
                if (!ship.Tokens.HasToken<FocusToken>()) result += 100;
            }
            else if (TokenToAssign is JamToken && ship.Owner.PlayerNo != Selection.ThisShip.Owner.PlayerNo)
            {
                result = ship.PilotInfo.Cost;
                if (ship.Tokens.HasTokenByColor(TokenColors.Green) || ship.Tokens.HasToken<BlueTargetLockToken>()) result += 100;
            }

            return result;
        }

        private bool FilterTargets(GenericShip ship)
        {
            DistanceInfo distInfo = new(Selection.ThisShip, ship);

            return distInfo.Range <= 3;
        }

        private void SelectTokenTarget()
        {
            Selection.ThisShip.Tokens.RemoveToken(TokenToAssign, AssignTokenToTarget);
        }

        private void AssignTokenToTarget()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();
            Messages.ShowInfo($"Agent Terex (Cyborg): {TokenToAssign.Name} is assigned to {TargetShip.PilotInfo.PilotName}");
            TargetShip.Tokens.AssignToken(TokenToAssign, FinishThisSubphase);
        }

        private void FinishThisSubphase()
        {
            Phases.CurrentSubPhase.CallBack();
        }
    }
}