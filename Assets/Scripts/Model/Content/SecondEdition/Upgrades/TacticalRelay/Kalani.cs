﻿using Actions;
using ActionsList;
using Arcs;
using BoardTools;
using Ship;
using SubPhases;
using System;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Kalani : GenericUpgrade
    {
        public Kalani() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Kalani",
                UpgradeType.TacticalRelay,
                cost: 3,
                isLimited: true,
                isSolitary: true,
                addAction: new ActionInfo(typeof(CalculateAction), ActionColor.White, this),
                charges: 3,
                regensChargesCount: 3,
                restriction: new FactionRestriction(Faction.Separatists),
                abilityType: typeof(Abilities.SecondEdition.KalaniAbility)
            );

            Avatar = new AvatarInfo(
                Faction.Separatists,
                new Vector2(250, 1)
            );

            ImageUrl = "https://infinitearenas.com/xw2/images/upgrades/kalani.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class KalaniAbility : GenericAbility
    {
        private GenericShip LastMovedShip;

        public override void ActivateAbility()
        {
            GenericShip.OnMovementFinishGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnMovementFinishGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (IsInFriendlyBullseyeInRange(ship))
            {
                LastMovedShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToChooseOwnShip);
            }
        }

        private bool IsInFriendlyBullseyeInRange(GenericShip enemyShip)
        {
            if (HostUpgrade.State.Charges == 0) return false;
            if (enemyShip.Owner.PlayerNo == HostShip.Owner.PlayerNo) return false;

            foreach (GenericShip friendlyShip in HostShip.Owner.Ships.Values)
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, friendlyShip);
                if (distInfo.Range <= 3 && friendlyShip.SectorsInfo.IsShipInSector(enemyShip, ArcType.Bullseye))
                {
                    return true;
                }
            }

            return false;
        }

        private void AskToChooseOwnShip(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                GiveLock,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: "Kalani",
                description: "Choose a ship and spend 1 charge - it acquires a lock on enemy ship, then gains 1 stress token",
                imageSource: HostUpgrade
            );
        }

        private void GiveLock()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            HostUpgrade.State.SpendCharge();
            ActionsHolder.AcquireTargetLock(TargetShip, LastMovedShip, AssignStress, AssignStress);

            Selection.ChangeActiveShip(LastMovedShip);
        }

        private void AssignStress()
        {
            TargetShip.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
        }

        private bool FilterTargets(GenericShip ship)
        {
            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            return distInfo.Range <= 3
                && ship.Owner.PlayerNo == HostShip.Owner.PlayerNo
                && ship.SectorsInfo.IsShipInSector(LastMovedShip, ArcType.Bullseye);
        }

        private int GetAiPriority(GenericShip ship)
        {
            int result = 1000 + ship.PilotInfo.Cost;
            if (ship.Tokens.HasToken<BlueTargetLockToken>('*')) result -= 200;
            if (ship.IsStressed) result -= 500;
            return result;
        }
    }
}