using Upgrade;
using Ship;
using ActionsList;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class UrsaWrenGunner : GenericUpgrade
    {
        public UrsaWrenGunner() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ursa Wren",
                UpgradeType.Gunner,
                cost: 4,
                restriction: new FactionRestriction(Faction.Republic, Faction.Separatists),
                abilityType: typeof(Abilities.SecondEdition.UrsaWrenGunnerAbility)
            );
            NameCanonical = "ursawren-gunner";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class UrsaWrenGunnerAbility : GenericAbility
    {
        // After you acquire a lock on an enemy unit beyond range 2, if there are no friendly units within range 0-1 of the locked unit, gain 1 calculate token. 

        public override void ActivateAbility()
        {
            HostShip.OnTargetLockIsAcquired += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTargetLockIsAcquired += RegisterTrigger;
        }

        private void RegisterTrigger(ITargetLockable target)
        {
            if (target is GenericShip && BoardTools.Board.GetRangeOfShips(HostShip, target as GenericShip) > 2 && BoardTools.Board.GetShipsAtRange(target as GenericShip, new Vector2(0,1), Team.Type.Enemy).Count == 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTargetLockIsAcquired, delegate { GrantCalculate(); });
            }
        }

        private void GrantCalculate()
        {
            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": gain 1 calculate token.");
            HostShip.Tokens.AssignToken(typeof(Tokens.CalculateToken), Triggers.FinishTrigger);
        }
    }
}