using Abilities.SecondEdition;
using ActionsList;
using Arcs;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class FennecShand : GenericUpgrade
    {
        public FennecShand() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Fennec Shand",
                UpgradeType.Crew,
                cost: 6,
                isLimited: true,
                charges: 2,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(FennecShandAbility),
                legalityInfo: new List<Legality>() { Legality.XWA }
            );

            NameCanonical = "fennecshand-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    // After you fully execute a maneuver, or perform a barrel roll or boost action, you may spend 1 charge. If you do, choose an enemy ship in your bullseye arc.
    // That ship gains 1 strain token, and you may acquire a lock on it.
    public class FennecShandAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += AskUseAbility;
            HostShip.OnActionIsPerformed += AskUseAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= AskUseAbility;
            HostShip.OnActionIsPerformed -= AskUseAbility;
        }

        private void AskUseAbility(GenericShip ship)
        {
            if (HostUpgrade.State.Charges > 0 && Roster.AllShips.Values.Any(s => Tools.IsAnotherTeam(HostShip, s) && IsInArc(s)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, UseAbility);
            }
        }

        private void AskUseAbility(GenericAction action)
        {
            if (HostUpgrade.State.Charges > 0 && (action is BarrelRollAction || action is BoostAction) && Roster.AllShips.Values.Any(s => Tools.IsAnotherTeam(HostShip, s) && IsInArc(s)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, UseAbility);
            }
        }

        private void UseAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                StrainAndTargetLockShip,
                callback: Triggers.FinishTrigger,
                descriptionLong: $"You may spend 1 charge. If you do, you may strain 1 ship in your bullseye arc and acquire a target lock on it."
            );
        }

        private void StrainAndTargetLockShip(object sender, EventArgs e)
        {
            HostShip.OnTargetLockIsAcquired += ApplyStrain;

            HostShip.ChooseTargetToAcquireTargetLock(
                delegate
                {
                    HostUpgrade.State.SpendCharge();
                    DecisionSubPhase.ConfirmDecision();
                },
                "Choose a target to acquire a lock and apply 1 strain.",
                HostUpgrade,
                IsInArc
            );
        }

        private void ApplyStrain(ITargetLockable target)
        {
            HostShip.OnTargetLockIsAcquired -= ApplyStrain;

            (target as GenericShip).Tokens.AssignToken(typeof(StrainToken), delegate { });
        }

        private bool IsInArc(GenericShip ship)
        {
            return HostShip.SectorsInfo.IsShipInSector(ship, ArcType.Bullseye);
        }
    }
}
