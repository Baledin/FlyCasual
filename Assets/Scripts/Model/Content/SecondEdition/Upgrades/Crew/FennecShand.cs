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
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskUseAbility);
            }
        }

        private void AskUseAbility(GenericAction action)
        {
            if (HostUpgrade.State.Charges > 0 && (action is BarrelRollAction || action is BoostAction) && Roster.AllShips.Values.Any(s => Tools.IsAnotherTeam(HostShip, s) && IsInArc(s)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                descriptionShort: HostUpgrade.UpgradeInfo.Name,
                useByDefault: AlwaysUseByDefault,
                useAbility: SelectTarget,
                descriptionLong: $"You may spend 1 charge to strain 1 ship in your bullseye arc, if you do, you may acquire a target lock on it.",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void SelectTarget(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                selectTargetAction: ApplyStrain,
                filterTargets: IsInArc,
                getAiPriority: GetAiPriority,
                subphaseOwnerPlayerNo: HostShip.Owner.PlayerNo,
                name: HostUpgrade.UpgradeInfo.Name,
                description: $"Select a ship to strain.",
                imageSource: HostUpgrade,
                callback: DecisionSubPhase.ConfirmDecision
            );
        }

        private void ApplyStrain()
        {
            HostUpgrade.State.SpendCharge();
            TargetShip.Tokens.AssignToken(typeof(StrainToken), AskAcquireLock);
        }

        private void AskAcquireLock()
        {
            AskToUseAbility(
                descriptionShort: HostUpgrade.UpgradeInfo.Name,
                useByDefault: AlwaysUseByDefault,
                useAbility: AcquireLock,
                callback: SelectShipSubPhase.FinishSelection,
                descriptionLong: $"Would you like to acquire a lock on {TargetShip.PilotInfo.PilotName}?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void AcquireLock(object sender, EventArgs e)
        {
            ActionsHolder.AcquireTargetLock(HostShip, TargetShip, DecisionSubPhase.ConfirmDecision, DecisionSubPhase.ConfirmDecision);
        }

        private bool IsInArc(GenericShip ship)
        {
            return HostShip.SectorsInfo.IsShipInSector(ship, ArcType.Bullseye);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 10 - HostShip.GetRangeToShip(ship);
        }
    }
}
