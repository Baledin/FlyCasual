using ActionsList;
using GameModes;
using Ship;
using SubPhases;
using System.Collections.Generic;
using System.Linq;

namespace Players
{
    public partial class AggressorAiPlayer : GenericAiPlayer
    {
        public AggressorAiPlayer() : base()
        {
            Name = "Aggressor AI";

            NickName = "Aggressor";
            Title = "Assassin Droid";
            Avatar = "UpgradesList.SecondEdition.IG88D";
        }

        public override void AssignManeuversStart()
        {
            base.AssignManeuversStart();

            if (!DebugManager.DebugStraightToCombat)
            {
                CalculateNavigation();
            }
            else
            {
                AssignManeuversRecursive();
            }
        }

        private void CalculateNavigation()
        {
            AI.Aggressor.NavigationSubSystem.CalculateNavigation(AssignManeuversRecursive);
        }

        private void AssignManeuversRecursive()
        {
            GenericShip shipWithoutManeuver = (!DebugManager.DebugStraightToCombat) ?
                AI.Aggressor.NavigationSubSystem.GetNextShipWithoutAssignedManeuver() :
                GetNextShipWithoutAssignedManeuver();

            if (shipWithoutManeuver != null)
            {
                Selection.ChangeActiveShip(shipWithoutManeuver);
                OpenDirectionsUiSilent();
            }
            else
            {
                GameMode.CurrentGameMode.ExecuteCommand(UI.GenerateNextButtonCommand());
            }
        }

        private GenericShip GetNextShipWithoutAssignedManeuver()
        {
            return Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).Ships.Values
                .FirstOrDefault(n => n.AssignedManeuver == null && !n.State.IsIonized);
        }

        private void OpenDirectionsUiSilent()
        {
            GameMode.CurrentGameMode.ExecuteCommand(
                PlanningSubPhase.GenerateSelectShipToAssignManeuver(Selection.ThisShip.ShipId)
            );
        }

        public override void AskAssignManeuver()
        {
            if (!DebugManager.DebugStraightToCombat)
            {
                AI.Aggressor.NavigationSubSystem.AssignPlannedManeuver(AssignManeuversRecursive);
            }
            else
            {
                ShipMovementScript.SendAssignManeuverCommand("2.F.S");
                AssignManeuversRecursive();
            }
        }

        public override void AssignManeuverMidgame(GenericShip ship, System.Func<string, bool> filter = null)
        {
            if (DebugManager.DebugStraightToCombat)
            {
                base.AssignManeuverMidgame(ship, filter);
                return;
            }

            AI.Aggressor.NavigationSubSystem.FindBestManeuverOnRealBoard(
                ship,
                filter,
                delegate (string maneuverCode) { SendMidgameManeuver(ship, maneuverCode); }
            );
        }

        protected override GenericShip SelectTargetForAttack()
        {
            if (DebugManager.DebugNoCombat) return null;

            return AI.Aggressor.TargetingSubSystem.SelectTargetAndWeapon(Selection.ThisShip);
        }

        protected override void PerformActionFromList(List<GenericAction> actionsList)
        {
            bool isActionTaken = false;

            List<GenericAction> availableActionsList = actionsList;

            Dictionary<GenericAction, int> actionsPriority = new();

            GenericShip ship = Selection.ThisShip;

            foreach (GenericAction action in availableActionsList)
            {
                ship.CallOnCheckActionComplexity(action, ref action.Color);
                ship.CallOnCheckActionColor(action, ref action.Color);

                int priority = action.GetActionPriority();
                ship.Ai.CallGetActionPriority(action, ref priority);

                // De-prioritize red actions unless overriden
                if (action.IsRed)
                {
                    if (ship.IsStressed && !ship.CallCanPerformActionWhileStressed(action)) continue;

                    double redActionPriorityModifier = 0.2;
                    ship.Ai.CallGetRedActionPriorityModifier(action, ref redActionPriorityModifier);
                    priority = (int)(priority * redActionPriorityModifier);
                }

                actionsPriority.Add(action, priority);
            }

            actionsPriority = actionsPriority.OrderByDescending(n => n.Value)
                .ToDictionary(n => n.Key, n => n.Value);

            if (actionsPriority.Count > 0)
            {
                KeyValuePair<GenericAction, int> prioritizedActions = actionsPriority.First();

                if (prioritizedActions.Value > 0)
                {
                    isActionTaken = true;

                    JSONObject parameters = new();
                    parameters.AddField("name", prioritizedActions.Key.Name);
                    GameController.SendCommand(
                        GameCommandTypes.Decision,
                        Phases.CurrentSubPhase.GetType(),
                        Phases.CurrentSubPhase.ID,
                        parameters.ToString()
                    );
                }
            }

            if (!isActionTaken)
            {
                GameMode.CurrentGameMode.ExecuteCommand(UI.GenerateSkipButtonCommand());
            }
        }

        public override void SetupShip()
        {
            Roster.HighlightPlayer(PlayerNo);

            AI.Aggressor.DeploymentSubSystem.SetupShip();
        }

        public override void PerformManeuver()
        {
            Roster.HighlightPlayer(PlayerNo);

            GenericShip nextShip = (!DebugManager.DebugStraightToCombat) ?
                AI.Aggressor.NavigationSubSystem.GetNextShipWithoutFinishedManeuver() :
                GetNextShipWithoutFinishedManeuver();

            if (nextShip != null)
            {
                Selection.ChangeActiveShip("ShipId:" + nextShip.ShipId);
                ActivateShip(nextShip);
            }
            else
            {
                Phases.Next();
            }
        }

        private static GenericShip GetNextShipWithoutFinishedManeuver()
        {
            return Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).Ships.Values
                .FirstOrDefault(n => !n.IsManeuverPerformed);
        }
    }
}
