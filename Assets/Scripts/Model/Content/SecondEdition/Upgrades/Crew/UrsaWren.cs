using Upgrade;
using Ship;
using ActionsList;
using System;
using UnityEngine;
using Movement;
using SubPhases;

namespace UpgradesList.SecondEdition
{
    public class UrsaWren : GenericUpgrade
    {
        public UrsaWren() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ursa Wren",
                UpgradeType.Crew,
                cost: 6,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.UrsaWrenAbility)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class UrsaWrenAbility : GenericAbility
    {
        //You can maintain up to 2 locks. Each lock must be on a different object.

        //After a friendly ship at range 0-3 is locked, you may acquire a lock on an enemy ship.
        public override void ActivateAbility()
        {
            HostShip.TwoTargetLocksOnDifferentTargetsAreAllowed.Add(HostShip);
            GenericShip.OnTargetLockIsAcquiredGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.TwoTargetLocksOnDifferentTargetsAreAllowed.Remove(HostShip);
            GenericShip.OnTargetLockIsAcquiredGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip activeShip, ITargetLockable lockedShip)
        {            
            if (lockedShip is GenericShip && !Tools.IsFriendly(HostShip, activeShip) && BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(0, 3), Team.Type.Friendly).Contains(lockedShip as GenericShip))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTargetLockIsAcquired, AcquireSecondTargetLock);
            }
        }
        private void AcquireSecondTargetLock(object sender, System.EventArgs e)
        {
            var previousSelectedShip = Selection.ThisShip;
            Selection.ThisShip = HostShip;

            HostShip.ChooseTargetToAcquireTargetLock(
                delegate {
                    Selection.ThisShip = previousSelectedShip;
                    Triggers.FinishTrigger();
                },
                HostShip.PilotInfo.PilotName + ": You may acquire a lock",
                HostUpgrade
            );
        }
    }
}