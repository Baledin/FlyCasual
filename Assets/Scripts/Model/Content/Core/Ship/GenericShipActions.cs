using Actions;
using ActionsList;
using Arcs;
using BoardTools;
using Editions;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;

namespace Ship
{
    public partial class GenericShip : ITargetLockable
    {
        private List<GenericAction> AvailableActionsList = new();
        private List<GenericAction> AvailableFreeActionsList = new();
        private List<GenericAction> AlreadyExecutedActions = new();

        private List<GenericAction> AvailableDiceModifications = new();
        private List<GenericAction> AlreadyUsedDiceModifications = new();

        public List<GenericAction> PlannedLinkedActions;

        // EVENTS
        public event EventHandlerShip OnMovementActivationStart;
        public static event EventHandlerShip OnMovementActivationStartGlobal;
        public event EventHandlerShip OnMovementActivationFinish;

        public event EventHandlerShip OnGenerateActions;
        public event EventHandlerShipActionBool OnTryAddAction;
        public static event EventHandlerShipActionBool OnTryAddActionGlobal;

        public event EventHandlerShip OnGenerateDiceModifications;
        public static event EventHandlerShip OnGenerateDiceModificationsGlobal;
        public event EventHandlerShipActionBool OnTryAddAvailableDiceModification;
        public static event EventHandlerShipActionBool OnTryAddAvailableDiceModificationGlobal;

        public event EventHandlerShip OnGenerateDiceModificationsOpposite;
        public static event EventHandlerShip OnGenerateDiceModificationsOppositeGlobal;
        public event EventHandlerShipActionBool OnTryAddDiceModificationOpposite;

        public event EventHandlerShip OnGenerateDiceModificationsAfterRolled;
        public static event EventHandlerShip OnGenerateDiceModificationsAfterRolledGlobal;
        public event EventHandlerShipActionBool OnTryAddDiceModificationAfterRolled;

        public event EventHandlerShip OnGenerateDiceModificationsCompareResults;
        public static event EventHandlerShip OnGenerateDiceModificationsCompareResultsGlobal;
        public event EventHandlerActionBool OnTryAddDiceModificationCompareResults;

        public event EventHandlerShip OnActionDecisionSubphaseEnd;
        public event EventHandlerShip OnActionIsSkipped;
        public event EventHandlerActionBool BeforeActionIsPerformed;
        public event EventHandlerAction OnActionIsPerformed;
        public static event EventHandlerAction OnActionIsPerformedGlobal;
        public event EventHandlerAction OnActionIsPerformed_System;

        public event EventHandlerShipToken OnTokenIsAssigned;
        public event EventHandlerShipToken BeforeTokenIsAssigned;
        public static event EventHandlerShipToken BeforeTokenIsAssignedGlobal;
        public static event EventHandlerShipToken OnTokenIsAssignedGlobal;
        public event EventHandlerShipToken OnTokenIsSpent;
        public static event EventHandlerShipToken OnTokenIsSpentGlobal;
        public event EventHandlerShipToken OnTokenIsRemoved;
        public static event EventHandlerShipToken OnTokenIsRemovedGlobal;
        public event EventHandlerShipTokenBool OnBeforeTokenIsRemoved;
        public static event EventHandlerShipTokenBool OnBeforeTokenIsRemovedGlobal;

        public event EventHandlerShipToken OnConditionIsAssigned;
        public event EventHandlerShipToken OnConditionIsRemoved;

        public event EventHandlerTargetLockable OnTargetLockIsAcquired;
        public static event EventHandlerShipTargetLockable OnTargetLockIsAcquiredGlobal;

        public event EventHandlerShip OnCoordinateTargetIsSelected;
        public event EventHandlerShip OnJamTargetIsSelected;

        public event EventHandlerShip OnRerollIsConfirmed;

        public EventHandlerShipTokenBool BeforeRemovingTokenInEndPhase;
        public static EventHandlerShipTokenBool BeforeRemovingTokenInEndPhaseGlobal;

        public event EventHandler OnBeforeDecloak;
        public event EventHandler OnDecloak;
        public event EventHandler OnSlam;

        public event EventHandlerActionColor OnCheckActionComplexity;
        public event EventHandlerActionColor OnCheckActionColor;

