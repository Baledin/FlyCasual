using Actions;
using BoardTools;
using Editions;
using GameCommands;
using GameModes;
using Obstacles;
using Players;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;
using UnityEngine.EventSystems;
using Upgrade;

public interface ITargetLockable
{
    int GetRangeToShip(GenericShip fromShip);
    void AssignToken(RedTargetLockToken token, Action callback);
    List<char> GetTargetLockLetterPairsOn(ITargetLockable targetShip);
    GenericTargetLockToken GetAnotherToken(Type oppositeType, char letter);
    void RemoveToken(GenericToken otherTargetLockToken);
    TokensManager GetTokens();
}

namespace ActionsList
{
    public class TargetLockAction : GenericAction
    {
        public TargetLockAction()
        {
            Name = DiceModificationName = "Target Lock";

            TokensSpend.Add(typeof(BlueTargetLockToken));
            IsReroll = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            if (ActionsHolder.HasTargetLockOn(Combat.Attacker, Combat.Defender))
            {
                List<char> letters = ActionsHolder.GetTargetLocksLetterPairs(Combat.Attacker, Combat.Defender);

                if (Combat.Attacker.Tokens.GetToken(typeof(BlueTargetLockToken), letters.First()).CanBeUsed)
                {
                    DiceRerollManager diceRerollManager = new()
                    {
                        CallBack = callBack
                    };

                    Selection.ActiveShip.Tokens.SpendToken(typeof(BlueTargetLockToken), diceRerollManager.Start, letters.First());
                }
                else
                {
                    Messages.ShowErrorToHuman("The attacker cannot use its current Target Lock on the defender");
                    callBack();
                }
            }
            else
            {
                Messages.ShowErrorToHuman("The attacker has no Target Lock on the defender");
                callBack();
            }
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                if (ActionsHolder.HasTargetLockOn(Combat.Attacker, Combat.Defender))
                {
                    result = !Combat.DiceRollAttack.IsEmpty;
                }
            }

            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.CurrentDiceRoll.FocusesNotRerolled;
                int attackBlanks = Combat.CurrentDiceRoll.BlanksNotRerolled;
                int numFocusTokens = Selection.ActiveShip.Tokens.CountTokensByType(typeof(FocusToken));

                if (numFocusTokens > 0)
                {
                    // Focus tokens can take care of our Focus results.  Use the Target Lock if there are any blanks.
                    if (attackBlanks > 0) result = 80;
                }
                else
                {
                    // We don't have any focus tokens.  If we have 1 or more blank + Focus results, use our Target Lock.
                    if (attackBlanks + attackFocuses > 0) result = 80;
                }
            }

