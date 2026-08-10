using Actions;
using ActionsList;
using BoardTools;
using Bombs;
using Editions;
using Movement;
using Obstacles;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SubPhases
{
    public class DecloakDecisionSubPhase : DecisionSubPhase
    {
        public override void PrepareDecision(Action callBack)
        {
            DescriptionShort = "Do you want to decloak?";

            DecisionOwner = Selection.ThisShip.Owner;

            AddDecision("Yes", Decloak);
            AddDecision("No", SkipDecloak);

            AddTooltip("Yes", "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/Decloak.png");

            DefaultDecisionName = "No";

            callBack();
        }

        private void Decloak(object sender, EventArgs e)
        {
            Phases.CurrentSubPhase.Pause();
            UI.CallHideTooltip();

            Selection.ThisShip.CallOnBeforeDecloak(StartDecloak);
        }

        private void StartDecloak()
        {
            Phases.StartTemporarySubPhaseOld(
                "Decloak",
                typeof(DecloakPlanningSubPhase),
                Phases.CurrentSubPhase.CallBack
            );
        }

        private void SkipDecloak(object sender, EventArgs e)
        {
            UI.CallHideTooltip();
            CallBack();
        }
    }

    public class DecloakPlanningSubPhase : GenericSubPhase
    {
        public string SelectedBoostHelper { get; private set; }
        public GameObject TemporaryShipBase { get; private set; }

        protected GenericAction HostAction { get; set; }

        protected List<ManeuverTemplate> AvailableBoostTemplates = new();
        protected List<ManeuverTemplate> AvailableBarrelRollTemplates = new();

        //Saves forward-center-bottom temporary ship bases and their collisions
        List<DecloakBarrelRollShiftData> BarrelRollShiftVariants = new();

        public ObstaclesStayDetectorForced TemporaryBaseCollider
        {
            get
            {
                return BarrelRollShiftVariants.First(n => n.Direction == SelectedShift).Collider;
            }
        }
        public GameObject TemporaryShipBaseFinal;

        protected ManeuverTemplate SelectedTemplate;

        protected Direction SelectedDirectionPrimary;
        protected Direction SelectedDirectionSecondary;
        protected Direction SelectedShift;

        public bool IsTractorBeamBarrelRoll = false;
        public bool IsIgnoreObstacles = false;

        private Players.GenericPlayer controller;
        public Players.GenericPlayer Controller
        {
            get
            {
                return controller ?? TheShip.Owner;
            }
            set
            {
                controller = value;
            }
        }

        public List<ActionFailReason> BarrelRollProblems { get; private set; } = new List<ActionFailReason>();

        public bool inReposition;

        public override void Start()
        {
            Name = "Decloak planning";
            IsTemporary = true;
            UpdateHelpInfo();

            StartDecloakPlanning();
        }

        protected void StartDecloakPlanning()
        {
            AskToSelectTemplate(PerformTemplatePlanning);
        }

        private void AskToSelectTemplate(Action callback)
        {
            GenerateListOfAvailableTemplates();

            if (AvailableBarrelRollTemplates.Count > 0 && AvailableBoostTemplates.Count > 0)
            {
                RegisterDirectionDecisionTrigger(callback);
            }
        }

        public void PerformTemplatePlanning()
        {
            Edition.Current.DecloakTemplatePlanning();
        }

        protected void GenerateListOfAvailableTemplates()
        {
            AvailableBarrelRollTemplates.AddRange(Selection.ThisShip.GetAvailableDecloakBarrelRollTemplates());

            AvailableBoostTemplates.AddRange(Selection.ThisShip.GetAvailableDecloakBoostTemplates());
        }

        protected void StartRepositionExecutionSubphase()
        {
            Pause();

            TheShip.ToggleShipStandAndPeg(false);

            if (SelectedDirectionPrimary == Direction.Top)
            {
                DecloakBoostExecutionSubPhase boostExecutionSubPhase = (DecloakBoostExecutionSubPhase)Phases.StartTemporarySubPhaseNew(
                    "Boost execution",
                    typeof(DecloakBoostExecutionSubPhase),
                    CallBack
                );
                boostExecutionSubPhase.TheShip = TheShip;
                boostExecutionSubPhase.IsTractorBeamBoost = IsTractorBeamBarrelRoll;
                boostExecutionSubPhase.SelectedBoostHelper = SelectedBoostHelper;
                boostExecutionSubPhase.FinalPositionInfo = new ShipPositionInfo(TemporaryShipBase.transform.position, TemporaryShipBase.transform.eulerAngles);
                boostExecutionSubPhase.Start();
            }
            else
            {
                DecloakBarrelRollExecutionSubPhase rollExecutionSubphase = (DecloakBarrelRollExecutionSubPhase)Phases.StartTemporarySubPhaseNew(
                    "Barrel Roll execution",
                    typeof(DecloakBarrelRollExecutionSubPhase),
                    CallBack
                );

                rollExecutionSubphase.TheShip = TheShip;
                rollExecutionSubphase.TemporaryShipBase = TemporaryShipBaseFinal;
                rollExecutionSubphase.Direction = SelectedDirectionPrimary;
                rollExecutionSubphase.IsTractorBeamBarrelRoll = IsTractorBeamBarrelRoll;

                rollExecutionSubphase.Start();
            }
        }

        protected void GenerateSelectTemplateDecisions(DecisionSubPhase subphase)
        {
            // Boost Templates
            foreach (ManeuverTemplate template in AvailableBoostTemplates)
            {
                switch (template.Bearing)
                {
                    case ManeuverBearing.Straight:
                        subphase.AddDecision(
                            "Boost " + template.NameNoDirection,
                            (EventHandler)delegate
                            {
                                SelectTemplate(template, Direction.Top);
                                DecisionSubPhase.ConfirmDecision();
                            }
                        );
                        break;
                    case ManeuverBearing.Bank:
                    case ManeuverBearing.Turn:
                        subphase.AddDecision(
                            "Boost " + template.Name,
                            (EventHandler)delegate
                            {
                                SelectTemplate(template, Direction.Top);
                                DecisionSubPhase.ConfirmDecision();
                            }
                        );
                        break;
                }
            }

            // Barrel Roll templates
            foreach (ManeuverTemplate template in AvailableBarrelRollTemplates)
            {
                switch (template.Bearing)
                {
                    case ManeuverBearing.Straight:
                        subphase.AddDecision(
                            $"Barrel Roll Left {template.NameNoDirection}",
                            (EventHandler)delegate
                            {
                                SelectTemplate(template, Direction.Left);
                                DecisionSubPhase.ConfirmDecision();
                            }
                        );

                        subphase.AddDecision(
                            $"Barrel Roll Right {template.NameNoDirection}",
                            (EventHandler)delegate
                            {
                                SelectTemplate(template, Direction.Right);
                                DecisionSubPhase.ConfirmDecision();
                            }
                        );

                        break;
                    case ManeuverBearing.Bank:
                    case ManeuverBearing.Turn:
                        switch (template.Direction)
                        {
                            case ManeuverDirection.Left:
                                subphase.AddDecision(
                                    $"Right {template.NameNoDirection} Forward",
                                    (EventHandler)delegate
                                    {
                                        SelectTemplate(template, Direction.Right, Direction.Top);
                                        DecisionSubPhase.ConfirmDecision();
                                    }
                                );

                                subphase.AddDecision(
                                    $"Left {template.NameNoDirection} Backwards",
                                    (EventHandler)delegate
                                    {
                                        SelectTemplate(template, Direction.Left, Direction.Bottom);
                                        DecisionSubPhase.ConfirmDecision();
                                    }
                                );
                                break;
                            case ManeuverDirection.Right:
                                subphase.AddDecision(
                                    $"Left {template.NameNoDirection} Forward",
                                    (EventHandler)delegate
                                    {
                                        SelectTemplate(template, Direction.Left, Direction.Top);
                                        DecisionSubPhase.ConfirmDecision();
                                    }
                                );

                                subphase.AddDecision(
                                    $"Right {template.NameNoDirection} Backwards",
                                    (EventHandler)delegate
                                    {
                                        SelectTemplate(template, Direction.Right, Direction.Bottom);
                                        DecisionSubPhase.ConfirmDecision();
                                    }
                                );
                                break;
                        }

                        break;
                }
            }
        }

        protected IEnumerator CheckCollisionsOfTemporaryElements(Action callback)
        {
            if (SelectedDirectionPrimary == Direction.Top)
            {
                ShowBoosterHelper();
            }
            else
            {
                yield return CheckTemplate();

                if (!IsColliderDataAllowed(SelectedTemplate.Collider))
                {
                    CancelBarrelRoll();
                }
                else
                {
                    yield return CheckPotentialFinalPositions();

                    if (IsPotentialFinalPositionsAnyAllowed())
                    {
                        callback();
                    }
                    else
                    {
                        CancelBarrelRoll();
                    }
                }
            }
        }

        private void ShowBoosterHelper()
        {
            SelectedBoostHelper = $"{SelectedTemplate.Bearing} {(SelectedTemplate.Speed == ManeuverSpeed.Speed1 ? "1" : "2")}"
                + (SelectedTemplate.Direction == ManeuverDirection.Forward ? "" : $" {SelectedTemplate.Direction}")
                + (SelectedDirectionSecondary == Direction.None ? "" : $" {SelectedDirectionSecondary}");

            TheShip.GetBoosterHelper().Find(SelectedBoostHelper).gameObject.SetActive(true);

            Transform newBase = TheShip.GetBoosterHelper().Find(SelectedBoostHelper + "/Finisher/BasePosition");

            GameObject prefab = (GameObject)Resources.Load(TheShip.ShipBase.TemporaryPrefabPath, typeof(GameObject));
            TemporaryShipBase = MonoBehaviour.Instantiate(prefab, TheShip.GetPosition(), TheShip.GetRotation(), Board.GetBoard());
            TemporaryShipBase.transform.SetPositionAndRotation(new Vector3(newBase.position.x, 0, newBase.position.z), newBase.rotation);
            GameManagerScript.Instance.StartCoroutine(
                CheckCollisionsOfTemporaryBoostElements(FinishBoost)
            );
        }

        protected IEnumerator CheckCollisionsOfTemporaryBoostElements(Action callback)
        {
            BarrelRollProblems.Clear();

            ObstaclesStayDetectorForced obstaclesStayDetectorMovementTemplate = TheShip.GetBoosterHelper().Find(SelectedBoostHelper).GetComponentInChildren<ObstaclesStayDetectorForced>();
            obstaclesStayDetectorMovementTemplate.TheShip = TheShip;

            TemporaryShipBase.transform.Find("ShipBase").Find("ObstaclesStayDetector").gameObject.AddComponent<ObstaclesStayDetectorForced>();
            ObstaclesStayDetectorForced obstaclesStayDetectorNewBase = TemporaryShipBase.GetComponentInChildren<ObstaclesStayDetectorForced>();

            yield return CheckBoostTemplate(obstaclesStayDetectorMovementTemplate);

            yield return CheckBoostTemplate(obstaclesStayDetectorNewBase);

            if (!IsBoostTemplateColliderDataAllowed(obstaclesStayDetectorMovementTemplate) || !IsBoostTemplateNewBaseDataAllowed(obstaclesStayDetectorNewBase))
            {
                CancelBoost();
            }
            else
            {
                callback();
            }
        }

        private void CancelBoost()
        {
            HidePlanningTemplates();

            ShowInformationAboutBoostProblems();

            GameModeCancelBoost();
        }

        protected virtual void GameModeCancelBoost()
        {
            HostAction ??= new BoostAction() { HostShip = TheShip };
            Rules.Actions.ActionIsFailed(TheShip, HostAction, BarrelRollProblems);
        }

        private void ShowInformationAboutBoostProblems()
        {
            foreach (ActionFailReason problem in BarrelRollProblems)
            {
                switch (problem)
                {
                    case ActionFailReason.Bumped:
                        Messages.ShowError("Boost would cause this ship to overlap another ship");
                        break;
                    case ActionFailReason.OffTheBoard:
                        Messages.ShowError("Boost would cause this ship to leave the battlefield");
                        break;
                    case ActionFailReason.ObstacleHit:
                        Messages.ShowError("Boost would cause this ship to overlap an obstacle");
                        break;
                    default:
                        break;
                }
            }
        }

        private bool IsBoostTemplateColliderDataAllowed(ObstaclesStayDetectorForced collider)
        {
            if (!TheShip.IsIgnoreObstacles
                && !TheShip.IsIgnoreObstaclesDuringBarrelRoll()
                && !IsIgnoreObstacles
                && collider.OverlapsAsteroidNow)
            {
                BarrelRollProblems.Add(ActionFailReason.ObstacleHit);
            }
            else if (collider.OffTheBoardNow)
            {
                BarrelRollProblems.Add(ActionFailReason.OffTheBoard);
            }

            return BarrelRollProblems.Count == 0;
        }

        private bool IsBoostTemplateNewBaseDataAllowed(ObstaclesStayDetectorForced collider)
        {
            if (collider.OverlapsShipNow)
            {
                BarrelRollProblems.Add(ActionFailReason.Bumped);
            }
            else if (!TheShip.IsIgnoreObstacles
                && !TheShip.IsIgnoreObstaclesDuringBarrelRoll()
                && !IsIgnoreObstacles
                && collider.OverlapsAsteroidNow)
            {
                BarrelRollProblems.Add(ActionFailReason.ObstacleHit);
            }
            else if (collider.OffTheBoardNow)
            {
                BarrelRollProblems.Add(ActionFailReason.OffTheBoard);
            }

            return BarrelRollProblems.Count == 0;
        }

        private IEnumerator CheckBoostTemplate(ObstaclesStayDetectorForced obstaclesStayDetectorMovementTemplate)
        {
            obstaclesStayDetectorMovementTemplate.ReCheckCollisionsStart();

            yield return Tools.WaitForFrames(3);
        }

        private void FinishBoost()
        {
            HidePlanningTemplates();

            StartRepositionExecution();
        }

        private void HidePlanningTemplates()
        {
            TheShip.GetBoosterHelper().Find(SelectedBoostHelper).gameObject.SetActive(false);
            MonoBehaviour.Destroy(TemporaryShipBase);

            Roster.SetRaycastTargets(true);
        }

        public void PerformTemplatePlanningSecondEdition()
        {
            GameManagerScript.Instance.StartCoroutine(
                CheckCollisionsOfTemporaryElements(AskBarrelRollShift)
            );
        }

        private void AskBarrelRollShift()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Barrel Roll position",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = Controller.PlayerNo,
                EventHandler = StartAskBarrelRollShiftSubphase
            });

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, ConfirmBarrelRollPosition);
        }

        public void ConfirmBarrelRollPosition()
        {
            CheckBarrelRollThroughObstacle();
            CheckMines();
            SyncCollisions(TemporaryBaseCollider);
            DestroyTemporaryElements();

            StartRepositionExecution();
        }

        private void CheckBarrelRollThroughObstacle()
        {
            if (SelectedTemplate.Collider.OverlapsAsteroidNow || TemporaryBaseCollider.OverlapsAsteroidNow)
            {
                if (HostAction is BarrelRollAction)
                {
                    (HostAction as BarrelRollAction).IsThroughObstacle = true;
                }
            }
        }

        public void StartRepositionExecution()
        {
            StartRepositionExecutionSubphase();
        }

        private void RegisterDirectionDecisionTrigger(Action callback)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Select direction and template",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = Controller.PlayerNo,
                EventHandler = StartSelectTemplateSubphase
            });

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, callback);
        }

        protected void StartSelectTemplateSubphase(object sender, EventArgs e)
        {
            DecloakDirectionDecisionSubPhase selectMoveTemplate = (DecloakDirectionDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(DecloakDirectionDecisionSubPhase),
                Triggers.FinishTrigger
            );

            GenerateSelectTemplateDecisions(selectMoveTemplate);

            selectMoveTemplate.DescriptionShort = "Decloak: Select template";

            selectMoveTemplate.DefaultDecisionName = selectMoveTemplate.GetDecisions().First().Name;

            selectMoveTemplate.RequiredPlayer = Controller.PlayerNo;

            selectMoveTemplate.Start();
        }

        public void SelectTemplate(ManeuverTemplate template, Direction directionPrimary, Direction directionSecondary = Direction.None)
        {
            SelectedTemplate = template;
            SelectedDirectionPrimary = directionPrimary;
            SelectedDirectionSecondary = directionSecondary;
        }

        protected virtual void CancelBarrelRoll()
        {
            DestroyTemporaryElements(isAll: true);
            ShowInformationAboutProblems();

            WhenCancelBarrelRollWithProblems(BarrelRollProblems);
        }

        private void ShowInformationAboutProblems()
        {
            foreach (ActionFailReason problem in BarrelRollProblems)
            {
                switch (problem)
                {
                    case ActionFailReason.Bumped:
                        Messages.ShowError("Barrel Roll would cause this ship to overlap another ship");
                        break;
                    case ActionFailReason.OffTheBoard:
                        Messages.ShowError("Barrel Roll would cause this ship to leave the battlefield");
                        break;
                    case ActionFailReason.ObstacleHit:
                        Messages.ShowError("Barrel Roll would cause this ship to overlap an obstacle");
                        break;
                    default:
                        break;
                }
            }
        }

        private IEnumerator CheckTemplate()
        {
            ShowBarrelRollTemplate();

            SelectedTemplate.Collider.TheShip = TheShip;
            SelectedTemplate.Collider.ReCheckCollisionsStart();

            yield return Tools.WaitForFrames(3);
        }

        private bool IsColliderDataAllowed(ObstaclesStayDetectorForced collider, bool isBaseFinalPosition = false)
        {
            if (collider.OverlapsShipNow && isBaseFinalPosition)
            {
                BarrelRollProblems.Add(ActionFailReason.Bumped);
            }
            else if (!TheShip.IsIgnoreObstacles
                && !TheShip.IsIgnoreObstaclesDuringBarrelRoll()
                && !IsIgnoreObstacles
                && collider.OverlapsAsteroidNow
                && !TheShip.IgnoreObstacleTypes.Contains(typeof(Asteroid)))
            {
                BarrelRollProblems.Add(ActionFailReason.ObstacleHit);
            }
            else if (collider.OffTheBoardNow)
            {
                BarrelRollProblems.Add(ActionFailReason.OffTheBoard);
            }

            return BarrelRollProblems.Count == 0;
        }

        private IEnumerator CheckPotentialFinalPositions()
        {
            List<Direction> directions = new() {
                Direction.Top,
                Direction.None,
                Direction.Bottom
            };

            foreach (Direction direction in directions)
            {
                DecloakBarrelRollShiftData currentData = new(
                    direction,
                    ShowTemporaryShipBase(direction, isVisible: false)
                );

                BarrelRollShiftVariants.Add(currentData);
                yield return currentData.CheckCollisions();
            }
        }

        private bool IsPotentialFinalPositionsAnyAllowed()
        {
            bool isAllowed = false;

            foreach (DecloakBarrelRollShiftData barrelRollData in BarrelRollShiftVariants)
            {
                BarrelRollProblems = new List<ActionFailReason>();
                if (IsColliderDataAllowed(barrelRollData.Collider, isBaseFinalPosition: true))
                {
                    isAllowed = true;
                }
            }

            return isAllowed;
        }

        private void StartAskBarrelRollShiftSubphase(object sender, EventArgs e)
        {
            DecloakPositionDecisionSubPhase selectDecloakPosition = (DecloakPositionDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                    Name,
                    typeof(DecloakPositionDecisionSubPhase),
                    Triggers.FinishTrigger
            );

            selectDecloakPosition.AddDecision("Forward", delegate { SetBarrelRollPosition(Direction.Top); }, isCentered: true);
            selectDecloakPosition.AddDecision("Center", delegate { SetBarrelRollPosition(Direction.None); }, isCentered: true);
            selectDecloakPosition.AddDecision("Backwards", delegate { SetBarrelRollPosition(Direction.Bottom); }, isCentered: true);

            selectDecloakPosition.DescriptionShort = "Barrel Roll: Select position";

            selectDecloakPosition.DefaultDecisionName = "Center";

            selectDecloakPosition.RequiredPlayer = Controller.PlayerNo;

            selectDecloakPosition.ShowSkipButton = false;
            selectDecloakPosition.OnNextButtonIsPressed = DecisionSubPhase.ConfirmDecision;

            selectDecloakPosition.Start();
        }

        private void SetBarrelRollPosition(Direction direction)
        {
            SelectedShift = direction;

            foreach (DecloakBarrelRollShiftData barrelRollShiftVariant in BarrelRollShiftVariants)
            {
                ToggleTemporaryShipBaseVisibility(
                    barrelRollShiftVariant.TemporaryShipBase,
                    barrelRollShiftVariant.Direction == SelectedShift
                );
            }

            BarrelRollProblems = new List<ActionFailReason>();

            if (!IsColliderDataAllowed(TemporaryBaseCollider, isBaseFinalPosition: true))
            {
                Messages.ShowError("This final position is not valid, choose another position");
                UI.HideNextButton();
            }
            else
            {
                UI.ShowNextButton();
                UI.HighlightNextButton();
            }

            DecisionSubPhase.ResetInput();
        }

        private void ShowBarrelRollTemplate()
        {
            SelectedTemplate.ApplyTemplate(
                TheShip,
                (SelectedDirectionPrimary == Direction.Left) ? TheShip.GetLeft() : TheShip.GetRight(),
                SelectedDirectionPrimary
            );
        }

        private GameObject ShowTemporaryShipBase(Direction shiftDirection, bool isVisible = true)
        {
            GameObject prefab = (GameObject)Resources.Load(TheShip.ShipBase.TemporaryPrefabPath, typeof(GameObject));
            GameObject temporaryShipBase = MonoBehaviour.Instantiate(
                prefab,
                SelectedTemplate.GetFinalPosition(),
                SelectedTemplate.GetFinalRotation(),
                Board.GetBoard()
            );

            int directionModifier = (SelectedDirectionPrimary == Direction.Left) ? -1 : 1;

            float finalShift = 0;
            switch (shiftDirection)
            {
                case Direction.Top:
                    finalShift += (SelectedTemplate.IsSideTemplate) ? 0.5f : 0.25f;
                    break;
                case Direction.Bottom:
                    finalShift -= (SelectedTemplate.IsSideTemplate) ? 0.5f : 0.25f;
                    break;
                default:
                    break;
            }

            temporaryShipBase.transform.localEulerAngles += new Vector3(0, directionModifier * -90, 0);

            Vector3 shift = new(
                directionModifier * TheShip.ShipBase.HALF_OF_SHIPSTAND_SIZE,
                0,
                TheShip.ShipBase.HALF_OF_SHIPSTAND_SIZE + finalShift
            );
            Vector3 absPosition = temporaryShipBase.transform.TransformPoint(shift);

            temporaryShipBase.transform.position = absPosition;

            temporaryShipBase.transform.Find("ShipBase").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material = TheShip.Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipBase").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material;
            temporaryShipBase.transform.Find("ShipBase").Find("ObstaclesStayDetector").gameObject.AddComponent<ObstaclesStayDetectorForced>();

            ToggleTemporaryShipBaseVisibility(temporaryShipBase, isVisible);

            return temporaryShipBase;
        }

        private void ToggleTemporaryShipBaseVisibility(GameObject shipBase, bool isVisible)
        {
            foreach (Renderer renderer in shipBase.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = isVisible;
            }
        }

        public void WhenCancelBarrelRollWithProblems(List<ActionFailReason> barrelRollProblems)
        {
            HostAction ??= new BarrelRollAction() { HostShip = TheShip };
            Rules.Actions.ActionIsFailed(TheShip, HostAction, barrelRollProblems);
        }

        private void DestroyTemporaryElements(bool isAll = false)
        {
            foreach (DecloakBarrelRollShiftData data in BarrelRollShiftVariants)
            {
                if (data.Direction == SelectedShift && !isAll)
                {
                    TemporaryShipBaseFinal = data.TemporaryShipBase;
                }
                else
                {
                    GameObject.Destroy(data.TemporaryShipBase);
                }
            }

            BarrelRollShiftVariants = new List<DecloakBarrelRollShiftData>();
            SelectedTemplate.DestroyTemplate();
        }

        private void CheckMines()
        {
            foreach (Collider mineCollider in SelectedTemplate.Collider.OverlappedMinesNow)
            {
                GenericDeviceGameObject mineObject = mineCollider.transform.parent.GetComponent<GenericDeviceGameObject>();
                if (!TheShip.MinesHit.Contains(mineObject)) TheShip.MinesHit.Add(mineObject);
            }
        }

        private void SyncCollisions(ObstaclesStayDetectorForced collider)
        {
            TheShip.ObstaclesLanded = new List<GenericObstacle>(collider.OverlappedAsteroidsNow);
            if (!TheShip.IsIgnoreObstaclesDuringBarrelRoll())
            {
                collider.OverlappedAsteroidsNow
                    .Where((a) => !TheShip.ObstaclesHit.Contains(a)).ToList()
                    .ForEach(TheShip.ObstaclesHit.Add);
            }
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            return false;
        }

        public override bool AnotherShipCanBeSelected(GenericShip anotherShip, int mouseKeyIsPressed)
        {
            return false;
        }

        protected class DecloakDirectionDecisionSubPhase : DecisionSubPhase { }

        protected class DecloakPositionDecisionSubPhase : DecisionSubPhase { }

        private class DecloakBarrelRollShiftData
        {
            public GameObject TemporaryShipBase { get; private set; }
            public Direction Direction { get; private set; }
            public ObstaclesStayDetectorForced Collider { get; private set; }

            public DecloakBarrelRollShiftData(Direction direction, GameObject temporaryShipBase)
            {
                Direction = direction;
                TemporaryShipBase = temporaryShipBase;
            }

            public IEnumerator CheckCollisions()
            {
                Collider = TemporaryShipBase.GetComponentInChildren<ObstaclesStayDetectorForced>();
                Collider.TheShip = (Phases.CurrentSubPhase as DecloakPlanningSubPhase).TheShip;
                Collider.ReCheckCollisionsStart();

                yield return Tools.WaitForFrames(3);
            }
        }
    }

    public class DecloakBarrelRollExecutionSubPhase : BarrelRollExecutionSubPhase
    {
        protected override void FinishBarrelRollAnimationPart2()
        {
            Phases.FinishSubPhase(typeof(DecloakBarrelRollExecutionSubPhase));
            Selection.ThisShip.Tokens.SpendToken(typeof(Tokens.CloakToken), FinishDecloakAnimationPart3);
        }

        private void FinishDecloakAnimationPart3()
        {
            Selection.ThisShip.CallDecloak(CallBack);
        }
    }

    public class DecloakBoostExecutionSubPhase : BoostExecutionSubPhase
    {
        protected override void FinishBoostAnimation()
        {
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            UpdateHelpInfo();

            Selection.ThisShip.ToggleShipStandAndPeg(true);
            Selection.ThisShip.Tokens.SpendToken(typeof(Tokens.CloakToken), FinishDecloakAnimationPart3);
        }

        private void FinishDecloakAnimationPart3()
        {
            Selection.ThisShip.CallDecloak(CallBack);
        }
    }
}