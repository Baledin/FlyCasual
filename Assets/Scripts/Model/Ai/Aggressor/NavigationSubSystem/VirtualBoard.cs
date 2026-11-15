using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI.Aggressor
{
    public class VirtualShipInfo
    {
        public GenericShip Ship { get; private set; }
        public ShipPositionInfo RealPositionInfo { get; private set; }
        public ShipPositionInfo VirtualPositionInfo { get; private set; }
        public string PlannedManeuverCode { get; set; }
        public Dictionary<string, NavigationResult> NavigationResults { get; private set; }
        public int OrderToActivate { get; set; }

        private bool SimpleManeuverPredictionIsReady;
        private bool AllFinalPositionsAreKnown { get { return NavigationResults != null; } }

        private bool VirtualPositionWithCollisionsIsReady;

        public VirtualShipInfo(GenericShip ship)
        {
            Ship = ship;
            RealPositionInfo = VirtualPositionInfo = new ShipPositionInfo(ship.GetPosition(), ship.GetAngles());
        }

        public void UpdateSimpleManeuverPrediction(ShipPositionInfo virtualPositionInfo, string maneuverCode)
        {
            VirtualPositionInfo = virtualPositionInfo;
            PlannedManeuverCode = maneuverCode;
            SimpleManeuverPredictionIsReady = true;
        }

        public void UpdateNavigationResults(Dictionary<string, NavigationResult> navigationResults)
        {
            NavigationResults = navigationResults;
        }

        public void Clear(ShipPositionInfo positionInfo)
        {
            RealPositionInfo = VirtualPositionInfo = positionInfo;

            SimpleManeuverPredictionIsReady = false;
            NavigationResults = null;
            VirtualPositionWithCollisionsIsReady = false;
        }

        public bool RequiresFinalPositionPrediction()
        {
            return !SimpleManeuverPredictionIsReady && !AllFinalPositionsAreKnown;
        }

        public bool RequiresManeuverAssignment()
        {
            return PlannedManeuverCode == null;
        }

        public void SetPlannedManeuverCode(string maneuverCode, int order)
        {
            PlannedManeuverCode = maneuverCode;
            OrderToActivate = order;
        }

        public bool RequiresCollisionPrediction()
        {
            return VirtualPositionWithCollisionsIsReady == false;
        }
    }

    public class VirtualBoard
    {
        public Dictionary<GenericShip, VirtualShipInfo> Ships;

        // A board is a snapshot of the ships that are in play at the moment it is created
        public VirtualBoard()
        {
            Ships = new Dictionary<GenericShip, VirtualShipInfo>();
            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                Ships.Add(ship, new VirtualShipInfo(ship));
            }
        }

        // Ships can join the game after the snapshot is taken (deployed from a host ship, returned from reserve),
        // so an unknown ship is registered at its current position instead of causing an exception
        public VirtualShipInfo GetInfo(GenericShip ship)
        {
            if (!Ships.TryGetValue(ship, out VirtualShipInfo info))
            {
                info = new VirtualShipInfo(ship);
                Ships.Add(ship, info);
            }

            return info;
        }

        public bool TryGetInfo(GenericShip ship, out VirtualShipInfo info)
        {
            info = null;
            return ship != null && Ships.TryGetValue(ship, out info);
        }

        public void SetVirtualPositionInfo(GenericShip ship, ShipPositionInfo virtualPositionInfo, string maneuverCode)
        {
            GetInfo(ship).UpdateSimpleManeuverPrediction(virtualPositionInfo, maneuverCode);
        }

        public void UpdatePositionInfo(GenericShip ship)
        {
            GetInfo(ship).Clear(new ShipPositionInfo(ship.GetPosition(), ship.GetAngles()));
        }

        public void SwitchToVirtualPosition(GenericShip ship)
        {
            ApplyVirtualPosition(ship, GetInfo(ship).VirtualPositionInfo);
        }

        public void SwitchToRealPosition(GenericShip ship)
        {
            ApplyRealPosition(ship, GetInfo(ship).RealPositionInfo);
        }

        // Moves only the logical position and the collider of the ship, the visible model stays where it is
        public static void ApplyVirtualPosition(GenericShip ship, ShipPositionInfo virtualPositionInfo)
        {
            if (!DebugManager.DebugMovementShowPlanning)
            {
                ShipPositionInfo savedModelPosition = new ShipPositionInfo(ship.GetShipAllPartsTransform().position, ship.GetShipAllPartsTransform().eulerAngles);
                ship.SetPositionInfo(virtualPositionInfo);
                ship.GetShipAllPartsTransform().position = savedModelPosition.Position;
                ship.GetShipAllPartsTransform().eulerAngles = savedModelPosition.Angles;
                ship.GetShipAllPartsTransform().Find("ShipBase/ShipBaseCollider").position = virtualPositionInfo.Position;
                ship.GetShipAllPartsTransform().Find("ShipBase/ShipBaseCollider").localPosition += new Vector3(0, 0.150289f, 1.156069f);
                ship.GetShipAllPartsTransform().Find("ShipBase/ShipBaseCollider").eulerAngles = virtualPositionInfo.Angles;
            }
            else
            {
                ship.SetPositionInfo(virtualPositionInfo);
            }
        }

        // Returns the logical position and the collider of the ship back to its visible model
        public static void ApplyRealPosition(GenericShip ship, ShipPositionInfo realPositionInfo)
        {
            if (!DebugManager.DebugMovementShowPlanning)
            {
                ShipPositionInfo savedModelPosition = new ShipPositionInfo(ship.GetShipAllPartsTransform().position, ship.GetShipAllPartsTransform().eulerAngles);
                ship.SetPositionInfo(realPositionInfo);
                ship.GetShipAllPartsTransform().position = savedModelPosition.Position;
                ship.GetShipAllPartsTransform().eulerAngles = savedModelPosition.Angles;
                ship.GetShipAllPartsTransform().Find("ShipBase/ShipBaseCollider").position = savedModelPosition.Position;
                ship.GetShipAllPartsTransform().Find("ShipBase/ShipBaseCollider").eulerAngles = savedModelPosition.Angles;
            }
            else
            {
                ship.SetPositionInfo(realPositionInfo);
            }
        }

        public void RestoreBoard()
        {
            foreach (GenericShip ship in Ships.Keys)
            {
                SwitchToRealPosition(ship);
            }
        }

        public void RemoveCollisionsExcept(GenericShip exceptShip)
        {
            foreach (GenericShip ship in Ships.Keys)
            {
                if (ship == exceptShip) continue;

                Vector3 savedModelPosition = ship.GetShipAllPartsTransform().position;
                
                ship.SetPosition(ship.GetPosition() - new Vector3(0, -100, 0));
                ship.GetShipAllPartsTransform().position = savedModelPosition;
            }
        }

        public void ReturnCollisionsExcept(GenericShip exceptShip)
        {
            foreach (GenericShip ship in Ships.Keys)
            {
                if (ship == exceptShip) continue;

                Vector3 savedModelPosition = ship.GetShipAllPartsTransform().position;

                ship.SetPosition(ship.GetPosition() - new Vector3(0, +100, 0));
                ship.GetShipAllPartsTransform().position = savedModelPosition;
            }
        }

        public bool RequiresCollisionPrediction(GenericShip ship)
        {
            return GetInfo(ship).RequiresCollisionPrediction();
        }

        public bool RequiresFinalPositionPrediction(GenericShip ship)
        {
            return GetInfo(ship).RequiresFinalPositionPrediction();
        }

        public bool RequiresManeuverAssignment(GenericShip ship)
        {
            return GetInfo(ship).RequiresManeuverAssignment();
        }

        public void UpdateNavigationResults(GenericShip ship, Dictionary<string, NavigationResult> navigationResults)
        {
            GetInfo(ship).UpdateNavigationResults(navigationResults);
        }
    }
}