            return result;
        }

        public override void ActionTake()
        {
            SelectTargetLockActionSubPhase subphase = Phases.StartTemporarySubPhaseNew<SelectTargetLockActionSubPhase>(
                "Select target for Target Lock",
                Phases.CurrentSubPhase.CallBack
            );
            subphase.HostAction = this;
            subphase.Start();
        }

        public override void RevertActionOnFail(bool hasSecondChance = false)
        {
            if (hasSecondChance)
            {
                UI.ShowSkipButton();
                UI.HighlightSkipButton();
            }
            else
            {
                Phases.GoBack();
            }
        }

        public override int GetActionPriority()
        {
            int result = 0;

            int maxOrdnanceRange = -1;
            int minOrdnanceRange = 99;
            int minShipTargetRange = HostShip.TargetLockMinRange;
            int curOrdnanceMax;
            int curOrdnanceMin;

            // Find the combined maximum and minimum range of all of our ordnance that currently has charges.
            foreach (GenericUpgrade currentUpgrade in Selection.ThisShip.UpgradeBar.GetUpgradesOnlyFaceup())
            {
                if (currentUpgrade is IShipWeapon
                    && (currentUpgrade.HasType(UpgradeType.Missile) || currentUpgrade.HasType(UpgradeType.Torpedo))
                    && currentUpgrade.State.Charges > currentUpgrade.UpgradeInfo.ChargesCost)
                {
                    if (currentUpgrade.UpgradeInfo.WeaponInfo.RequiresTokens.Contains(typeof(BlueTargetLockToken)))
                    {
                        curOrdnanceMax = currentUpgrade.UpgradeInfo.WeaponInfo.MaxRange;
                        curOrdnanceMin = currentUpgrade.UpgradeInfo.WeaponInfo.MinRange;

                        if (curOrdnanceMin < minOrdnanceRange && curOrdnanceMin >= minShipTargetRange)
                        {
                            minOrdnanceRange = curOrdnanceMin;
                        }

                        if (curOrdnanceMax > maxOrdnanceRange)
                        {
                            maxOrdnanceRange = curOrdnanceMax;
                        }
                    }
                }
            }

            // If our minimum range is less than 99, we have ordnance that is loaded and have set our min and max ranges.
            // Check all enemy ships to see if they are in range of our ordnance.
            if (minOrdnanceRange < 99 && HasValidLockTargetsAndNoLockOnShipInRange(Selection.ThisShip, minOrdnanceRange, maxOrdnanceRange))
            {
                // We have ordnance, we have targets for that ordnance, and none of them have our target lock on them.
                result += 55;
            }

            // We have at least 2 Force and we haven't already decided to possibly perform a target lock action.
            // Jedi with 2 or more Force should target lock more often than they focus.
            if (Selection.ThisShip.State.Force > 1 && result == 0 && HasValidLockTargetsAndNoLockOnShipInRange(Selection.ThisShip))
            {
                // We don't already have a target that is in range and locked, and we have targets available.
                result += 55;
            }

            return result;
        }

        /// <summary>
        /// Returns true if we don't already have a target that is in range and locked, and we have targets available.
        /// </summary>
        public static bool HasValidLockTargetsAndNoLockOnShipInRange(GenericShip ship, int minRange = 1, int maxRange = 3)
        {
            bool validTargetLockedAlready = false;
            int numTargetLockTargets = 0;

            foreach (KeyValuePair<string, GenericShip> anotherShip in Roster.GetPlayer(Roster.AnotherPlayer(ship.Owner.PlayerNo)).Ships)
            {
                ShotInfo shotInfo = new(ship, anotherShip.Value, ship.PrimaryWeapons);
                if (shotInfo.Range >= minRange && shotInfo.Range <= maxRange && shotInfo.IsShotAvailable)
                {
                    if (!ActionsHolder.HasTargetLockOn(ship, anotherShip.Value))
                    {
                        // We have a target in range that doesn't have a target lock on it from us.
                        numTargetLockTargets++;
                    }
                    else
                    {
                        // We already have a target in range that has our target lock on it.
                        validTargetLockedAlready = true;
                    }
                }
            }

            return (validTargetLockedAlready == false && numTargetLockTargets > 0);
        }
    }
}

namespace SubPhases
{
    public class SelectTargetLockActionSubPhase : AcquireTargetLockSubPhase
    {
        protected override void CancelShipSelection()
        {
            Rules.Actions.ActionIsFailed(TheShip, HostAction, ActionFailReason.WrongRange, true);
        }

        public override void SkipButton()
        {
            Rules.Actions.ActionIsFailed(TheShip, HostAction, ActionFailReason.WrongRange, false);
        }

        protected override void SuccessfulCallback()
        {
            Phases.FinishSubPhase(typeof(SelectTargetLockActionSubPhase));
            base.SuccessfulCallback();
        }
    }

    public class AcquireTargetLockSubPhase : SelectTargetLockableSubPhase
    {
        public override void Prepare()
        {
            CanMeasureRangeBeforeSelection = (Edition.Current is Editions.SecondEdition);

            DescriptionShort ??= "Target Lock";
            DescriptionLong ??= "Choose a ship to acquire a target lock on it";

            PrepareByParameters(
                TrySelectTargetLock,
                FilterShipTargets ?? FilterTargetLockTargets,
                GetTargetLockAiPriority,
                Selection.ThisShip.Owner.PlayerNo,
                true,
                DescriptionShort,
                DescriptionLong,
                ImageSource
            );
        }

        private bool FilterTargetLockTargets(ITargetLockable target)
        {
            return Rules.TargetLocks.TargetLockIsAllowed(Selection.ThisShip, target);
        }

