using BoardTools;
using Movement;
using Players;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI.Aggressor
{
    public static class NavigationSubSystem
    {
        private static GenericPlayer CurrentPlayer;

        // Each AI player has its own board. A board is rebuilt at the start of every planning of its owner,
        // so outside of the Planning phase it can be outdated (ships destroyed, deployed, moved by abilities)
        // and must only be used through the tolerant lookups below.
        private static readonly Dictionary<PlayerNo, VirtualBoard> VirtualBoards = new Dictionary<PlayerNo, VirtualBoard>();

        // Board of the player whose navigation is being calculated right now
        private static VirtualBoard VirtualBoard
        {
            get { return GetBoard(CurrentPlayer.PlayerNo); }
        }

        private static VirtualBoard GetBoard(PlayerNo playerNo)
        {
            if (!VirtualBoards.TryGetValue(playerNo, out VirtualBoard board))
            {
                board = new VirtualBoard();
                VirtualBoards[playerNo] = board;
            }

            return board;
        }

        private static int OrderOfActivation;

        private static NavigationResult CurrentNavigationResult;

        public static void CalculateNavigation(Action callback)
        {
            CurrentPlayer = Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer);

            // Always start from a fresh snapshot of the board
            VirtualBoards[CurrentPlayer.PlayerNo] = new VirtualBoard();

            GameManagerScript.Instance.StartCoroutine
            (
                RunGuarded(
                    StartCalculations(),
                    callback,
                    delegate { RecoverFromFailedCalculations(callback); }
                )
            );
        }

        // Runs a coroutine (including nested ones) and reports an exception instead of silently stopping,
        // because a stopped coroutine never calls its callback and the game hangs.
        // Callbacks are called outside of the guarded part: their exceptions are not errors of navigation.
        private static IEnumerator RunGuarded(IEnumerator routine, Action onFinished, Action onError)
        {
            Stack<IEnumerator> routines = new Stack<IEnumerator>();
            routines.Push(routine);

            while (routines.Count > 0)
            {
                bool hasNext = false;
                object yielded = null;
                Exception error = null;

                try
                {
                    hasNext = routines.Peek().MoveNext();
                    if (hasNext) yielded = routines.Peek().Current;
                }
                catch (Exception e)
                {
                    error = e;
                }

                if (error != null)
                {
                    Debug.LogException(error);

                    // Dispose suspended routines to run their "finally" blocks
                    while (routines.Count > 0)
                    {
                        try
                        {
                            (routines.Pop() as IDisposable)?.Dispose();
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }

                    onError();
                    yield break;
                }

                if (!hasNext)
                {
                    routines.Pop();
                }
                else if (yielded is IEnumerator nestedRoutine)
                {
                    routines.Push(nestedRoutine);
                }
                else
                {
                    yield return yielded;
                }
            }

            onFinished();
        }

        // Navigation is failed: return the board to its real state and let ships use simple maneuvers
        private static void RecoverFromFailedCalculations(Action callback)
        {
            Debug.LogError("AI navigation is failed, simple maneuvers will be used instead");

            try
            {
                RestoreRealBoard();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            // Drop partial results (planned maneuvers of this board are not verified)
            VirtualBoards[CurrentPlayer.PlayerNo] = new VirtualBoard();

            foreach (GenericShip ship in CurrentPlayer.Ships.Values)
            {
                ship.ClearAssignedManeuver();
            }

            Selection.DeselectThisShip();
            ShowCalculationsEnd();

            callback();
        }

        private static IEnumerator StartCalculations()
        {
            ShowCalculationsStart();

            SwitchEnemyShipsToSimpleVirtualPositions();
            yield return PredictAllFinalPositionsOfOwnShips();

            RestoreRealBoard();

            List<GenericShip> orderOfActivation = GenerateOrderOfActivation();

            yield return FindBestManeuversForShips(orderOfActivation);

            RestoreRealBoard();
            ShowCalculationsEnd();
        }

        private static void SwitchEnemyShipsToSimpleVirtualPositions()
        {
            foreach (GenericShip ship in CurrentPlayer.EnemyShips.Values)
            {
                PredictSimpleFinalPositionOfEnemyShip(ship);
            }
        }

        private static void PredictSimpleFinalPositionOfEnemyShip(GenericShip ship)
        {
            Selection.ThisShip = ship;

            GenericMovement savedMovement = ship.AssignedManeuver;

            // Decide what maneuvers to use as temporary
            string temporyManeuver = (ship.State.IsIonized) ? "1.F.S" : "2.F.S";
            bool isTemporaryManeuverAdded = false;
            if (!ship.HasManeuver(temporyManeuver))
            {
                isTemporaryManeuverAdded = true;
                ship.Maneuvers.Add(temporyManeuver, MovementComplexity.Easy);
            }
            GenericMovement movement = ShipMovementScript.MovementFromString(temporyManeuver);

            // Check maneuver
            ship.SetAssignedManeuver(movement, isSilent: true);
            movement.Initialize();
            movement.IsSimple = true;

            MovementPrediction prediction = new MovementPrediction(ship, movement);
            prediction.CalculateOnlyFinalPositionIgnoringCollisions();

            if (isTemporaryManeuverAdded)
            {
                ship.Maneuvers.Remove(temporyManeuver);
            }

            if (savedMovement != null)
            {
                ship.SetAssignedManeuver(savedMovement, isSilent: true);
            }
            else
            {
                ship.ClearAssignedManeuver();
            }

            VirtualBoard.SetVirtualPositionInfo(ship, prediction.FinalPositionInfo, temporyManeuver);
        }

        private static IEnumerator PredictAllFinalPositionsOfOwnShips()
        {
            foreach (GenericShip ship in CurrentPlayer.EnemyShips.Values)
            {
                VirtualBoard.SwitchToVirtualPosition(ship);
            }

            foreach (GenericShip ship in CurrentPlayer.Ships.Values)
            {
                yield return PredictFinalPosionsOfOwnShip(ship);
            }

            foreach (GenericShip ship in CurrentPlayer.EnemyShips.Values)
            {
                VirtualBoard.SwitchToRealPosition(ship);
            }
        }

        private static IEnumerator PredictFinalPosionsOfOwnShip(GenericShip ship)
        {
            Selection.ChangeActiveShip(ship);
            VirtualBoard.SwitchToRealPosition(ship);

            Dictionary<string, NavigationResult> navigationResults = new Dictionary<string, NavigationResult>();
            foreach (var maneuver in ship.GetManeuvers())
            {
                GenericMovement movement = ShipMovementScript.MovementFromString(maneuver.Key);
                ship.SetAssignedManeuver(movement, isSilent: true);
                movement.Initialize();
                movement.IsSimple = true;

                MovementPrediction prediction = new MovementPrediction(ship, movement);
                prediction.CalculateOnlyFinalPositionIgnoringCollisions();

                VirtualBoard.SetVirtualPositionInfo(ship, prediction.FinalPositionInfo, prediction.CurrentMovement.ToString());
                VirtualBoard.SwitchToVirtualPosition(ship);

                float minDistanceToEnemyShip, minDistanceToNearestEnemyInShotRange, minAngle;
                int enemiesInShotRange;
                ProcessHeavyGeometryCalculations(ship, out minDistanceToEnemyShip, out minDistanceToNearestEnemyInShotRange, out minAngle, out enemiesInShotRange);

                NavigationResult result = new NavigationResult()
                {
                    movement = prediction.CurrentMovement,
                    distanceToNearestEnemy = minDistanceToEnemyShip,
                    distanceToNearestEnemyInShotRange = minDistanceToNearestEnemyInShotRange,
                    angleToNearestEnemy = minAngle,
                    enemiesInShotRange = enemiesInShotRange,
                    isBumped = prediction.IsBumped,
                    isLandedOnObstacle = prediction.IsLandedOnAsteroid,
                    isOffTheBoard = prediction.IsOffTheBoard,
                    FinalPositionInfo = prediction.FinalPositionInfo
                };
                result.CalculatePriority(ship);

                navigationResults.Add(maneuver.Key, result);

                VirtualBoard.SwitchToRealPosition(ship);

                yield return true;
            }

            ship.ClearAssignedManeuver();
            VirtualBoard.UpdateNavigationResults(ship, navigationResults);
            Selection.DeselectThisShip();
        }

        private static List<GenericShip> GenerateOrderOfActivation()
        {
            OrderOfActivation = 0;

            List<GenericShip> orderOfActivation = new List<GenericShip>();

            List<GenericShip> AllShips = new List<GenericShip>(Roster.AllShips.Values.ToList());

            while (AllShips.Count > 0)
            {
                int lowestInitiative = AllShips.Min(n => n.State.Initiative);

                GenericShip shipToActivate = AllShips
                    .Where(n => n.State.Initiative == lowestInitiative)
                    .OrderBy(n => GetMinDistanceToEnemyShip(n))
                    .OrderByDescending(n => n.Owner.PlayerNo == Phases.PlayerWithInitiative)
                    .First();

                orderOfActivation.Add(shipToActivate);
                AllShips.Remove(shipToActivate);
            }

            if (DebugManager.DebugAiNavigation)
            {
                string orderOfActivationText = "";
                foreach (GenericShip ship in orderOfActivation)
                {
                    orderOfActivationText += (ship.ShipId + ", ");
                }
            }

            return orderOfActivation;
        }

        private static IEnumerator FindBestManeuversForShips(List<GenericShip> orderOfActivation)
        {
            while (orderOfActivation.Count > 0)
            {
                SetVirtualPositionsForShipsWithPreviousActivations(orderOfActivation);

                GenericShip ship = orderOfActivation.First();
                orderOfActivation.Remove(ship);

                if (ship.Owner.PlayerNo == CurrentPlayer.PlayerNo)
                {
                    yield return FindBestManeuver(ship);
                }
                else
                {
                    yield return PredictCollisionDetectionOfEnemyShip(ship);
                }
            }
        }

        private static IEnumerator FindBestManeuver(GenericShip ship)
        {
            VirtualShipInfo shipInfo = VirtualBoard.GetInfo(ship);

            // Nothing to choose from: the ship gets a simple maneuver when it is asked to assign one
            if (shipInfo.NavigationResults == null || shipInfo.NavigationResults.Count == 0) yield break;

            Selection.ChangeActiveShip(ship);

            int bestPriority = int.MinValue;
            KeyValuePair<string, NavigationResult> maneuverToCheck = new KeyValuePair<string, NavigationResult>();

            do
            {
                VirtualBoard.SwitchToRealPosition(ship);

                bestPriority = shipInfo.NavigationResults.Max(n => n.Value.Priority);
                maneuverToCheck = shipInfo.NavigationResults.Where(n => n.Value.Priority == bestPriority).First();

                GenericMovement movement = ShipMovementScript.MovementFromString(maneuverToCheck.Key);

                ship.SetAssignedManeuver(movement, isSilent: true);
                movement.Initialize();
                movement.IsSimple = true;

                MovementPrediction prediction = new MovementPrediction(ship, movement);
                yield return prediction.CalculateMovementPredicition();

                VirtualBoard.SetVirtualPositionInfo(ship, prediction.FinalPositionInfo, prediction.CurrentMovement.ToString());
                VirtualBoard.SwitchToVirtualPosition(ship);

                CurrentNavigationResult = new NavigationResult()
                {
                    movement = prediction.CurrentMovement,
                    isBumped = prediction.IsBumped,
                    isLandedOnObstacle = prediction.IsLandedOnAsteroid,
                    obstaclesHit = prediction.AsteroidsHit.Count,
                    isOffTheBoard = prediction.IsOffTheBoard,
                    minesHit = prediction.MinesHit.Count,
                    isOffTheBoardNextTurn = false, //!NextTurnNavigationResults.Any(n => !n.isOffTheBoard),
                    isHitAsteroidNextTurn = false, //!NextTurnNavigationResults.Any(n => n.obstaclesHit == 0),
                    FinalPositionInfo = prediction.FinalPositionInfo
                };

                foreach (GenericShip enemyShip in CurrentPlayer.EnemyShips.Values)
                {
                    VirtualBoard.SwitchToVirtualPosition(enemyShip);
                }

                if (!prediction.IsOffTheBoard)
                {
                    yield return CheckNextTurnRecursive(ship);

                    ProcessHeavyGeometryCalculations(ship, out float minDistanceToEnemyShip, out float minDistanceToNearestEnemyInShotRange, out float minAngle, out int enemiesInShotRange);

                    CurrentNavigationResult.distanceToNearestEnemy = minDistanceToEnemyShip;
                    CurrentNavigationResult.distanceToNearestEnemyInShotRange = minDistanceToNearestEnemyInShotRange;
                    CurrentNavigationResult.angleToNearestEnemy = minAngle;
                    CurrentNavigationResult.enemiesInShotRange = enemiesInShotRange;
                }

                CurrentNavigationResult.CalculatePriority(ship);

                shipInfo.NavigationResults[maneuverToCheck.Key] = CurrentNavigationResult;

                bestPriority = shipInfo.NavigationResults.Max(n => n.Value.Priority);

                VirtualBoard.SwitchToRealPosition(ship);

                maneuverToCheck = shipInfo.NavigationResults.First(n => n.Key == maneuverToCheck.Key);

                foreach (GenericShip enemyShip in CurrentPlayer.EnemyShips.Values)
                {
                    VirtualBoard.SwitchToRealPosition(enemyShip);
                }

            } while (maneuverToCheck.Value.Priority != bestPriority);

            shipInfo.SetPlannedManeuverCode(maneuverToCheck.Key, ++OrderOfActivation);
            ship.ClearAssignedManeuver();
            Selection.DeselectThisShip();
        }

        private static void ProcessHeavyGeometryCalculations(GenericShip ship, out float minDistanceToEnemyShip, out float minDistanceToNearestEnemyInShotRange, out float minAngle, out int enemiesInShotRange)
        {
            minDistanceToEnemyShip = float.MaxValue;
            minDistanceToNearestEnemyInShotRange = 0;
            minAngle = float.MaxValue;
            enemiesInShotRange = 0;
            foreach (GenericShip enemyShip in ship.Owner.EnemyShips.Values)
            {
                DistanceInfo distInfo = new DistanceInfo(ship, enemyShip);
                if (distInfo.MinDistance.DistanceReal < minDistanceToEnemyShip)
                {
                    minDistanceToEnemyShip = distInfo.MinDistance.DistanceReal;
                }

                if (ship.PrimaryWeapons.Count > 0)
                {
                    ShotInfo shotInfo = new ShotInfo(ship, enemyShip, ship.PrimaryWeapons.First());
                    if (shotInfo.IsShotAvailable)
                    {
                        enemiesInShotRange++;

                        if (minDistanceToNearestEnemyInShotRange < shotInfo.DistanceReal)
                        {
                            minDistanceToNearestEnemyInShotRange = shotInfo.DistanceReal;
                        }
                    }
                }

                Vector3 forward = ship.GetFrontFacing();
                Vector3 toEnemyShip = enemyShip.GetCenter() - ship.GetCenter();
                float angle = Mathf.Abs(Vector3.SignedAngle(forward, toEnemyShip, Vector3.down));
                if (angle < minAngle) minAngle = angle;
            }
        }

        private static void SetVirtualPositionsForShipsWithPreviousActivations(List<GenericShip> orderOfActivation)
        {
            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                if (!orderOfActivation.Contains(ship))
                {
                    VirtualBoard.SwitchToVirtualPosition(ship);
                }
            }
        }

        private static IEnumerator PredictCollisionDetectionOfEnemyShip(GenericShip ship)
        {
            Selection.ThisShip = ship;

            GenericMovement savedMovement = ship.AssignedManeuver;

            // Decide what maneuvers to use as temporary
            string temporyManeuver = (ship.State.IsIonized) ? "1.F.S" : "2.F.S";
            bool isTemporaryManeuverAdded = false;
            if (!ship.HasManeuver(temporyManeuver))
            {
                isTemporaryManeuverAdded = true;
                ship.Maneuvers.Add(temporyManeuver, MovementComplexity.Easy);
            }
            GenericMovement movement = ShipMovementScript.MovementFromString(temporyManeuver);
            MovementPrediction prediction = new MovementPrediction(ship, movement);

            // The real maneuver of the enemy ship must be returned even if prediction is failed
            try
            {
                // Check maneuver
                ship.SetAssignedManeuver(movement, isSilent: true);
                movement.Initialize();
                movement.IsSimple = true;

                yield return prediction.CalculateMovementPredicition();
            }
            finally
            {
                if (isTemporaryManeuverAdded)
                {
                    ship.Maneuvers.Remove(temporyManeuver);
                }

                if (savedMovement != null)
                {
                    ship.SetAssignedManeuver(savedMovement, isSilent: true);
                }
                else
                {
                    ship.ClearAssignedManeuver();
                }
            }

            VirtualBoard.SetVirtualPositionInfo(ship, prediction.FinalPositionInfo, temporyManeuver);
        }

        private static IEnumerator CheckNextTurnRecursive(GenericShip ship)
        {
            VirtualBoard.RemoveCollisionsExcept(ship);

            bool HasAnyManeuverWithoutOffBoardFinish = false;
            bool HasAnyManeuverWithoutAsteroidCollision = false;

            foreach (string turnManeuver in GetShortestTurnManeuvers(ship))
            {
                GenericMovement movement = ShipMovementScript.MovementFromString(turnManeuver);

                ship.SetAssignedManeuver(movement, isSilent: true);
                movement.Initialize();
                movement.IsSimple = true;

                MovementPrediction prediction = new MovementPrediction(ship, movement);
                yield return prediction.CalculateMovementPredicition();

                if (!prediction.IsOffTheBoard) HasAnyManeuverWithoutOffBoardFinish = true;
                if (!prediction.IsLandedOnAsteroid) HasAnyManeuverWithoutAsteroidCollision = true;
            }

            CurrentNavigationResult.isOffTheBoardNextTurn = !HasAnyManeuverWithoutOffBoardFinish;
            CurrentNavigationResult.isHitAsteroidNextTurn = !HasAnyManeuverWithoutAsteroidCollision;

            VirtualBoard.ReturnCollisionsExcept(ship);
        }

        private static List<string> GetShortestTurnManeuvers(GenericShip ship)
        {
            List<string> bestTurnManeuvers = new List<string>();

            ManeuverHolder bestLeftTurnManeuver = ship.GetManeuverHolders()
                .Where(n =>
                    n.Bearing == ManeuverBearing.Turn
                    && n.Direction == ManeuverDirection.Left
                )
                .OrderBy(n => n.SpeedIntUnsigned)
                .FirstOrDefault();
            if(bestLeftTurnManeuver.Bearing == ManeuverBearing.Turn)
                bestTurnManeuvers.Add(bestLeftTurnManeuver.ToString());

            ManeuverHolder bestRightTurnManeuver = ship.GetManeuverHolders()
                .Where(n =>
                    n.Bearing == ManeuverBearing.Turn
                    && n.Direction == ManeuverDirection.Right
                )
                .OrderBy(n => n.SpeedIntUnsigned)
                .FirstOrDefault();
            if (bestRightTurnManeuver.Bearing == ManeuverBearing.Turn)
                bestTurnManeuvers.Add(bestRightTurnManeuver.ToString());

            return bestTurnManeuvers;
        }

        // Requests that can come at any moment of the game.
        // They don't expect that the ship is known to the board or has a planned maneuver.

        public static GenericShip GetNextShipWithoutAssignedManeuver()
        {
            return Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).Ships.Values
                .Where(n => n.AssignedManeuver == null)
                .OrderBy(n => GetOrderToActivate(n))
                .FirstOrDefault();
        }

        public static GenericShip GetNextShipWithoutFinishedManeuver()
        {
            return Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).Ships.Values
                .Where(n => !n.IsManeuverPerformed)
                .OrderByDescending(n => n.State.Initiative == Phases.CurrentSubPhase.RequiredInitiative)
                .ThenBy(n => GetOrderToActivate(n))
                .FirstOrDefault();
        }

        // Ships that were not planned (joined the game later) are activated last
        private static int GetOrderToActivate(GenericShip ship)
        {
            return (GetBoard(ship.Owner.PlayerNo).TryGetInfo(ship, out VirtualShipInfo info) && info.OrderToActivate > 0)
                ? info.OrderToActivate
                : int.MaxValue;
        }

        private static string GetPlannedManeuverCode(GenericShip ship)
        {
            if (!GetBoard(ship.Owner.PlayerNo).TryGetInfo(ship, out VirtualShipInfo info)) return null;

            // Without an order of activation the code is only a candidate that was checked during calculations
            return (info.OrderToActivate > 0) ? info.PlannedManeuverCode : null;
        }

        public static void AssignPlannedManeuver(Action callBack)
        {
            GenericShip ship = Selection.ThisShip;

            string maneuverCode = GetPlannedManeuverCode(ship) ?? GenericAiPlayer.GetSimpleManeuver(ship);
            if (maneuverCode == null)
            {
                Debug.LogError("AI cannot find any maneuver for " + ship.PilotInfo.PilotName + " (ID:" + ship.ShipId + ")");
                maneuverCode = "2.F.S";
            }

            ShipMovementScript.SendAssignManeuverCommand(maneuverCode);
            GameManagerScript.Wait(0.2f, delegate { Selection.DeselectThisShip(); callBack(); });
        }

        // Selection of a maneuver outside of the Planning phase (deployment from a host ship, etc.)
        // Only the real current state of the board is used - virtual boards can be outdated at this moment.
        // Callback gets null if the ship has no suitable maneuvers.
        public static void FindBestManeuverOnRealBoard(GenericShip ship, Func<string, bool> filter, Action<string> callback)
        {
            string simpleManeuver = GenericAiPlayer.GetSimpleManeuver(ship, filter);
            string bestManeuver = simpleManeuver;

            GameManagerScript.Instance.StartCoroutine
            (
                RunGuarded(
                    FindBestManeuverOnRealBoardCoroutine(ship, filter, delegate (string maneuverCode) { bestManeuver = maneuverCode; }),
                    delegate { callback(bestManeuver); },
                    delegate { callback(simpleManeuver); }
                )
            );
        }

        private static IEnumerator FindBestManeuverOnRealBoardCoroutine(GenericShip ship, Func<string, bool> filter, Action<string> setBestManeuver)
        {
            GenericShip savedSelectedShip = Selection.ThisShip;
            GenericMovement savedMovement = ship.AssignedManeuver;
            ShipPositionInfo realPositionInfo = new ShipPositionInfo(ship.GetPosition(), ship.GetAngles());

            int bestPriority = int.MinValue;

            try
            {
                // Movement templates and collision detectors rely on the selected ship
                Selection.ThisShip = ship;

                foreach (KeyValuePair<string, MovementComplexity> maneuver in ship.GetManeuvers())
                {
                    if (maneuver.Value == MovementComplexity.None) continue;
                    if (filter != null && !filter(maneuver.Key)) continue;

                    GenericMovement movement = ShipMovementScript.MovementFromString(maneuver.Key);
                    ship.SetAssignedManeuver(movement, isSilent: true);
                    movement.Initialize();
                    movement.IsSimple = true;

                    MovementPrediction prediction = new MovementPrediction(ship, movement);
                    yield return prediction.CalculateMovementPredicition();

                    NavigationResult result = new NavigationResult()
                    {
                        movement = prediction.CurrentMovement,
                        isBumped = prediction.IsBumped,
                        isLandedOnObstacle = prediction.IsLandedOnAsteroid,
                        obstaclesHit = prediction.AsteroidsHit.Count,
                        isOffTheBoard = prediction.IsOffTheBoard,
                        minesHit = prediction.MinesHit.Count,
                        FinalPositionInfo = prediction.FinalPositionInfo
                    };

                    if (!prediction.IsOffTheBoard)
                    {
                        VirtualBoard.ApplyVirtualPosition(ship, prediction.FinalPositionInfo);

                        ProcessHeavyGeometryCalculations(ship, out float minDistanceToEnemyShip, out float minDistanceToNearestEnemyInShotRange, out float minAngle, out int enemiesInShotRange);

                        result.distanceToNearestEnemy = minDistanceToEnemyShip;
                        result.distanceToNearestEnemyInShotRange = minDistanceToNearestEnemyInShotRange;
                        result.angleToNearestEnemy = minAngle;
                        result.enemiesInShotRange = enemiesInShotRange;

                        VirtualBoard.ApplyRealPosition(ship, realPositionInfo);
                    }

                    result.CalculatePriority(ship);

                    if (result.Priority > bestPriority)
                    {
                        bestPriority = result.Priority;
                        setBestManeuver(maneuver.Key);
                    }
                }
            }
            finally
            {
                VirtualBoard.ApplyRealPosition(ship, realPositionInfo);

                if (savedMovement != null)
                {
                    ship.SetAssignedManeuver(savedMovement, isSilent: true);
                }
                else
                {
                    ship.ClearAssignedManeuver();
                }

                Selection.ThisShip = savedSelectedShip;
            }
        }

        // Low Priority

        private static void ShowCalculationsStart()
        {
            Roster.ToggleCalculatingStatus(Phases.CurrentSubPhase.RequiredPlayer, true);
        }

        private static void ShowCalculationsEnd()
        {
            Roster.ToggleCalculatingStatus(Phases.CurrentSubPhase.RequiredPlayer, false);
        }

        private static void RestoreRealBoard()
        {
            VirtualBoard.RestoreBoard();
        }

        private static float GetMinDistanceToEnemyShip(GenericShip ship)
        {
            float minDistanceToEnemyShip = float.MaxValue;

            foreach (GenericShip enemyShip in ship.Owner.EnemyShips.Values)
            {
                DistanceInfo distInfo = new DistanceInfo(ship, enemyShip);
                if (distInfo.MinDistance.DistanceReal < minDistanceToEnemyShip)
                {
                    minDistanceToEnemyShip = distInfo.MinDistance.DistanceReal;
                }
            }

            return minDistanceToEnemyShip;
        }
    }
}
