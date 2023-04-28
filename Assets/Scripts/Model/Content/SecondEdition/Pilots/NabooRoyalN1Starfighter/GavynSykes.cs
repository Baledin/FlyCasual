﻿using Abilities.SecondEdition;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NabooRoyalN1Starfighter
    {
        public class GavynSykes : NabooRoyalN1Starfighter
        {
            public GavynSykes() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Gavyn Sykes",
                    3,
                    33,
                    isLimited: true,
                    abilityText: "When you defend or perform a primary attack, if the maneuver you revealed is greater than the enemy ship’s maneuver, you may reroll your blank results.",
                    abilityType: typeof(GavynSykesAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GavynSykesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Gavyn Sykes",
                IsDiceModificationAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                int.MaxValue,
                new List<DieSide>() { DieSide.Blank },
                timing: DiceModificationTimingType.AfterRolled
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsDiceModificationAvailable()
        {
            if (HostShip.RevealedManeuver == null || Combat.Defender.RevealedManeuver == null) return false;

            return ((Combat.AttackStep == CombatStep.Attack && Combat.Attacker == HostShip && HostShip.RevealedManeuver.Speed > Combat.Defender.RevealedManeuver.Speed)
                || (Combat.AttackStep == CombatStep.Defence && Combat.Defender == HostShip && HostShip.RevealedManeuver.Speed > Combat.Attacker.RevealedManeuver.Speed)) && Combat.CurrentDiceRoll.Blanks > 0;
        }

        private int GetAiPriority()
        {
            return 95;
        }
    }
}
