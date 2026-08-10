using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class AhsokaTanoRebel : GenericUpgrade
    {
        public AhsokaTanoRebel() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ahsoka Tano",
                UpgradeType.Crew,
                cost: 10,
                isLimited: true,
                addForce: 1,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.AhsokaTanoRebelCrewAbility),
                legalityInfo: new List<Legality>() { Legality.XWA }
            );

            NameCanonical = "ahsokatano-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AhsokaTanoRebelCrewAbility : GenericAbility
    {
        // Before you activate, you may spend 1 force to ignore obstacles until the end of this phase.
        // If you do, and you move through an obstacle, you may acquire a lock on an enemy ship in your front arc at range 1.

        bool originalIgnoreObstaclesValue;

        public override void ActivateAbility()
        {
            HostShip.OnActivationPhaseStart += AskUseAbility;
            HostShip.OnCheckForceRecurring += SetForceRecurring;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActivationPhaseStart -= AskUseAbility;
            HostShip.OnCheckForceRecurring -= SetForceRecurring;
        }

        private void AskUseAbility(GenericShip ship)
        {
            if (HostShip.State.Force > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementActivationStart, AskUseAbility);
            }
        }

        private void SetForceRecurring(ref bool isRecurring)
        {
            isRecurring = true;
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                IgnoreObstacles,
                callback: Triggers.FinishTrigger,
                descriptionLong: $"Spend 1 force to ignore obstacles? If you do and you move through any obstacles, you may you may acquire a lock at range 1 in your front arc."
            );
        }

        private void IgnoreObstacles(object sender, EventArgs e)
        {
            originalIgnoreObstaclesValue = HostShip.IsIgnoreObstacles;
            HostShip.IsIgnoreObstacles = true;

            HostShip.OnMovementFinish += CheckTargetLock;
            Phases.Events.OnActivationPhaseEnd_NoTriggers += RestoreIgnoreObstacles;

            HostShip.State.SpendForce(1, DecisionSubPhase.ConfirmDecision);
        }

        private void CheckTargetLock(GenericShip ship)
        {
            HostShip.OnMovementFinish -= CheckTargetLock;

            if (HostShip.ObstaclesHit.Any() && Roster.AllShips.Values.Where(s => IsEnemyShipInArcAtRangeOne(s)).Any())
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementActivationFinish, AcquireTargetLock);
            }
        }

        private void AcquireTargetLock(object sender, EventArgs e)
        {
            HostShip.ChooseTargetToAcquireTargetLock(
                Triggers.FinishTrigger,
                "You may acquire a lock",
                HostUpgrade,
                shipFilter: IsEnemyShipInArcAtRangeOne
            );
        }

        private bool IsEnemyShipInArcAtRangeOne(GenericShip ship)
        {
            return Tools.IsAnotherTeam(HostShip, ship) &&
                HostShip.GetRangeToShip(ship) == 1 &&
                HostShip.SectorsInfo.IsShipInSector(ship, Arcs.ArcType.Front);
        }

        private void RestoreIgnoreObstacles()
        {
            Phases.Events.OnActivationPhaseEnd_NoTriggers -= RestoreIgnoreObstacles;
            HostShip.IsIgnoreObstacles = originalIgnoreObstaclesValue;
        }
    }
}