        public event EventHandlerArcFacingList OnGetAvailableArcFacings;

        public event EventHandlerFailedAction OnActionIsReadyToBeFailed;
        public event EventHandlerAction OnActionIsReallyFailed;

        public event EventHandlerShipRefBool OnCanBeCoordinated;

        public event EventHandlerShip OnAfterModifyDefenseDiceStep;

        public event EventHandlerShip OnPerformActionStepStart;

        public event EventHandlerActionShip OnActionTargetIsWrong;

        public event EventHandlerCoordinateData OnCheckCoordinateModeModification;
        public event EventHandlerShipRefBool OnCheckCanCoordinate;

        public event EventHandlerActionBool OnCanPerformActionWhileIonized;
        public event EventHandlerActionBool OnCanPerformActionWhileStressed;
        public event EventHandlerBool OnCheckCanPerformActionsWhileStressed;

        // ACTIONS

        public void GenerateAvailableActionsList()
        {
            AvailableActionsList = new List<GenericAction>();

            foreach (GenericAction action in ActionBar.AllActions)
            {
                AddAvailableAction(action);
            }

            OnGenerateActions?.Invoke(this);
        }

        public List<GenericAction> GetAvailableActions()
        {
            GenerateAvailableActionsList();
            return AvailableActionsList;
        }

        public List<GenericAction> GetAvailableActionsWhiteOnly()
        {
            return GetAvailableActions().Where(a => a.Color == ActionColor.White).ToList();
        }

        private GenericAction GetActionAsRed(GenericAction action)
        {
            //Make a deep clone to avoid changing the original action to red
            GenericAction instance = (GenericAction)Activator.CreateInstance(action.GetType());
            if (instance.IsCritCancelAction) ((CancelCritAction)instance).Initialize(((CancelCritAction)action).CritCard);
            instance.Color = ActionColor.Red;
            return instance;
        }

        public List<GenericAction> GetAvailableActionsAsRed()
        {
            List<GenericAction> redActions = new();

            GenerateAvailableActionsList();

            foreach (GenericAction action in AvailableActionsList)
            {
                redActions.Add(GetActionAsRed(action));
            }

            return redActions;
        }

        public List<GenericAction> GetAvailableActionsWhiteOnlyAsRed()
        {
            List<GenericAction> redActions = new();

            GenerateAvailableActionsList();

            foreach (GenericAction action in AvailableActionsList.Where(n => n.Color == ActionColor.White))
            {
                redActions.Add(GetActionAsRed(action));
            }

            return redActions;
        }

        public List<GenericAction> GetAvailableFreeActions()
        {
            return AvailableFreeActionsList;
        }

        public void CallMovementActivationStart(Action callBack)
        {
            OnMovementActivationStart?.Invoke(this);

            OnMovementActivationStartGlobal?.Invoke(this);

            Triggers.ResolveTriggers(TriggerTypes.OnMovementActivationStart, callBack);
        }

        public void CallMovementActivationFinish(Action callback)
        {
            OnMovementActivationFinish?.Invoke(this);

            Triggers.ResolveTriggers(TriggerTypes.OnMovementActivationFinish, callback);
        }

        public void CallOnActionDecisionSubphaseEnd(Action callback)
        {
            OnActionDecisionSubphaseEnd?.Invoke(this);

            Triggers.ResolveTriggers(TriggerTypes.OnActionDecisionSubPhaseEnd, callback);
        }

        public void CallActionIsSkipped()
        {
            OnActionIsSkipped?.Invoke(this);
        }

        public void CallActionIsTaken(GenericAction action, Action callBack)
        {
            if (!action.IsRealAction)
            {
                callBack();
                return;
            }

            OnActionIsPerformed_System?.Invoke(action);

            Triggers.ResolveTriggers(
                TriggerTypes.OnActionIsPerformed_System,
                delegate
                {
                    OnActionIsPerformed?.Invoke(action);

                    OnActionIsPerformedGlobal?.Invoke(action);

                    Triggers.ResolveTriggers(TriggerTypes.OnActionIsPerformed, callBack);
                }
            );
        }

