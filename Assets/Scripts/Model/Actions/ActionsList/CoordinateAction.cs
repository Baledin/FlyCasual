using Actions;
using ActionsList;
using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;

namespace Actions
{
    public class CoordinateActionData : EventArgs
    {
        public int MaxTargets { get; set; }
        public bool SameShipTypeLimit { get; set; }
        public bool TargetLowerInitiave { get; set; }
        public bool SameActionLimit { get; set; }
        public bool TreatCoordinatedActionAsRed { get; set; }
        public GenericShip CoordinateProvider { get; protected set; }
        public GenericAction FirstChosenAction { get; set; }
        public Func<GenericShip, bool> Filter { get; set; }
        public Func<GenericShip, int> GetAiPriority;

        public int MinRange = 1;
        public int MaxRange = 2;

        public CoordinateActionData(GenericShip coordinateProvider)
        {
            CoordinateProvider = coordinateProvider;
            MaxTargets = 1;
            Filter = FilterCoordinateTargets;
        }

        protected bool FilterCoordinateTargets(GenericShip ship)
        {
            return ship.Owner.PlayerNo == Selection.ThisShip.Owner.PlayerNo
                && Board.CheckInRange(CoordinateProvider, ship, MinRange, MaxRange, RangeCheckReason.CoordinateAction)
                && ship.CanBeCoordinated
                && (!TargetLowerInitiave || ship.PilotInfo.Initiative < CoordinateProvider.PilotInfo.Initiative)
                && (!SameShipTypeLimit || Selection.MultiSelectedShips.Count == 0 || ship.ShipInfo.ShipName == Selection.MultiSelectedShips.First().ShipInfo.ShipName)
                && CoordinateProvider.CallCheckCanCoordinate(ship);
        }
    }
}

namespace ActionsList
{

    public class CoordinateAction : GenericAction
    {
        protected CoordinateActionData CoordinateActionData;

        public CoordinateAction()
        {
            Name = DiceModificationName = "Coordinate";
        }

        public override void ActionTake()
        {
            CoordinateActionData = HostShip.CallCheckCoordinateModeModification();

            if (CoordinateActionData.MaxTargets == 1)
            {
                CoordinateTargetSubPhase subphase = Phases.StartTemporarySubPhaseNew<CoordinateTargetSubPhase>(
                    "Select target for Coordinate",
                    Phases.CurrentSubPhase.CallBack
                );
                subphase.MinRange = CoordinateActionData.MinRange;
                subphase.MaxRange = CoordinateActionData.MaxRange;
                subphase.HostAction = this;
                subphase.Start();
            }
            else
            {
                CoordinateMultiTargetSubPhase subphase = Phases.StartTemporarySubPhaseNew<CoordinateMultiTargetSubPhase>(
                    "Select targets for Coordinate",
                    Phases.CurrentSubPhase.CallBack
                );
                subphase.HostAction = this;

                subphase.RequiredPlayer = HostShip.Owner.PlayerNo;

                subphase.Filter = CoordinateActionData.Filter;
                subphase.MaxToSelect = CoordinateActionData.MaxTargets;
                subphase.WhenDone = CoordinateTargets;
                subphase.CoordinateActionData = CoordinateActionData;
                subphase.GetAiPriority += CoordinateActionData.GetAiPriority;

                subphase.DescriptionShort = "Coordinate Action";
                subphase.DescriptionLong = "Select one or more other ships.\nThey will each perform an action.";

                subphase.Start();
            }
        }

        protected void CoordinateTargets(Action callback)
        {
            Phases.CurrentSubPhase.Pause();

            foreach (GenericShip ship in Selection.MultiSelectedShips)
            {
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = string.Format("Coordinate {0}: {1}", ship.ShipId, ship.PilotInfo.PilotName),
                        TriggerType = TriggerTypes.OnCoordinateMultiTargetsAreSelected,
                        TriggerOwner = HostShip.Owner.PlayerNo,
                        EventHandler = delegate { CoordinateShipForMultiSelection(ship); }
                    }
                );
            }

