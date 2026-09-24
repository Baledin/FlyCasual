using Abilities.SecondEdition;
using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.TIESfFighter
{
    public class LieutenantLeHuseEoD : TIESfFighter
    {
        public LieutenantLeHuseEoD() : base()
        {
            PilotInfo = new PilotCardInfo25(
                pilotName: "Lieutenant LeHuse",
                pilotTitle: "Evacuation of D'Qar",
                faction: Faction.FirstOrder,
                initiative: 5,
                cost: 11,
                loadoutValue: 0,
                isLimited: true,
                limited: 1,
                abilityType: typeof(LieutenantLeHuseEoDAbility),
                extraUpgradeIcons: new()
                {
                    UpgradeType.Talent,
                    UpgradeType.Missile,
                    UpgradeType.Gunner
                },
                tags: new()
                {
                    Tags.Tie
                },
                isStandardLayout: true,
                legality: new List<Legality>() { Legality.XWA }
            );

            PilotNameCanonical = "lieutenantlehuse-evacuationofdqar";

            MustHaveUpgrades.Add(typeof(Determination));
            MustHaveUpgrades.Add(typeof(ConcussionMissiles));
            MustHaveUpgrades.Add(typeof(FirstOrderOrdnanceTechEoD));

            ShipAbilities.Add(new HeavyWeaponTurretEoD());

            ShipInfo.ActionIcons.LinkedActions.Clear();
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LieutenantLeHuseEoDAbility : GenericAbility
    {
        // At the start of the Engagement Phase, you may acquire a lock on an object at range 1-3 that has a friendly lock. If you do, break a friendly lock on that target.

        readonly List<ITargetLockable> targets = new();
        ITargetLockable enemyTarget;

        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            SetTargets();

            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskStealTargetLock);
        }

        private void SetTargets()
        {
            targets.Clear();

            foreach (GenericShip friendly in HostShip.Owner.Ships.Values)
            {
                foreach (BlueTargetLockToken token in friendly.Tokens.GetTokens<BlueTargetLockToken>('*'))
                {
                    if (token.OtherTargetLockTokenOwner.GetRangeToShip(HostShip) is >= 1 and <= 3) targets.Add(token.OtherTargetLockTokenOwner);
                }
            }
        }

        private void AskStealTargetLock(object sender, EventArgs e)
        {
            if (targets.Count > 0)
            {
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    NeverUseByDefault,
                    StealTargetLock,
                    callback: Triggers.FinishTrigger,
                    descriptionLong: $"You may acquire a lock on an object at range 1-3 that has a friendly lock. If you do, break a friendly lock on that target.",
                    imageHolder: HostShip,
                    requiredPlayer: HostShip.Owner.PlayerNo
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void StealTargetLock(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                AcquireLock,
                GetEnemyTargets, // TODO: filters don't allow non-ships
                StealLockAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostShip.PilotInfo.PilotName,
                description: "Select a target to acquire a lock.",
                imageSource: HostShip,
                callback: DecisionSubPhase.ConfirmDecision
            );
        }

        private void AcquireLock()
        {
            enemyTarget = TargetShip; // Used because we have multiple phases, each with their own TargetShip that changes

            ActionsHolder.AcquireTargetLock(
                HostShip,
                TargetShip,
                AskBreakFriendlyLock,
                delegate
                {
                    Messages.ShowError("Target lock failed.");
                    SelectShipSubPhase.FinishSelection();
                }
            );
        }

        private void AskBreakFriendlyLock()
        {
            List<RedTargetLockToken> locks = (enemyTarget as GenericShip).Tokens.GetTokens<RedTargetLockToken>('*').Where(t => t.OtherTargetLockTokenOwner != HostShip && Tools.IsFriendly(HostShip, t.OtherTargetLockTokenOwner as GenericShip)).ToList();

            if (locks.Count == 1)
            {
                BreakFriendlyLock(locks.First(), SelectShipSubPhase.FinishSelection);
            }
            else
            {
                BreakLockDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<BreakLockDecisionSubPhase>(
                    "Select a friendly target lock to break",
                    delegate
                    {

                        Triggers.FinishTrigger();
                    }
                 );

                subphase.DescriptionShort = "Break target lock";
                subphase.DescriptionLong = "Select a friendly target lock to break";
                subphase.ImageSource = HostShip;

                foreach (RedTargetLockToken tlock in locks)
                {
                    subphase.AddDecision($"{tlock.Letter}", delegate { BreakFriendlyLock(tlock, BreakLockDecisionSubPhase.ConfirmDecision); });
                }

                subphase.ShowSkipButton = false;

                subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;

                subphase.CallBack = SelectShipSubPhase.FinishSelection;

                subphase.Start();
            }
        }

        private void BreakFriendlyLock(RedTargetLockToken redLock, Action callback)
        {
            redLock.RemoveFromHost(callback);
        }

        private int StealLockAiPriority(ITargetLockable target)
        {
            // Higher priority to closer ships in front arc
            if (HostShip.GetAllWeapons().Any(w => new ShotInfo(HostShip, (target as GenericShip), w).IsShotAvailable))
            {
                return 100 - (target.GetRangeToShip(HostShip) * 10);
            }

            return 0;
        }

        private bool GetEnemyTargets(GenericShip ship)
        {
            return targets.Contains(ship);
        }

        private class BreakLockDecisionSubPhase : DecisionSubPhase { }
    }
}