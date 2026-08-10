using Bombs;
using Movement;
using Obstacles;
using Remote;
using RulesList;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ship
{
    public partial class GenericShip
    {
        public Vector3 StartingPosition { get; private set; }

        public GenericMovement AssignedManeuver { get; private set; }
        public GenericMovement RevealedManeuver { get; set; }

        public List<GenericRemote> RemotesOverlapped = new();
        public List<GenericRemote> RemotesMovedThrough = new();

        public bool IsIgnoreObstacles;
        public bool IsIgnoreObstacleObstructionWhenAttacking;

        public bool IsLandedModel;

        public List<GenericObstacle> IgnoreObstaclesList = new();
        public List<Type> IgnoreObstacleTypes = new();

        public EventHandlerBool OnTryCanPerformRedManeuverWhileStressed;
        public EventHandlerBool OnCheckIgnoreObstaclesDuringBoost;
        public EventHandlerBool OnCheckIgnoreObstaclesDuringBarrelRoll;

        public bool IsLandedOnObstacle
        {
            get
            {
                return ObstaclesLanded.Any(o => !IgnoreObstaclesList.Contains(o));
            }

            set
            {
                if (value == false) ObstaclesLanded = new List<GenericObstacle>();
            }
        }

        public List<GenericObstacle> PreviousObstaclesLanded = new();

        private List<GenericObstacle> obstaclesLanded = new();

        public List<GenericObstacle> ObstaclesLanded
        {
            get
            {
                return obstaclesLanded;
            }

            set
            {
                PreviousObstaclesLanded = obstaclesLanded;
                obstaclesLanded = value;
            }
        }

        public bool IsHitObstacles
        {
            get
            {
                return !IsIgnoreObstacles && ObstaclesHit.Any(o => !IgnoreObstacleTypes.Contains(o.GetType())) && ObstaclesHit.Any(o => !IgnoreObstaclesList.Contains(o));
            }

            set
            {
                if (value == false) ObstaclesHit = new ();
            }
        }

        public List<GenericObstacle> ObstaclesHitProcessed = new();

        public List<GenericObstacle> ObstaclesHit = new();

        public List<GenericDeviceGameObject> MinesHit = new();

        public bool IsBumped
        {
            get { return ShipsBumped.Count != 0; }
        }

        public List<GenericShip> ShipsBumped = new();

        public List<GenericShip> ShipsMovedThrough = new();
        public List<GenericShip> ShipsBumpedOnTheEnd = new();

        public List<GenericShip> ShipsBoostedThrough = new();

        public GenericShip LastShipCollision { get; set; }

        public EventHandlerShipManeuvers OnGetManeuvers;
        public Dictionary<string, MovementComplexity> Maneuvers { get; set; }
        public AI.GenericAiTable HotacManeuverTable { get; protected set; }

        // EVENTS

        public event EventHandlerShipMovement AfterGetManeuverColorDecreaseComplexity;
        public event EventHandlerShipMovement AfterGetManeuverColorIncreaseComplexity;
        public event EventHandlerShipMovement AfterGetManeuverAvailablity;

        public static event EventHandlerShip OnReadyGetManeuvers;
        public event EventHandlerShip OnManeuverIsReadyToBeRevealed;
        public static event EventHandlerShip OnManeuverIsReadyToBeRevealedGlobal;
        public event EventHandlerShip OnManeuverIsRevealed;
        public static event EventHandlerShip OnManeuverIsRevealedGlobal;
        public static event EventHandlerShip OnManeuverIsSkippedGlobal;
        public static event EventHandlerShip OnNoManeuverWasRevealedGlobal;
        public event EventHandlerShip BeforeMovementIsExecuted;
        public event EventHandlerShip OnMovementStart;
        public event EventHandlerShip OnMovementExecuted;
        public event EventHandlerShip OnMovementFinish;
        public event EventHandlerShip OnMovementFinishSuccessfully;
        public event EventHandlerShip OnMovementFinishUnsuccessfully;
        public event EventHandlerShip OnMovementBumped;
        public static event EventHandlerShip OnMovementFinishGlobal;
        public static event EventHandlerShip OnMovementFinishSuccessfullyGlobal;
        public static event EventHandlerShip OnMovementFinishUnsuccessfullyGlobal;

        public event EventHandlerShip OnPositionIsReadyToFinish;
        public static event EventHandlerShip OnPositionIsReadyToFinishGlobal;
        public event EventHandlerShip OnPositionFinish;
        public static event EventHandlerShip OnPositionFinishGlobal;

        // TRIGGERS

        public void CallReadyToGetManeuvers()
        {
            OnReadyGetManeuvers?.Invoke(this);
        }

        public void CallManeuverIsReadyToBeRevealed(System.Action callBack)
        {
            if (Selection.ThisShip.AssignedManeuver != null && Selection.ThisShip.AssignedManeuver.IsRevealDial)
            {
                OnManeuverIsReadyToBeRevealedGlobal?.Invoke(this);
                OnManeuverIsReadyToBeRevealed?.Invoke(this);

                // Do not trigger dial reveal abilities when ionized
                if (!this.State.IsIonized)
                {
                    Triggers.ResolveTriggers(TriggerTypes.OnManeuverIsReadyToBeRevealed, callBack);
                }
                else
                {
                    callBack();
                }
            }
            else
            {
                callBack();
            }
        }

        public void CallManeuverIsRevealed(Action callBack, Action whenSkippedCallback)
        {
            if (AssignedManeuver != null) Roster.ToggleManeuverVisibility(Selection.ThisShip, true);

            if (AssignedManeuver != null && AssignedManeuver.IsRevealDial)
            {
                // Make a new copy of AssignedManeuver, so changes to it doesn't affect RevealedManeuver
                RevealedManeuver = ShipMovementScript.CopyMovement(AssignedManeuver);

                OnManeuverIsRevealed?.Invoke(this);
                OnManeuverIsRevealedGlobal?.Invoke(this);

                Triggers.ResolveTriggers(
                    TriggerTypes.OnManeuverIsRevealed,
                    delegate
                    {
                        if (!IsManeuverSkipped)
                        {
                            callBack();
                        }
                        else
                        {
                            OnManeuverIsSkippedGlobal?.Invoke(this);
                            Triggers.ResolveTriggers(TriggerTypes.OnManeuverIsSkipped, whenSkippedCallback);
                        }
                    }
                );
            }
            else // For ionized ships
            {
                OnNoManeuverWasRevealedGlobal?.Invoke(this);

                callBack();
            }
        }

        public void StartMoving(System.Action callback)
        {
            OnMovementStart?.Invoke(this);

            Triggers.ResolveTriggers(TriggerTypes.OnMovementStart, callback);
        }


        public void CallExecuteMoving(Action callback)
        {
            OnMovementExecuted?.Invoke(this);

            Triggers.ResolveTriggers(
                TriggerTypes.OnMovementExecuted,
                delegate { Selection.ThisShip.CallFinishMovement(callback); }
            );
        }

        public void CallBeforeMovementIsExecuted(Action callback)
        {
            BeforeMovementIsExecuted?.Invoke(this);

            Triggers.ResolveTriggers(
                TriggerTypes.BeforeMovementIsExecuted,
                callback
            );
        }

        public void CallOnMovementBumped(GenericShip ship)
        {
            OnMovementBumped?.Invoke(ship);
        }

        public void CallFinishMovement(Action callback)
        {
            OnMovementFinish?.Invoke(this);
            OnMovementFinishGlobal?.Invoke(this);

            // If we didn't bump, or end up off the board then we have succesfully completed our manuever.
            if (CheckSuccessOfManeuver())
            {
                OnMovementFinishSuccessfully?.Invoke(this);
                OnMovementFinishSuccessfullyGlobal?.Invoke(this);
            }
            else if (IsBumped)
            {
                OnMovementFinishUnsuccessfully?.Invoke(this);
                OnMovementFinishUnsuccessfullyGlobal?.Invoke(this);

                foreach (GenericShip ship in ShipsBumped)
                {
                    ship.CallOnMovementBumped(this);
                }
            }

            Triggers.ResolveTriggers(
                TriggerTypes.OnMovementFinish,
                delegate ()
                {
                    Roster.HideAssignedManeuverDial(this);
                    Selection.ThisShip.CallPositionIsReadyToFinish(callback);
                }
            );
        }

        public bool CheckSuccessOfManeuver()
        {
            return (AssignedManeuver.Speed == 0 || !IsBumped) && !BoardTools.Board.IsOffTheBoard(this);
        }

        public void CallPositionIsReadyToFinish(System.Action callback)
        {
            OnPositionIsReadyToFinish?.Invoke(this);
            OnPositionIsReadyToFinishGlobal?.Invoke(this);

            Triggers.ResolveTriggers(
                TriggerTypes.OnPositionIsReadyToFinish,
                delegate ()
                {
                    Selection.ThisShip.CallFinishPosition(callback);
                }
            );
        }

        public void CallFinishPosition(System.Action callback)
        {
            OnPositionFinish?.Invoke(this);
            OnPositionFinishGlobal?.Invoke(this);

            Triggers.ResolveTriggers(TriggerTypes.OnPositionFinish, callback);
        }

        // MANEUVERS

        // TODO: Rewrite
        public MovementComplexity GetColorComplexityOfManeuver(ManeuverHolder movement)
        {
            if (IonizationRule.IsIonized(this)) return movement.ColorComplexity;
            AfterGetManeuverColorDecreaseComplexity?.Invoke(this, ref movement);
            AfterGetManeuverColorIncreaseComplexity?.Invoke(this, ref movement);
            AfterGetManeuverAvailablity?.Invoke(this, ref movement);

            return movement.ColorComplexity;
        }

        public MovementComplexity GetLastManeuverColor()
        {
            return AssignedManeuver.ColorComplexity;
        }

        public ManeuverBearing GetLastManeuverBearing()
        {
            ManeuverBearing result = AssignedManeuver.Bearing;
            return result;
        }

        public Dictionary<string, MovementComplexity> GetManeuvers()
        {
            Dictionary<string, MovementComplexity> maneuvers = new(Maneuvers);

            CallReadyToGetManeuvers();
            OnGetManeuvers?.Invoke(maneuvers);

            Dictionary<string, MovementComplexity> result = new();

            foreach (KeyValuePair<string, MovementComplexity> maneuverHolder in maneuvers)
            {
                result.Add(maneuverHolder.Key, new ManeuverHolder(maneuverHolder.Key).ColorComplexity);
            }

            return result;
        }

        public List<ManeuverHolder> GetManeuverHolders()
        {
            Dictionary<string, MovementComplexity> maneuvers = new(Maneuvers);

            CallReadyToGetManeuvers();
            OnGetManeuvers?.Invoke(maneuvers);

            List<ManeuverHolder> result = new();

            foreach (KeyValuePair<string, MovementComplexity> maneuverHolder in maneuvers)
            {
                result.Add(new ManeuverHolder(maneuverHolder.Key, this));
            }

            return result;
        }

        public bool HasManeuver(string maneuverString)
        {
            bool result = false;
            if (Maneuvers.ContainsKey(maneuverString))
            {
                result = (Maneuvers[maneuverString] != MovementComplexity.None);
            }

            return result;
        }

        public bool HasManeuver(ManeuverHolder maneuverStruct)
        {
            string maneuverString = maneuverStruct.ToString();

            return HasManeuver(maneuverString);
        }

        public void SetAssignedManeuver(GenericMovement movement, bool isSilent = false)
        {
            if (movement == null)
            {
                ClearAssignedManeuver();
            }
            else
            {
                AssignedManeuver = movement;
                if (!isSilent) Roster.UpdateAssignedManeuverDial(this, movement);
            }
        }

        public void ClearAssignedManeuver()
        {
            AssignedManeuver = null;
        }

        public void Rotate180(Action callBack)
        {
            Phases.StartTemporarySubPhaseOld("Rotate ship 180°", typeof(SubPhases.KoiogranTurnSubPhase), callBack);
        }

        public void Rotate90Clockwise(Action callBack)
        {
            Phases.StartTemporarySubPhaseOld("Rotate ship 90°", typeof(SubPhases.Rotate90ClockwiseSubPhase), callBack);
        }

        public void Rotate90Counterclockwise(Action callBack)
        {
            Phases.StartTemporarySubPhaseOld("Rotate ship -90°", typeof(SubPhases.Rotate90CounterclockwiseSubPhase), callBack);
        }

        public bool CanPerformRedManeuverWhileStressed()
        {
            bool result = false;

            OnTryCanPerformRedManeuverWhileStressed?.Invoke(ref result);

            return result;
        }

        public bool IsIgnoreObstaclesDuringBoost()
        {
            bool result = false;

            OnCheckIgnoreObstaclesDuringBoost?.Invoke(ref result);

            return result;
        }

        public bool IsIgnoreObstaclesDuringBarrelRoll()
        {
            bool result = false;

            OnCheckIgnoreObstaclesDuringBarrelRoll?.Invoke(ref result);

            return result;
        }
    }
}