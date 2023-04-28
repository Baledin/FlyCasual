﻿using Abilities.SecondEdition;
using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.ResistanceTransportPod
{
    public class RoseTico : ResistanceTransportPod
    {
        public RoseTico()
        {
            PilotInfo = new PilotCardInfo(
                "Rose Tico",
                3,
                26,
                isLimited: true,
                abilityType: typeof(RoseTicoResistanceTransportPodAbility),
                extraUpgradeIcon: UpgradeType.Talent
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RoseTicoResistanceTransportPodAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                () => { return true; },
                () => { return 90; },
                DiceModificationType.Reroll,
                GetRerollNumber
            );
        }

        private int GetRerollNumber()
        {
            int friendlyShipsInArc = 0;

            foreach (GenericShip friendlyShip in HostShip.Owner.Ships.Values)
            {
                if (friendlyShip.ShipId == HostShip.ShipId) continue;

                ShotInfo shotInfo = new ShotInfo(Combat.Attacker, friendlyShip, Combat.Attacker.PrimaryWeapons.First());
                if (shotInfo.InArc && Tools.IsFriendly(friendlyShip, HostShip)) friendlyShipsInArc++;
            }

            return friendlyShipsInArc;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}