        private int GetTargetLockAiPriority(GenericShip ship)
        {
            int result = 0;

            if (ship.Owner.PlayerNo != Selection.ThisShip.Owner.PlayerNo)
            {
                ShotInfo shotInfo = new(Selection.ThisShip, ship, Selection.ThisShip.PrimaryWeapons);
                if (shotInfo.IsShotAvailable) result += 1000;
                if (!ship.ShipsBumped.Contains(Selection.ThisShip)) result += 500;
                if (shotInfo.Range <= 3) result += 250;

                result += ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);
            }

            return result;
        }

        protected virtual void SuccessfulCallback()
        {
            UI.HideSkipButton();
            CallBack();
        }

        protected virtual void TrySelectTargetLock()
        {
            if (Rules.TargetLocks.TargetLockIsAllowed(Selection.ThisShip, TargetLocked))
            {
                ActionsHolder.AcquireTargetLock(
                    Selection.ThisShip,
                    TargetLocked,
                    SuccessfulCallback,
                    RevertSubPhase
                );
            }
            else
            {
                RevertSubPhase();
            }
        }

        public override void RevertSubPhase()
        {

        }

        public override void SkipButton()
        {
            CallBack();
        }
    }

    public class SelectTargetLockableSubPhase : SelectShipSubPhase
    {
        public static ITargetLockable TargetLocked { get; private set; }

        public override GenericShip TargetShip
        {
            get { return TargetLocked as GenericShip; }
            set { TargetLocked = value; }
        }

        private Func<ITargetLockable, bool> filterTargetLockableTargets;

        public Func<ITargetLockable, bool> FilterTargetLockableTargets
        {
            get { return filterTargetLockableTargets ?? FilterTargetLockTargets; }
            set { filterTargetLockableTargets = value; }
        }

        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.SelectShip, GameCommandTypes.SelectObstacle, GameCommandTypes.PressSkip, GameCommandTypes.CancelShipSelection }; } }

        private bool FilterTargetLockTargets(ITargetLockable target)
        {
            return Rules.TargetLocks.TargetLockIsAllowed(Selection.ThisShip, target);
        }

        public override void ProcessClick()
        {
            if (Roster.GetPlayer(RequiredPlayer).GetType() != typeof(HumanPlayer)) return;

            TryToSelectObstacle();
        }

        private void TryToSelectObstacle()
        {
            if (!EventSystem.current.IsPointerOverGameObject() &&
                (Input.touchCount == 0 || !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)))
            {
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo))
                    {
                        if (hitInfo.transform.tag.StartsWith("Obstacle"))
                        {
                            GenericObstacle clickedObstacle = ObstaclesManager.GetChosenObstacle(hitInfo.transform.name);

                            if (clickedObstacle.IsPlaced)
                            {
                                SelectObstacle(clickedObstacle);
                            }
                        }
                    }
                }
            }
        }

        private void SelectObstacle(GenericObstacle obstacle)
        {
            if (FilterTargetLockableTargets(obstacle))
            {
                if (IsLocked) return;
                IsLocked = true;

                ConfirmSelectionOfObstacle(obstacle);
            }
            else
            {
                Messages.ShowError("This obstacle cannot be selected");
            }
        }

        private void ConfirmSelectionOfObstacle(GenericObstacle obstacle)
        {
            GameMode.CurrentGameMode.ExecuteCommand(GenerateSelectObstacleCommand(obstacle.ObstacleGO.name));
        }

        private GameCommand GenerateSelectObstacleCommand(string obstacleName)
        {
            JSONObject parameters = new();
            parameters.AddField("name", obstacleName);
            return GameController.GenerateGameCommand(
                GameCommandTypes.SelectObstacle,
                Phases.CurrentSubPhase.GetType(),
                Phases.CurrentSubPhase.ID,
                parameters.ToString()
            );
        }

        public static void ConfirmSelectionOfObstacle(string obstacleName)
        {
            TargetLocked = ObstaclesManager.GetChosenObstacle(obstacleName);
            (Phases.CurrentSubPhase as SelectTargetLockableSubPhase).InvokeFinish();
        }
    }
}