        public void CallBeforeActionIsPerformed(GenericAction action, Action callBack, bool isFree)
        {
            if (!action.IsRealAction)
            {
                callBack();
                return;
            }

            BeforeActionIsPerformed?.Invoke(action, ref isFree);

            Triggers.ResolveTriggers(TriggerTypes.BeforeActionIsPerformed, callBack);
        }

        public void GenerateAvailableFreeActionsList(List<GenericAction> freeActions)
        {
            AvailableFreeActionsList = new List<GenericAction>();
            foreach (GenericAction action in freeActions)
            {
                AddAvailableFreeAction(action);
            }

            OnGenerateActions?.Invoke(this);
        }

        public bool CanPerformAction(GenericAction action)
        {
            bool result = action.IsActionAvailable();

            OnTryAddAction?.Invoke(this, action, ref result);

            OnTryAddActionGlobal?.Invoke(this, action, ref result);

            return result;
        }

        public bool CanPerformFreeAction(GenericAction action)
        {
            bool result = action.IsActionAvailable() && action.CanBePerformedAsAFreeAction;

            OnTryAddAction?.Invoke(this, action, ref result);

            OnTryAddActionGlobal?.Invoke(this, action, ref result);

            return result;
        }

        public void AskPerformFreeAction(GenericAction freeAction, Action callback, string descriptionShort, string descriptionLong = null, IImageHolder imageHolder = null, bool isForced = false)
        {
            AskPerformFreeAction(new List<GenericAction> { freeAction }, callback, descriptionShort, descriptionLong, imageHolder, isForced);
        }

        // TODO: move actions list into subphase
        public void AskPerformFreeAction(List<GenericAction> freeActions, Action callback, string descriptionShort, string descriptionLong = null, IImageHolder imageHolder = null, bool isForced = false)
        {
            foreach (GenericAction freeAction in freeActions)
            {
                freeAction.HostShip ??= Selection.ThisShip;
            }

            GenerateAvailableFreeActionsList(freeActions);

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Free action",
                    TriggerOwner = this.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnFreeAction,
                    EventHandler = delegate
                    {
                        FreeActionDecisonSubPhase newSubPhase = (FreeActionDecisonSubPhase)Phases.StartTemporarySubPhaseNew
                        (
                            "Free action decision",
                            typeof(FreeActionDecisonSubPhase),
                            (Action)delegate
                            {
                                if (Phases.CurrentSubPhase is FreeActionDecisonSubPhase phase && phase.ActionWasPerformed)
                                {
                                    ActionsHolder.TakeActionFinish(
                                        delegate
                                        {
                                            ActionsHolder.EndActionDecisionSubhase(
                                            delegate { FinishFreeActionDecision(callback); }
                                            );
                                        }
                                    );
                                }
                                else
                                {
                                    Selection.ThisShip.CallActionIsSkipped();
                                    FinishFreeActionDecision(callback);
                                }
                            }
                        );
                        newSubPhase.DecisionOwner = this.Owner;
                        newSubPhase.ShowSkipButton = !isForced;
                        newSubPhase.IsForced = isForced;
                        newSubPhase.DescriptionShort = descriptionShort;
                        newSubPhase.DescriptionLong = descriptionLong;
                        newSubPhase.ImageSource = imageHolder;

                        newSubPhase.Start();
                    }
                }
            );

