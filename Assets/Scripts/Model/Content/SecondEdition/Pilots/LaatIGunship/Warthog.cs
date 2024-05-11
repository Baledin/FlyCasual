﻿using MainPhases;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ship
{
    namespace SecondEdition.LaatIGunship
    {
        public class Warthog : LaatIGunship
        {
            public Warthog() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Warthog\"",
                    3,
                    51,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.WarthogAbility)
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you or a friendly non-limited ship at range 0-2 are destroyed during the Engagement Phase, 
    //that ship is not removed until the end of that phase.
    public class WarthogAbility : GenericAbility
    {
        private Queue<GenericShip> ShipsToRemoveLater = new Queue<GenericShip>();

        public override void ActivateAbility()
        {
            GenericShip.OnCheckPreventDestructionGlobal += ActivateAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnCheckPreventDestructionGlobal -= ActivateAbility;
        }

        private void ActivateAbility(GenericShip ship, ref bool preventDestruction)
        {
            if (!(Phases.CurrentPhase is CombatPhase))
                return;

            if (ship == HostShip || 
                (Tools.IsFriendly(ship, HostShip) && !ship.PilotInfo.IsLimited && HostShip.GetRangeToShip(ship) <= 2))
            {
                preventDestruction = true;
                Messages.ShowInfo(HostName + ": " + ship.PilotInfo.PilotName + " is not removed until the end of the Engagement Phase");

                if (!ShipsToRemoveLater.Any())
                    Phases.Events.OnCombatPhaseEnd_NoTriggers += RegisterTrigger;

                if (!ShipsToRemoveLater.Contains(ship))
                    ShipsToRemoveLater.Enqueue(ship);
            }
        }

        public void RegisterTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseEnd, DestroyShips);
        }

        private void DestroyShips(object sender, EventArgs e)
        {
            Phases.Events.OnCombatPhaseEnd_NoTriggers -= RegisterTrigger;
            
            DestroyShipsRecursively();
        }

        private void DestroyShipsRecursively()
        {
            if (ShipsToRemoveLater.Any())
            {
                var ship = ShipsToRemoveLater.Dequeue();
                Selection.ChangeActiveShip(ship);
                ship.DestroyShipForced(DestroyShipsRecursively);
            }
            else
                Triggers.FinishTrigger();
        }
    }
}