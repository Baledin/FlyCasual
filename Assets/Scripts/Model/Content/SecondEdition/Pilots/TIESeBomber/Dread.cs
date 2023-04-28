﻿using ActionsList;
using Arcs;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESeBomber
    {
        public class Dread : TIESeBomber
        {
            public Dread() : base()
            {
                PilotInfo = new PilotCardInfo
                (
                    "\"Dread\"",
                    3,
                    32,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DreadPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DreadPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction action)
        {
            if (action is ReloadAction
                && GetShipsInBullseyeArc().Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AssignDepleteToShipsInBullseye);
            }
        }

        private List<GenericShip> GetShipsInBullseyeArc()
        {
            List<GenericShip> shipsInBullseye = new List<GenericShip>();

            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                if (HostShip.SectorsInfo.IsShipInSector(ship, ArcType.Bullseye)) shipsInBullseye.Add(ship);
            }

            return shipsInBullseye;
        }

        private void AssignDepleteToShipsInBullseye(object sender, EventArgs e)
        {
            List<GenericShip> sufferedShips = new List<GenericShip>(GetShipsInBullseyeArc());
            AssignDepleteToShipsInBullseyeRecursive(sufferedShips);
        }

        private void AssignDepleteToShipsInBullseyeRecursive(List<GenericShip> sufferedShips)
        {
            if (sufferedShips.Count == 0)
            {
                Triggers.FinishTrigger();
            }
            else
            {
                GenericShip sufferedShip = sufferedShips.First();
                sufferedShips.Remove(sufferedShip);

                Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: {sufferedShip.PilotInfo.PilotName} gains Deplete token");

                sufferedShip.Tokens.AssignToken
                (
                    typeof(Tokens.DepleteToken),
                    delegate { AssignDepleteToShipsInBullseyeRecursive(sufferedShips); }
                );
            }
        }
    }
}