            Triggers.ResolveTriggers(TriggerTypes.OnFreeAction, Triggers.FinishTrigger);
        }

        private void FinishFreeActionDecision(Action callback)
        {
            Phases.FinishSubPhase(typeof(FreeActionDecisonSubPhase));
            callback();
        }

        public void AddAvailableAction(GenericAction action)
        {
            if (CanPerformAction(action))
            {
                if (!AvailableActionsList.Any(n => n.Name == action.Name && n.Color == action.Color))
                {
                    AvailableActionsList.Add(action);
                }
            }
        }

        public void RemoveAvailableAction(Type ActionType)
        {
            AvailableActionsList.RemoveAll(action => action.GetType().Equals(ActionType));
        }

        public void AddAvailableFreeAction(GenericAction action)
        {
            if (CanPerformFreeAction(action))
            {
                if (!AvailableFreeActionsList.Any(n => n.Name == action.Name && n.Color == action.Color))
                {
                    AvailableFreeActionsList.Add(action);
                }
            }
        }

        public void AddAlreadyExecutedAction(GenericAction action)
        {
            if (action.IsRealAction) AlreadyExecutedActions.Add(action);
        }

        public void ClearAlreadyExecutedActions()
        {
            AlreadyExecutedActions = new List<GenericAction>();
        }

        public void RemoveAlreadyExecutedAction(GenericAction action)
        {
            List<GenericAction> keys = new(AlreadyExecutedActions);

            foreach (GenericAction executedAction in keys)
            {
                if (executedAction.Name == action.Name)
                {
                    AlreadyExecutedActions.Remove(executedAction);
                    return;
                }
            }
        }

        public bool IsAlreadyExecutedAction(GenericAction action)
        {
            bool result = false;

            if (action.IsRealAction)
            {
                foreach (GenericAction executedAction in AlreadyExecutedActions)
                {
                    if (executedAction.Name == action.Name)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        // DICE MODIFICATIONS

        // GENERATE LIST OF AVAILABLE DICE MODIFICATIONS

        public void GenerateDiceModifications(DiceModificationTimingType type)
        {
            switch (type)
            {
                case DiceModificationTimingType.Normal:
                    GenerateDiceModificationsNormal();
                    break;
                case DiceModificationTimingType.AfterRolled:
                    GenerateDiceModificationsAfterRolled();
                    break;
                case DiceModificationTimingType.Opposite:
                    GenerateDiceModificationsOpposite();
                    break;
                case DiceModificationTimingType.CompareResults:
                    GenerateDiceModificationsCompareResults();
                    break;
                default:
                    break;
            }
        }

        private void GenerateDiceModificationsNormal()
        {
            AvailableDiceModifications = new List<GenericAction>(); ;

            //OLD
            foreach (GenericToken token in Tokens.GetAllTokens())
            {
                GenericAction action = token.GetAvailableEffects();
                if (action != null) AddAvailableDiceModificationOwn(action);
            }

            OnGenerateDiceModifications?.Invoke(this);

            OnGenerateDiceModificationsGlobal?.Invoke(this);
        }

        private void GenerateDiceModificationsAfterRolled()
        {
            AvailableDiceModifications = new List<GenericAction>(); ;

            OnGenerateDiceModificationsAfterRolled?.Invoke(this);

            OnGenerateDiceModificationsAfterRolledGlobal?.Invoke(this);
        }

        private void GenerateDiceModificationsOpposite()
        {
            AvailableDiceModifications = new List<GenericAction>();

            OnGenerateDiceModificationsOpposite?.Invoke(this);

            OnGenerateDiceModificationsOppositeGlobal?.Invoke(this);
        }

        private void GenerateDiceModificationsCompareResults()
        {
            AvailableDiceModifications = new List<GenericAction>();

            OnGenerateDiceModificationsCompareResults?.Invoke(this);

            OnGenerateDiceModificationsCompareResultsGlobal?.Invoke(this);
        }

        // ADD DICE MODIFICATION TO A LIST

        //Only for own dice modifications
        public void AddAvailableDiceModificationOwn(GenericAction action)
        {
            AddAvailableDiceModification(action, this);
        }

        //Can be used for own and aura-like dice modifications
        public void AddAvailableDiceModification(GenericAction action, GenericShip sourceShip)
        {
            action.HostShip = sourceShip;
            action.DiceModificationShip = this;
            if (NotAlreadyAddedSameDiceModification(action) && CanUseDiceModification(action))
            {
                AvailableDiceModifications.Add(action);
            }
        }

        private bool NotAlreadyAddedSameDiceModification(GenericAction action)
        {
            // Returns true if AvailableActionEffects doesn't contain action with the same name
            return !AvailableDiceModifications.Any(n => n.Name == action.Name);
        }

        public bool CanUseDiceModification(GenericAction action)
        {
            bool result = true;

            if (!action.IsDiceModificationAvailable()) result = false;

            if (IsDiceModificationAlreadyUsed(action)) result = false;

            if (result)
            {
                switch (action.DiceModificationTiming)
                {
                    case DiceModificationTimingType.Normal:
                        OnTryAddAvailableDiceModification?.Invoke(this, action, ref result);
                        OnTryAddAvailableDiceModificationGlobal?.Invoke(this, action, ref result);
                        break;
                    case DiceModificationTimingType.Opposite:
                        OnTryAddDiceModificationOpposite?.Invoke(this, action, ref result);
                        break;
                    case DiceModificationTimingType.AfterRolled:
                        OnTryAddDiceModificationAfterRolled?.Invoke(this, action, ref result);
                        break;
                    case DiceModificationTimingType.CompareResults:
                        OnTryAddDiceModificationCompareResults?.Invoke(action, ref result);
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public void AddAlreadyUsedDiceModification(GenericAction action)
        {
            if (!action.CanBeUsedFewTimes) AlreadyUsedDiceModifications.Add(action);
        }

        public void RemoveAlreadyUsedDiceModification(GenericAction action)
        {
            AlreadyUsedDiceModifications.RemoveAll(a => a.Name == action.Name);
        }

        public void ClearAlreadyUsedDiceModifications()
        {
            AlreadyUsedDiceModifications = new List<GenericAction>();
        }

        private bool IsDiceModificationAlreadyUsed(GenericAction action)
        {
            bool result = false;

            foreach (GenericAction alreadyUsedAction in AlreadyUsedDiceModifications)
            {
                if (alreadyUsedAction.DiceModificationName == action.DiceModificationName)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public List<GenericAction> GetDiceModificationsGenerated()
        {
            return AvailableDiceModifications;
        }

        // TOKENS

        public void CallBeforeAssignToken(GenericToken token, Action callback)
        {
            BeforeTokenIsAssigned?.Invoke(this, token);
            BeforeTokenIsAssignedGlobal?.Invoke(this, token);

            Triggers.ResolveTriggers(TriggerTypes.OnBeforeTokenIsAssigned, callback);
        }

        public void CallOnTokenIsAssigned(GenericToken token, Action callback)
        {
            TokensChangePanel.CreateTokensChangePanel(this, token, isAssigned: true);

            OnTokenIsAssigned?.Invoke(this, token);
            OnTokenIsAssignedGlobal?.Invoke(this, token);

            Tokens.TokenToAssign = null;

            Triggers.ResolveTriggers(TriggerTypes.OnTokenIsAssigned, callback);
        }

        public void CallOnConditionIsAssigned(GenericToken token)
        {
            OnConditionIsAssigned?.Invoke(this, token);
        }

        public void CallOnConditionIsRemoved(GenericToken token)
        {
            OnConditionIsRemoved?.Invoke(this, token);
        }

        public bool CanRemoveToken(GenericToken token)
        {
            bool result = true;

            OnBeforeTokenIsRemoved?.Invoke(this, token, ref result);

            OnBeforeTokenIsRemovedGlobal?.Invoke(this, token, ref result);

            return result;
        }

        public void CallOnRemoveTokenEvent(GenericToken token)
        {
            TokensChangePanel.CreateTokensChangePanel(this, token, isAssigned: false);

            OnTokenIsRemoved?.Invoke(this, token);
            OnTokenIsRemovedGlobal?.Invoke(this, token);
        }

        public void CallOnTargetLockIsAcquiredEvent(ITargetLockable target, Action callback)
        {
            OnTargetLockIsAcquired?.Invoke(target);
            OnTargetLockIsAcquiredGlobal?.Invoke(this, target);

            Triggers.ResolveTriggers(TriggerTypes.OnTargetLockIsAcquired, callback);
        }

        public void ChooseTargetToAcquireTargetLock(Action callback, string abilityName, IImageHolder imageSource, Func<GenericShip, bool> shipFilter = null)
        {
            AcquireTargetLockSubPhase selectTargetLockSubPhase = (AcquireTargetLockSubPhase)Phases.StartTemporarySubPhaseNew(
                "Select target for Target Lock",
                typeof(AcquireTargetLockSubPhase),
                delegate
                {
                    UI.HideSkipButton();
                    Phases.FinishSubPhase(typeof(AcquireTargetLockSubPhase));
                    callback();
                });

            if (shipFilter != null) selectTargetLockSubPhase.FilterShipTargets = shipFilter;

            selectTargetLockSubPhase.RequiredPlayer = Owner.PlayerNo;
            selectTargetLockSubPhase.DescriptionShort = abilityName;
            selectTargetLockSubPhase.ImageSource = imageSource;
            selectTargetLockSubPhase.Start();
        }

        public void CallFinishSpendToken(GenericToken token, Action callback)
        {
            OnTokenIsSpent?.Invoke(this, token);

            OnTokenIsSpentGlobal?.Invoke(this, token);

            if (Combat.SpentTokens.ContainsKey(this))
            {
                Combat.SpentTokens[this].Add(token.GetType());
            }
            else
            {
                Combat.SpentTokens.Add(this, new List<Type>() { token.GetType() });
            }

            Triggers.ResolveTriggers(TriggerTypes.OnTokenIsSpent, callback);
        }

        public bool ShouldRemoveTokenInEndPhase(GenericToken token)
        {
            bool remove = token.Temporary;
            BeforeRemovingTokenInEndPhaseGlobal?.Invoke(this, token, ref remove);
            BeforeRemovingTokenInEndPhase?.Invoke(this, token, ref remove);
            return remove;
        }

        // Coordinate

        public void CallCoordinateTargetIsSelected(GenericShip targetShip, Action callback)
        {
            OnCoordinateTargetIsSelected?.Invoke(targetShip);

            Triggers.ResolveTriggers(TriggerTypes.OnCoordinateTargetIsSelected, callback);
        }

        // Jam action

        public void CallJamTargetIsSelected(GenericShip targetShip, Action callback)
        {
            OnJamTargetIsSelected?.Invoke(targetShip);

            Triggers.ResolveTriggers(TriggerTypes.OnJamTargetIsSelected, callback);
        }

        // Reroll is confirmed

        public void CallRerollIsConfirmed(Action callback)
        {
            OnRerollIsConfirmed?.Invoke(this);

            Triggers.ResolveTriggers(TriggerTypes.OnRerollIsConfirmed, callback);
        }

        // Decloak

        public void CallOnBeforeDecloak(Action callback)
        {
            OnBeforeDecloak?.Invoke();

            Triggers.ResolveTriggers(TriggerTypes.OnBeforeDecloak, callback);
        }


        public void CallDecloak(Action callback)
        {
            OnDecloak?.Invoke();

            Triggers.ResolveTriggers(TriggerTypes.OnDecloak, callback);
        }

        // SLAM

        public void CallSlam(Action callback)
        {
            OnSlam?.Invoke();

            Triggers.ResolveTriggers(TriggerTypes.OnSlam, callback);
        }

        public ActionColor CallOnCheckActionComplexity(GenericAction action, ref ActionColor color)
        {
            OnCheckActionComplexity?.Invoke(action, ref color);
            return color;
        }

        public ActionColor CallOnCheckActionColor(GenericAction action, ref ActionColor color)
        {
            OnCheckActionColor?.Invoke(action, ref color);
            return color;
        }

        // ArcFacing

        public List<ArcFacing> GetAvailableArcFacings()
        {
            List<ArcFacing> availableArcFacings = new()
            {
                ArcFacing.Front,
                ArcFacing.Left,
                ArcFacing.Right,
                ArcFacing.Rear
            };

            OnGetAvailableArcFacings?.Invoke(availableArcFacings);

            return availableArcFacings;
        }

        // Action is failed

        public void CallActionIsReadyToBeFailed(GenericAction action, List<ActionFailReason> failReasons, bool hasSecondChance = false)
        {
            bool isDefaultFailOverwritten = false;

            if (action is CloakAction) isDefaultFailOverwritten = true;
            OnActionIsReadyToBeFailed?.Invoke(action, failReasons, ref isDefaultFailOverwritten);

            Triggers.ResolveTriggers(
                TriggerTypes.OnActionIsReadyToBeFailed,
                delegate
                {
                    CallOnActionIsReallyFailed(action, isDefaultFailOverwritten, hasSecondChance);
                }
            );
        }

        private void CallOnActionIsReallyFailed(GenericAction action, bool isDefaultFailOverwritten, bool hasSecondChance)
        {
            if (hasSecondChance)
            {
                Edition.Current.ActionIsFailed(this, action, isDefaultFailOverwritten, hasSecondChance);
            }
            else
            {
                if (action.IsRed)
                {
                    if (!isDefaultFailOverwritten) Messages.ShowError("The attempted red action has failed, this ship gains a stress token");
                    this.Tokens.AssignToken(
                        typeof(StressToken),
                        delegate
                        {
                            CallResolveActionIsReallyFailed(action, isDefaultFailOverwritten, hasSecondChance);
                        }
                    );
                }
                else
                {
                    if (!isDefaultFailOverwritten) Messages.ShowError("The attempted action has failed");
                    CallResolveActionIsReallyFailed(action, isDefaultFailOverwritten, hasSecondChance);
                }
            }
        }

        private void CallResolveActionIsReallyFailed(GenericAction action, bool isDefaultFailOverwritten, bool hasSecondChance)
        {
            if (!isDefaultFailOverwritten)
            {
                OnActionIsReallyFailed?.Invoke(action);

                Triggers.ResolveTriggers(
                    TriggerTypes.OnActionIsReallyFailed,
                    delegate
                    {
                        Edition.Current.ActionIsFailed(this, action, isDefaultFailOverwritten, hasSecondChance);
                    }
                );
            }
            else
            {
                Edition.Current.ActionIsFailed(this, action, isDefaultFailOverwritten, hasSecondChance);
            }
        }

        public void CallAfterModifyDefenseDiceStep(Action callback)
        {
            OnAfterModifyDefenseDiceStep?.Invoke(this);

            Triggers.ResolveTriggers(TriggerTypes.OnAfterModifyDefenseDiceStep, callback);
        }

        public void CallPerformActionStepStart()
        {
            OnPerformActionStepStart?.Invoke(this);
        }

        // ITargetLockable

        public int GetRangeToShip(GenericShip ship)
        {
            DistanceInfo distanceInfo = new(ship, this);
            return distanceInfo.Range;
        }

        public void AssignToken(RedTargetLockToken token, Action callback)
        {
            Tokens.AssignToken(token, callback);
        }

        public List<char> GetTargetLockLetterPairsOn(ITargetLockable targetShip)
        {
            return Tokens.GetTargetLockLetterPairsOn(targetShip);
        }

        public GenericTargetLockToken GetAnotherToken(Type oppositeType, char letter)
        {
            return Tokens.GetToken(oppositeType, letter) as GenericTargetLockToken;
        }

        public void RemoveToken(GenericToken otherTargetLockToken)
        {
            Tokens.GetAllTokens().Remove(otherTargetLockToken);
        }

        public void CallActionTargetIsWrong(GenericAction action, GenericShip wrongTarget, Action callback)
        {
            OnActionTargetIsWrong?.Invoke(action, wrongTarget);

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, callback);
        }

        public CoordinateActionData CallCheckCoordinateModeModification()
        {
            CoordinateActionData coordinateActionData = new(this);
            OnCheckCoordinateModeModification?.Invoke(ref coordinateActionData);
            return coordinateActionData;
        }

        public bool CallCheckCanCoordinate(GenericShip ship)
        {
            bool result = true;
            OnCheckCanCoordinate?.Invoke(ship, ref result);
            return result;
        }

        public bool CallCanPerformActionWhileIonized(GenericAction action)
        {
            bool canPerformActionsWhileIonized = false;
            OnCanPerformActionWhileIonized?.Invoke(action, ref canPerformActionsWhileIonized);
            return canPerformActionsWhileIonized;
        }

        public bool CallCanPerformActionWhileStressed(GenericAction action)
        {
            bool canPerformActionsWhileStressed = false;
            OnCanPerformActionWhileStressed?.Invoke(action, ref canPerformActionsWhileStressed);
            return canPerformActionsWhileStressed;
        }

        // Only notify to don't skip action step
        public bool CallCheckCanPerformActionsWhileStressed()
        {
            bool canPerformActionsWhileStressed = false;
            OnCheckCanPerformActionsWhileStressed?.Invoke(ref canPerformActionsWhileStressed);
            return canPerformActionsWhileStressed;
        }

        public TokensManager GetTokens()
        {
            return this.Tokens;
        }
    }
}