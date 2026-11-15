using SubPhases;
using Tokens;
using UnityEngine;

namespace ActionsList
{
    public class ReinforceAction : GenericAction
    {
        private Direction aiBetterSide = Direction.None;

        public ReinforceAction()
        {
            Name = DiceModificationName = "Reinforce";
        }

        public override void ActionTake()
        {
            ReinforceSideSubphase decisionSubphase = (ReinforceSideSubphase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(ReinforceSideSubphase),
                Phases.CurrentSubPhase.CallBack
            );

            decisionSubphase.DescriptionShort = "Reinforce: Select a side";
            decisionSubphase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            decisionSubphase.AddDecision(
                "Fore side",
                delegate { Selection.ThisShip.Tokens.AssignToken(typeof(ReinforceForeToken), DecisionSubPhase.ConfirmDecision); },
                isCentered: true
            );

            decisionSubphase.AddDecision(
                "Aft side",
                delegate { Selection.ThisShip.Tokens.AssignToken(typeof(ReinforceAftToken), DecisionSubPhase.ConfirmDecision); },
                isCentered: true
            );

            decisionSubphase.DefaultDecisionName = (aiBetterSide == Direction.Top) ? "Fore side" : "Aft side";

            decisionSubphase.Start();
        }

        public override int GetActionPriority()
        {
            int resultFore = 25 + 30 * ActionsHolder.CountEnemiesTargeting(Selection.ThisShip, 1);
            int resultAft = 25 + 30 * ActionsHolder.CountEnemiesTargeting(Selection.ThisShip, -1);

            aiBetterSide = (resultFore >= resultAft) ? Direction.Top : Direction.Bottom;

            return Mathf.Max(resultFore, resultAft);
        }

        private class ReinforceSideSubphase : DecisionSubPhase { }
    }
}