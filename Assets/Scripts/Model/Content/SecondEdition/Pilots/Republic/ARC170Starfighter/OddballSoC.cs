using ActionsList;
using BoardTools;
using Content;
using Editions;
using Movement;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ARC170Starfighter
    {
        public class OddballSoC : ARC170Starfighter
        {
            public OddballSoC() : base()
            {
                PilotInfo = new PilotCardInfo25(
                    "\"Odd Ball\"",
                    "Siege of Coruscant",
                    Faction.Republic,
                    5,
                    5,
                    0,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.OddBallSoCAbility),
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    },
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Gunner,
                        UpgradeType.Astromech
                    },
                    isStandardLayout: true,
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );

                ShipInfo.Shields++;
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.Selfless));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.VeteranTailGunner));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.R4PAstromech));

                ShipAbilities.Add(new Abilities.SecondEdition.BornForThisAbility());

                PilotNameCanonical = "oddball-siegeofcoruscant";

                ModelInfo.SkinName = "Red";
            }
        }

        public class OddballSoCXWA : OddballSoC
        {
            public OddballSoCXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 12;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class OddBallSoCAbility : GenericAbility
    {
        private GenericShip LockedShip;

        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckConditions;
            HostShip.OnMovementFinishSuccessfully += RegisterMovementTrigger;
        }
        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
            HostShip.OnMovementFinishSuccessfully -= RegisterMovementTrigger;
        }

        protected void CheckConditions(GenericAction action)
        {
            if (action.IsRed)
            {
                HostShip.OnActionDecisionSubphaseEnd += RegisterActionTrigger;
            }
        }

        protected void RegisterMovementTrigger(GenericShip ship)
        {
            if (HostShip.GetLastManeuverColor() == MovementComplexity.Complex && Board.GetShipsAtRange(HostShip, new Vector2(0, 1), Team.Type.Enemy).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToChooseLockedTarget);
            }
        }

        private void RegisterActionTrigger(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= RegisterActionTrigger;

            if (Board.GetShipsAtRange(HostShip, new Vector2(0, 1), Team.Type.Enemy).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToChooseLockedTarget);
            }
        }

        private void AskToChooseLockedTarget(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                AskToSelectAnotherFriendlyShip,
                FilterTargets,
                GetLockedTargetAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostShip.PilotInfo.PilotName,
                description: "You may choose an enemy ship at range 0-1...",
                imageSource: HostShip
            );
        }

        private bool FilterTargets(GenericShip ship)
        {
            return Board.GetShipsAtRange(HostShip, new Vector2(0, 1), Team.Type.Enemy).Contains(ship);
        }

        private int GetLockedTargetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }

        private void AskToSelectAnotherFriendlyShip()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            LockedShip = TargetShip;

            SelectTargetForAbility(
                AcquireTargetLock,
                FilterFriendlyTargets,
                GetFriednlyShipAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostShip.PilotInfo.PilotName,
                description: "Choose a friendly ship at range 0-3, it may acquire a lock on that enemy ship",
                imageSource: HostShip
            );
        }

        private bool FilterFriendlyTargets(GenericShip ship)
        {
            return Board.GetShipsAtRange(HostShip, new Vector2(0, 3), Team.Type.Friendly).Contains(ship);
        }

        private int GetFriednlyShipAiPriority(GenericShip ship)
        {
            int priority = ship.PilotInfo.Cost;

            DistanceInfo distInfo = new DistanceInfo(ship, LockedShip);
            if (distInfo.Range < 4) priority += 100;

            ShotInfo shotInfo = new ShotInfo(ship, LockedShip, ship.PrimaryWeapons);
            if (shotInfo.IsShotAvailable) priority += 50;

            if (!ship.Tokens.HasToken<BlueTargetLockToken>('*')) priority += 100;

            if (!ship.ActionBar.HasAction(typeof(TargetLockAction))) priority = 0;

            return priority;
        }

        private void AcquireTargetLock()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            IsAbilityUsed = true;
            Messages.ShowInfo(TargetShip.PilotInfo.PilotName + " acquired a Lock on " + LockedShip.PilotInfo.PilotName);
            ActionsHolder.AcquireTargetLock(TargetShip, LockedShip, FinishAbility, FinishAbility);
        }

        private void FinishAbility()
        {

            Selection.ChangeActiveShip(HostShip);
            Triggers.FinishTrigger();
        }
    }

    public class BornForThisAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName + ": Born for This: Focus",
                IsFocusAvailable,
                () => 20,
                DiceModificationType.Change,
                int.MaxValue,
                new List<DieSide> { DieSide.Focus },
                DieSide.Success,
                isGlobal: true,
                payAbilityCost: PayFocusCost
            );

            AddDiceModification(
                HostName + ": Born for This: Evade",
                IsEvadeAvailable,
                GetDiceModificationPriority,
                DiceModificationType.Change,
                1,
                new List<DieSide> { DieSide.Blank, DieSide.Focus },
                DieSide.Success,
                isGlobal: true,
                payAbilityCost: PayEvadeCost
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                int attackSuccessesCancelable = Combat.DiceRollAttack.SuccessesCancelable;
                int defenceSuccesses = Combat.CurrentDiceRoll.Successes;
                if (attackSuccessesCancelable > defenceSuccesses)
                {
                    int defenceFocuses = Combat.DiceRollDefence.Focuses;
                    int numFocusTokens = Selection.ActiveShip.Tokens.CountTokensByType(typeof(FocusToken));
                    if (numFocusTokens > 0 && defenceFocuses == Combat.CurrentDiceRoll.Count)
                    {
                        // Multiple focus results on our defense roll and we have a Focus token.  Use it instead of the Evade.
                        result = 0;
                    }
                    else
                    {
                        // Either we don't have a focus token or we have at least one blank.  Better use the Evade.
                        result = 70;
                    }
                }
            }

            if (Edition.Current is Editions.SecondEdition && Combat.CurrentDiceRoll.Failures == 0) return 0;

            return result;
        }

        private bool IsEvadeAvailable()
        {
            GenericShip activeShip = Combat.Defender;
            return Combat.AttackStep == CombatStep.Defence
                && !HostShip.IsStrained
                && Tools.IsFriendly(activeShip, HostShip)
                && activeShip != HostShip
                && Board.IsShipBetweenRange(HostShip, activeShip, 0, 2)
                && HostShip.Tokens.HasToken(typeof(EvadeToken));
        }

        private void PayEvadeCost(Action<bool> callback)
        {
            if (HostShip.Tokens.HasToken(typeof(EvadeToken)))
            {
                HostShip.Tokens.AssignToken(typeof(StrainToken), delegate { });
                HostShip.Tokens.RemoveToken(typeof(EvadeToken), () => callback(true));
            }
            else callback(false);
        }

        private bool IsFocusAvailable()
        {
            GenericShip activeShip = Combat.Defender;
            return Combat.AttackStep == CombatStep.Defence
                && Combat.CurrentDiceRoll.Focuses > 0
                && !HostShip.IsStrained
                && Tools.IsFriendly(activeShip, HostShip)
                && activeShip != HostShip
                && Board.IsShipBetweenRange(HostShip, activeShip, 0, 2)
                && HostShip.Tokens.HasToken(typeof(FocusToken));
        }

        private void PayFocusCost(Action<bool> callback)
        {
            if (HostShip.Tokens.HasToken(typeof(FocusToken)))
            {
                HostShip.Tokens.AssignToken(typeof(StrainToken), delegate { });
                HostShip.Tokens.RemoveToken(typeof(FocusToken), () => callback(true));
            }
            else callback(false);
        }
    }
}