            Triggers.ResolveTriggers(TriggerTypes.OnCoordinateMultiTargetsAreSelected, callback);
        }

        protected void CoordinateShipForMultiSelection(GenericShip targetShip)
        {
            CoordinateActionData.CoordinateProvider.OnCoordinateTargetIsSelected += PrepareToRememberChosenAction;
            CoordinateActionData.CoordinateProvider.CallCoordinateTargetIsSelected(
                targetShip,
                delegate { PerformMultiCoordinateEffect(targetShip); }
            );
        }

        protected void PrepareToRememberChosenAction(GenericShip coordinatedShip)
        {
            coordinatedShip.OnActionIsPerformed += RememberChosenAction;
            coordinatedShip.OnActionIsSkipped += ClearRememberChosenAction;
        }

        protected void ClearRememberChosenAction(GenericShip coordinatedShip)
        {
            coordinatedShip.OnActionIsPerformed -= RememberChosenAction;
            coordinatedShip.OnActionIsSkipped -= ClearRememberChosenAction;
        }

        protected void RememberChosenAction(GenericAction action)
        {
            CoordinateActionData.FirstChosenAction ??= action;
            ClearRememberChosenAction(Selection.ThisShip);
        }

        protected void PerformMultiCoordinateEffect(GenericShip targetShip)
        {
            CoordinateActionData.CoordinateProvider.State.LastCoordinatedShip = targetShip;

            Selection.ChangeActiveShip(targetShip);
            GenericAction currentAction = ActionsHolder.CurrentAction;

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Coordinate",
                    TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnFreeActionPlanned,
                    EventHandler = delegate { PerformFreeAction(targetShip); }
                }
            );

            MovementTemplates.ReturnRangeRuler();

            Triggers.ResolveTriggers(TriggerTypes.OnFreeActionPlanned, (System.Action)delegate
            {
                Selection.ChangeActiveShip(CoordinateActionData.CoordinateProvider);
                CoordinateActionData.CoordinateProvider.OnCoordinateTargetIsSelected -= PrepareToRememberChosenAction;
                ActionsHolder.CurrentAction = currentAction;
                Triggers.FinishTrigger();
            });
        }

        protected virtual void PerformFreeAction(GenericShip targetShip)
        {
            targetShip.AskPerformFreeAction(
                GetPossibleActions(),
                delegate
                {
                    Selection.ChangeActiveShip(CoordinateActionData.CoordinateProvider);
                    Triggers.FinishTrigger();
                },
                "Coordinate action",
                "You are coordinated by " + CoordinateActionData.CoordinateProvider.PilotInfo.PilotName
            );
        }

        protected virtual List<GenericAction> GetPossibleActions()
        {
            List<GenericAction> result = new();

            if (!CoordinateActionData.SameActionLimit || CoordinateActionData.FirstChosenAction == null)
            {
                result = Selection.ThisShip.GetAvailableActions();
            }
            else
            {
                result = Selection.ThisShip.GetAvailableActions().Where(n => n.GetType() == CoordinateActionData.FirstChosenAction.GetType()).ToList();
            }

            if (CoordinateActionData.TreatCoordinatedActionAsRed)
            {
                result = result.Select(n => n.AsRedAction).ToList();
            }

            return result;
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
    }
}

namespace SubPhases
{
    public class CoordinateTargetSubPhase : SelectShipSubPhase
    {
        public int MinRange = 1;
        public int MaxRange = 2;

        public override void Prepare()
        {
            PrepareByParameters(
                SelectCoordinateTarget,
                FilterCoordinateTargets,
                GetAiCoordinatePriority,
                Selection.ThisShip.Owner.PlayerNo,
                false,
                "Coordinate Action",
                "Select another ship.\nIt will perform an action."
            );
        }

        protected override void CancelShipSelection()
        {
            Rules.Actions.ActionIsFailed(TheShip, HostAction, ActionFailReason.WrongRange, true);
        }

        public override void SkipButton()
        {
            Rules.Actions.ActionIsFailed(TheShip, HostAction, ActionFailReason.WrongRange, false);
        }

        protected int GetAiCoordinatePriority(GenericShip ship)
        {
            int result = 0;

            result += NeedTokenPriority(ship);
            result += ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);

            return result;
        }

        protected int NeedTokenPriority(GenericShip ship)
        {
            if (!ship.Tokens.HasToken(typeof(FocusToken))) return 100;
            if (ship.ActionBar.HasAction(typeof(EvadeAction)) && !ship.Tokens.HasToken(typeof(EvadeToken))) return 50;
            if (ship.ActionBar.HasAction(typeof(TargetLockAction)) && !ship.Tokens.HasToken(typeof(BlueTargetLockToken), '*')) return 50;
            return 0;
        }

        protected bool FilterCoordinateTargets(GenericShip ship)
        {
            return ship.Owner.PlayerNo == Selection.ThisShip.Owner.PlayerNo
                && Board.CheckInRange(Selection.ThisShip, ship, MinRange, MaxRange, RangeCheckReason.CoordinateAction)
                && ship.CanBeCoordinated
                && Selection.ThisShip.CallCheckCanCoordinate(ship);
        }

        protected void SelectCoordinateTarget()
        {
            Selection.ThisShip.State.LastCoordinatedShip = TargetShip;

            Selection.ThisShip.CallCoordinateTargetIsSelected(TargetShip, PerformCoordinateEffect);
        }

        protected virtual void PerformCoordinateEffect()
        {
            GenericShip coordinatingShip = Selection.ThisShip;
            Selection.ThisShip = TargetShip;
            GenericAction currentAction = ActionsHolder.CurrentAction;

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Coordinate",
                    TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnFreeActionPlanned,
                    EventHandler = PerformFreeAction
                }
            );

            MovementTemplates.ReturnRangeRuler();

            Triggers.ResolveTriggers(TriggerTypes.OnFreeActionPlanned, (Action)delegate
            {
                Selection.ThisShip = coordinatingShip;
                ActionsHolder.CurrentAction = currentAction;
                Phases.FinishSubPhase(typeof(CoordinateTargetSubPhase));
                CallBack();
            });
        }

        public override void RevertSubPhase() { }

        protected virtual List<GenericAction> GetPossibleActions()
        {
            return Selection.ThisShip.GetAvailableActions().Select(n => n.AsCoordinatedAction).ToList();
        }

        protected virtual void PerformFreeAction(object sender, System.EventArgs e)
        {
            TargetShip.AskPerformFreeAction(
                GetPossibleActions(),
                Triggers.FinishTrigger,
                "Coordinate action",
                "You are coordinated"
            );
        }
    }

    public class CoordinateMultiTargetSubPhase : MultiSelectionSubphase
    {
        public CoordinateActionData CoordinateActionData;
    }
}