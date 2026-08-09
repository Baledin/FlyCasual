using Bombs;
using Obstacles;
using Remote;
using Ship;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Movement
{

    public class MovementPrediction
    {
        public GenericShip Ship;
        public GenericMovement CurrentMovement;
        public ShipPositionInfo FinalPositionInfo { get; private set; }
        public ShipPositionInfo FinalPositionBeforeRotationInfo { get; private set; }
        public float SuccessfullMovementProgress { get; private set; }
        public bool IsOffTheBoard;
        public bool IsBumped { get { return ShipsBumped.Count != 0; } }
        public bool IsBumpedSameTeam { get { return ShipsBumped.Any(v => Tools.IsSameTeam(Ship, v)); } }
        public bool IsBumpedAnotherTeam { get { return ShipsBumped.Any(v => Tools.IsAnotherTeam(Ship, v)); } }
        public List<GenericShip> ShipsBumped = new();
        public List<GenericShip> ShipsBumpedOnTheEnd = new();
        public List<GenericRemote> RemotesOverlapped = new();
        public List<GenericRemote> RemotesMovedThrough = new();
        public List<GenericShip> ShipsMovedThrough = new();
        public List<GenericObstacle> AsteroidsHit = new();
        public List<GenericDeviceGameObject> MinesHit = new();
        public bool IsLandedOnAsteroid { get { return LandedOnObstacles.Count > 0; } }
        public List<GenericObstacle> LandedOnObstacles = new();

        private GameObject[] GeneratedShipStands;

        public MovementPrediction(GenericShip ship, GenericMovement movement)
        {
            Ship = ship;
            CurrentMovement = movement;
        }

        public IEnumerator CalculateMovementPredicition()
        {
            DisableCollisionDetectionAtCurrentPosition();
            GenerateShipStands();
            yield return UpdateColisionDetection();
            EnableCollisionDetectionAtCurrentPosition();
            PerformCleanup();
        }

        private void DisableCollisionDetectionAtCurrentPosition()
        {
            Ship.ToggleColliders(false);
        }

        private void EnableCollisionDetectionAtCurrentPosition()
        {
            Ship.ToggleColliders(true);
        }

        private void GenerateShipStands()
        {
            GeneratedShipStands = CurrentMovement.PlanMovement();

            if (CurrentMovement.HasRotationInTheEnd)
            {
                RotateAroundCenter(GeneratedShipStands.Last(), CurrentMovement.RotationEndDegrees);
            }
        }

        private void RotateAroundCenter(GameObject shipStand, int degrees)
        {
            Vector3 centerOfTempBase = shipStand.transform.TransformPoint(new Vector3(0, 0, -Ship.ShipBase.HALF_OF_SHIPSTAND_SIZE));
            shipStand.transform.RotateAround(centerOfTempBase, new Vector3(0, 1, 0), degrees);
        }

        private IEnumerator UpdateColisionDetection()
        {
            yield return new WaitForFixedUpdate();
            GetResults();
        }

        private void GetResults()
        {
            bool finalPositionFound = false;
            ObstaclesStayDetector lastShipBumpDetector = null;

            for (int i = GeneratedShipStands.Length - 1; i >= 0; i--)
            {
                ObstaclesStayDetector obstacleStayDetector = GeneratedShipStands[i].GetComponentInChildren<ObstaclesStayDetector>();
                ObstaclesHitsDetector obstacleHitsDetector = GeneratedShipStands[i].GetComponentInChildren<ObstaclesHitsDetector>();

                if (!finalPositionFound)
                {
                    if (obstacleStayDetector.OverlapsShip)
                    {
                        // Save information in which ships we are bumped
                        lastShipBumpDetector = obstacleStayDetector;
                        if (ShipsBumpedOnTheEnd.Count == 0) ShipsBumpedOnTheEnd.AddRange(obstacleStayDetector.OverlappedShips);
                    }
                    else
                    {
                        finalPositionFound = true;
                        SuccessfullMovementProgress = (float)(i) / (GeneratedShipStands.Length - 1);

                        ProcessBumpedShips(lastShipBumpDetector);

                        ProcessFinalPosition(i);
                        ProcessOffTheBoard(obstacleStayDetector);
                        ProcessObstaclesLanded(obstacleStayDetector);
                        ProcessRemotesOverlaps(obstacleStayDetector);
                        ProcessObstaclesHit(obstacleStayDetector);
                        ProcessMines(obstacleStayDetector);
                    }
                }
                else
                {
                    ProcessStandOnPath(obstacleHitsDetector);
                }
            }
        }

        private void ProcessFinalPosition(int index)
        {
            SaveFinalPositionInfo(GeneratedShipStands[index]);

            if (CurrentMovement.HasRotationInTheEnd && SuccessfullMovementProgress == 1)
            {
                RotateAroundCenter(GeneratedShipStands[index], -CurrentMovement.RotationEndDegrees);
                SaveFinalPositionInfoBeforeRotation(GeneratedShipStands[index]);
                RotateAroundCenter(GeneratedShipStands[index], CurrentMovement.RotationEndDegrees);
            }
            else
            {
                SaveFinalPositionInfoBeforeRotation(GeneratedShipStands[index]);
            }
        }

        private void SaveFinalPositionInfo(GameObject shipStand)
        {
            FinalPositionInfo = new ShipPositionInfo
            (
                shipStand.transform.position,
                shipStand.transform.eulerAngles
            );

            CurrentMovement.FinalPositionInfo = FinalPositionInfo;
        }

        private void SaveFinalPositionInfoBeforeRotation(GameObject shipStand)
        {
            FinalPositionBeforeRotationInfo = new ShipPositionInfo
            (
                shipStand.transform.position,
                shipStand.transform.eulerAngles
            );

            CurrentMovement.FinalPositionInfoBeforeRotation = FinalPositionBeforeRotationInfo;
        }

        private void ProcessBumpedShips(ObstaclesStayDetector lastShipBumpDetector)
        {
            if (lastShipBumpDetector != null)
            {
                foreach (GenericShip overlappedShip in lastShipBumpDetector.OverlappedShips)
                {
                    if (!ShipsBumped.Contains(overlappedShip))
                    {
                        ShipsBumped.Add(overlappedShip);
                    }
                }
            }
        }

        private void ProcessOffTheBoard(ObstaclesStayDetector obstacleStayDetector)
        {
            IsOffTheBoard = obstacleStayDetector.OffTheBoard;
        }

        private void ProcessObstaclesLanded(ObstaclesStayDetector obstacleStayDetector)
        {
            LandedOnObstacles = new List<GenericObstacle>(obstacleStayDetector.OverlappedAsteroids);
        }

        private void ProcessRemotesOverlaps(ObstaclesStayDetector obstacleStayDetector)
        {
            foreach (GenericRemote overlappedRemote in obstacleStayDetector.OverlappedRemotes)
            {
                if (!RemotesOverlapped.Contains(overlappedRemote))
                {
                    RemotesOverlapped.Add(overlappedRemote);
                }
            }
        }

        private void ProcessObstaclesHit(ObstaclesStayDetector obstacleStayDetector)
        {
            foreach (GenericObstacle asteroidHit in obstacleStayDetector.OverlappedAsteroids)
            {
                if (!AsteroidsHit.Contains(asteroidHit))
                {
                    AsteroidsHit.Add(asteroidHit);
                }
            }
        }

        private void ProcessMines(ObstaclesStayDetector obstacleStayDetector)
        {
            foreach (Collider mineHit in obstacleStayDetector.OverlappedMines)
            {
                GenericDeviceGameObject MineObject = mineHit.transform.parent.GetComponent<GenericDeviceGameObject>();
                if (!MinesHit.Contains(MineObject))
                {
                    MinesHit.Add(MineObject);
                }
            }
        }

        private void ProcessStandOnPath(ObstaclesHitsDetector obstacleHitsDetector)
        {
            foreach (GenericObstacle asteroidHit in obstacleHitsDetector.OverlappedAsteroids)
            {
                if (!AsteroidsHit.Contains(asteroidHit))
                {
                    AsteroidsHit.Add(asteroidHit);
                }
            }

            foreach (Collider mineHit in obstacleHitsDetector.OverlappedMines)
            {
                GenericDeviceGameObject MineObject = mineHit.transform.parent.GetComponent<GenericDeviceGameObject>();
                if (!MinesHit.Contains(MineObject))
                {
                    MinesHit.Add(MineObject);
                }
            }

            foreach (GenericRemote remoteMovedThrough in obstacleHitsDetector.RemotesMovedThrough)
            {
                if (!RemotesMovedThrough.Contains(remoteMovedThrough))
                {
                    RemotesMovedThrough.Add(remoteMovedThrough);
                }
            }

            foreach (GenericShip shipsMovedThrough in obstacleHitsDetector.ShipsMovedThrough)
            {
                if (!ShipsMovedThrough.Contains(shipsMovedThrough))
                {
                    ShipsMovedThrough.Add(shipsMovedThrough);
                }
            }
        }

        private void PerformCleanup()
        {
            if (!DebugManager.DebugMovementDestroyTempBasesLater)
            {
                DestroyGeneratedShipStands();
            }
            else
            {
                GameManagerScript.Wait(2, DestroyGeneratedShipStands);
            }
        }

        private void DestroyGeneratedShipStands()
        {
            foreach (GameObject shipStand in GeneratedShipStands)
            {
                GameObject.Destroy(shipStand);
            }
        }


        // Calculation of only final position
        public void CalculateOnlyFinalPositionIgnoringCollisions()
        {
            DisableCollisionDetectionAtCurrentPosition();
            GenerateFinalShipStand();
            // TODO: GET FINAL POSITION
            EnableCollisionDetectionAtCurrentPosition();
            PerformCleanup();
        }

        private void GenerateFinalShipStand()
        {
            GeneratedShipStands = CurrentMovement.PlanFinalPosition();

            SaveFinalPositionInfoBeforeRotation(GeneratedShipStands.Last());

            if (CurrentMovement.HasRotationInTheEnd)
            {
                Vector3 centerOfTempBase = GeneratedShipStands.Last().transform.TransformPoint(new Vector3(0, 0, -Ship.ShipBase.HALF_OF_SHIPSTAND_SIZE));
                GeneratedShipStands.Last().transform.RotateAround(centerOfTempBase, new Vector3(0, 1, 0), CurrentMovement.RotationEndDegrees);
            }

            SaveFinalPositionInfo(GeneratedShipStands.Last());
        }
    }
}