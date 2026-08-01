using Arcs;
using BoardTools;
using System.Collections.Generic;
using System.Linq;

namespace Ship
{
    public class SectorsHolder : ArcsHolder
    {
        public List<GenericArc> Sectors { get { return Arcs.Where(a => a is ArcFront || a is ArcLeft || a is ArcRight || a is ArcRear).ToList(); } }

        public SectorsHolder(GenericShip hostShip) : base(hostShip)
        {
            Arcs = new List<GenericArc>();

            if (HostShip.ShipInfo.ArcInfo.Arcs.Any(a => a.ArcType != ArcType.None))
            {
                Arcs.AddRange(
                    new List<GenericArc>()
                    {
                        new ArcBullseye(hostShip.ShipBase),
                        new ArcFront(hostShip.ShipBase),
                        new ArcRear(hostShip.ShipBase),
                        new ArcLeft(hostShip.ShipBase),
                        new ArcRight(hostShip.ShipBase),
                        new ArcFullFront(hostShip.ShipBase),
                        new ArcFullRear(hostShip.ShipBase)
                    }
                );
            }
        }

        public Dictionary<ArcFacing, List<GenericShip>> GetEnemiesInAllSectors()
        {
            Dictionary<ArcFacing, List<GenericShip>> EnemiesInAllSectors = new();

            foreach (GenericArc sector in Sectors)
            {
                EnemiesInAllSectors.Add(sector.Facing, new List<GenericShip>());
                foreach (GenericShip enemyShip in HostShip.Owner.AnotherPlayer.Ships.Values)
                {
                    ShotInfoArc sectorInfo = new(HostShip, enemyShip, sector);
                    if (sectorInfo.InArc && sectorInfo.Range != 0)
                    {
                        EnemiesInAllSectors[sector.Facing].Add(enemyShip);
                    }
                }
            }

            return EnemiesInAllSectors;
        }

        public bool IsShipInSector(GenericShip anotherShip, ArcType arcType)
        {
            ShotInfoArc arcInfo = GetSectorInfo(anotherShip, arcType);

            if (arcInfo != null)
            {
                bool result = arcInfo.InArc;
                if (arcType == ArcType.Bullseye) HostShip.CallOnBullseyeArcCheck(anotherShip, ref result);

                return result;
            }
            else
            {
                return false;
            }
        }

        public int RangeToShipBySector(GenericShip anotherShip, ArcType arcType)
        {
            ShotInfoArc arcInfo = GetSectorInfo(anotherShip, arcType);

            if (arcInfo != null)
            {
                bool result = arcInfo.IsShotAvailable;
                if (arcType == ArcType.Bullseye) HostShip.CallOnBullseyeArcCheck(anotherShip, ref result);

                return result ? arcInfo.Range : int.MaxValue;
            }
            else
            {
                return int.MaxValue;
            }
        }

        public ShotInfoArc GetSectorInfo(GenericShip anotherShip, ArcType arcType)
        {
            GenericArc arc = Arcs.FirstOrDefault(n => n.ArcType == arcType);

            if (arc != null)
            {
                return new ShotInfoArc(HostShip, anotherShip, arc);
            }
            else
            {
                return null;
            }
        }
    }
}