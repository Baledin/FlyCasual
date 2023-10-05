﻿using System.Collections.Generic;
using System;
using Upgrade;
using Content;
using Ship;
using BoardTools;
using SubPhases;
using System.Linq;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class CorranHorn : T65XWing
        {
            public CorranHorn() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Corran Horn",
                    5,
                    46,
                    pilotTitle: "Rogue Nine",
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CorranHornXWingAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                PilotNameCanonical = "corranhorn-t65xwing";

                ModelInfo.SkinName = "Green";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CorranHornXWingAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (ActionsHolder.HasTargetLockOn(HostShip, Combat.Defender)
                && AnyFriendlyShipsLockedYou())
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskToSelectShipForCorranHornAbility);
            }
        }

        private bool AnyFriendlyShipsLockedYou()
        {
            foreach (GenericShip friendlyShip in HostShip.Owner.Ships.Values)
            {
                if (Tools.IsSameShip(HostShip, friendlyShip)) continue;

                if (!Tools.IsFriendly(HostShip, friendlyShip)) continue;

                if (ActionsHolder.HasTargetLockOn(friendlyShip, HostShip)) return true;
            }

            return false;
        }

        private void AskToSelectShipForCorranHornAbility(object sender, EventArgs e)
        {
            SelectTargetForAbility
            (
                TransferLock,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostShip.PilotInfo.PilotName,
                description: "Choose a ship that will transfer it's lock to defender",
                imageSource: HostShip
            );
        }

        private void TransferLock()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            char lockLetter = ActionsHolder.GetTargetLocksLetterPairs(TargetShip, HostShip).First();
            ActionsHolder.ReassignTargetLockToken(lockLetter, HostShip, Combat.Defender, Triggers.FinishTrigger);
        }

        private bool FilterTargets(GenericShip ship)
        {
            return ActionsHolder.HasTargetLockOn(ship, HostShip);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost * HasDefenderInRange(ship);
        }

        private int HasDefenderInRange(GenericShip ship)
        {
            if (ActionsHolder.HasTargetLockOn(ship, Combat.Defender)) return 0;

            ShotInfo shotInfo = new ShotInfo(ship, Combat.Defender, ship.PrimaryWeapons);
            return (shotInfo.IsShotAvailable) ? 1 : 0;
        }
    }
}
