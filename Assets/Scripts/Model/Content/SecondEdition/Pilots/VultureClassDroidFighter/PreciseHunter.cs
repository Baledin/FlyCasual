﻿using Arcs;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class PreciseHunter : VultureClassDroidFighter
    {
        public PreciseHunter()
        {
            PilotInfo = new PilotCardInfo(
                "Precise Hunter",
                3,
                24,
                limited: 3,
                abilityType: typeof(Abilities.SecondEdition.PreciseHunterAbility),
                pilotTitle: "Pinpoint Protocols"
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you perform an attack, if the defender is in your bullseye arc, you may reroll one blank result.
    public class PreciseHunterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Reroll,
                1,
                new List<DieSide> { DieSide.Blank }
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsDiceModificationAvailable()
        {
            return (Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker == HostShip
                && Combat.Attacker.SectorsInfo.IsShipInSector(Combat.Defender, ArcType.Bullseye));
        }

        public int GetDiceModificationAiPriority()
        {
            return 95;
        }
    }
}
