using Actions;
using ActionsList;
using Assets.Scripts.Model.Tools.Enums;
using Bombs;
using Movement;
using Obstacles;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ActionsHolder;

namespace ActionsList
{

    public class BoostAction : GenericAction
    {
        public string SelectedBoostTemplate;

        public bool IsThroughObstacle { get; set; }

        public BoostAction()
        {
            Name = "Boost";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/BoostAction.png";
        }

        public override void ActionTake()
        {
            if (Selection.ThisShip.Owner.UsesHotacAiRules)
            {
                Phases.CurrentSubPhase.CallBack();
            }
            else
            {
                Phases.CurrentSubPhase.Pause();
                BoostPlanningSubPhase phase = Phases.StartTemporarySubPhaseNew<BoostPlanningSubPhase>(
                    "Boost",
                    delegate
                    {
                        SelectedBoostTemplate = null;
                        Phases.CurrentSubPhase.CallBack();
                    }
                );
                phase.SelectedBoostHelper = SelectedBoostTemplate;
                phase.HostAction = this;
                phase.Start();
            }
        }

        public override void RevertActionOnFail(bool hasSecondChance = false)
        {
            SelectedBoostTemplate = null;
            Phases.GoBack();
        }
    }

    public class BoostMove
    {
        public string Name { get; private set; }
        public BoostTemplates Template;
        public bool IsRed;
        public bool IsPurple;
        public bool IsForced { get; private set; }

        public BoostMove(BoostTemplates template, bool isRed = false, bool isPurple = false, bool isForced = false)
        {
            Template = template;
            IsRed = isRed;
            IsPurple = isPurple;
            IsForced = isForced;

            Name = template.GetDescription();
        }

        public static BoostTemplates GetBoostTemplateFromName(string name)
        {
            return EnumExtensions.GetEnumValueFromDescription<BoostTemplates>(name);
        }
    }
}

namespace SubPhases
{

    public class BoostPlanningSubPhase : GenericSubPhase
    {
        public GenericAction HostAction;
        public GameObject ShipStand;
        private ObstaclesStayDetectorForced obstaclesStayDetectorBase;
        private ObstaclesStayDetectorForced obstaclesStayDetectorMovementTemplate;

        public bool inReposition;

        private int updatesCount = 0;

        public List<BoostMove> AvailableBoostMoves = new();
        public string SelectedBoostHelper;

        public bool IsTractorBeamBoost = false;
        public bool IsIgnoreObstacles = false;

        public override void Start()
        {
            Name = "Boost planning";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBoostPlanning();
        }

        public void InitializeRendering()
        {
            GameObject prefab = (GameObject)Resources.Load(TheShip.ShipBase.TemporaryPrefabPath, typeof(GameObject));
            ShipStand = MonoBehaviour.Instantiate(prefab, TheShip.GetPosition(), TheShip.GetRotation(), BoardTools.Board.GetBoard());
            ShipStand.transform.position = new Vector3(ShipStand.transform.position.x, 0, ShipStand.transform.position.z);
            foreach (Renderer render in ShipStand.transform.Find("ShipBase").GetComponentsInChildren<Renderer>())
            {
                render.enabled = false;
            }

            ShipStand.transform.Find("ShipBase").Find("ObstaclesStayDetector").gameObject.AddComponent<ObstaclesStayDetectorForced>();
            obstaclesStayDetectorBase = ShipStand.GetComponentInChildren<ObstaclesStayDetectorForced>();
            obstaclesStayDetectorBase.TheShip = TheShip;
            Roster.SetRaycastTargets(false);
        }

        public void StartBoostPlanning()
        {
            AvailableBoostMoves = TheShip.GetAvailableBoostTemplates(HostAction);

            InitializeRendering();

            if (SelectedBoostHelper != null)
            {
                SelectTemplate(AvailableBoostMoves.First(n => n.Name == SelectedBoostHelper));
                SelectTemplateDecisionIsTaken();
            }
            else
            {
                AskSelectTemplate();
            }
        }

        private void AskSelectTemplate()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Select template for Boost",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = TheShip.Owner.PlayerNo,
                EventHandler = StartSelectTemplateDecision
            });

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, SelectTemplateDecisionIsTaken);
        }

        private void StartSelectTemplateDecision(object sender, System.EventArgs e)
        {
            SelectBoostTemplateDecisionSubPhase selectBoostTemplateDecisionSubPhase = (SelectBoostTemplateDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Select boost template decision",
                typeof(SelectBoostTemplateDecisionSubPhase),
                Triggers.FinishTrigger
            );

            foreach (BoostMove move in AvailableBoostMoves)
            {
                ActionColor color = ActionColor.White;

                if (move.IsRed)
                {
                    color = ActionColor.Red;
                }
                else if (move.IsPurple)
                {
                    color = ActionColor.Purple;
                }

                selectBoostTemplateDecisionSubPhase.AddDecision(
                    move.Name,
                    delegate
                    {
                        SelectTemplate(move);
                        DecisionSubPhase.ConfirmDecision();
                    },
                    color: color,
                    isCentered: move.Template == BoostTemplates.Straight1
                );
            }

            selectBoostTemplateDecisionSubPhase.DescriptionShort = "Select boost direction";

            selectBoostTemplateDecisionSubPhase.DefaultDecisionName = "Straight 1";

            selectBoostTemplateDecisionSubPhase.RequiredPlayer = TheShip.Owner.PlayerNo;

            selectBoostTemplateDecisionSubPhase.Start();
        }

        private class SelectBoostTemplateDecisionSubPhase : DecisionSubPhase { }

        private void SelectTemplate(BoostMove move)
        {
            if (move.IsRed && !HostAction.IsRed)
            {
                HostAction.Color = ActionColor.Red;
                TheShip.OnActionIsPerformed += ResetActionColor;
            }

            if (move.IsPurple && !HostAction.IsPurple)
            {
                HostAction.Color = ActionColor.Purple;
                TheShip.OnActionIsPerformed += ResetActionColor;
            }

            SelectedBoostHelper = move.Name;
        }

        private void ResetActionColor(GenericAction action)
        {
            action.HostShip.OnActionIsPerformed -= ResetActionColor;
            HostAction.Color = ActionColor.White;
        }

        private void SelectTemplateDecisionIsTaken()
        {
            if (SelectedBoostHelper != null)
            {
                TheShip.CallUpdateChosenBoostTemplate(ref SelectedBoostHelper);

                (HostAction as BoostAction).SelectedBoostTemplate = SelectedBoostHelper;
                TryConfirmBoostPosition();
            }
            else
            {
                CancelBoost(new List<ActionFailReason>() { ActionFailReason.NoTemplateAvailable });
            }
        }

        private void ShowBoosterHelper()
        {
            TheShip.GetBoosterHelper().Find(SelectedBoostHelper).gameObject.SetActive(true);

            Transform newBase = TheShip.GetBoosterHelper().Find(SelectedBoostHelper + "/Finisher/BasePosition");

            ShipStand.transform.SetPositionAndRotation(new Vector3(newBase.position.x, 0, newBase.position.z), newBase.rotation);

            obstaclesStayDetectorMovementTemplate = TheShip.GetBoosterHelper().Find(SelectedBoostHelper).GetComponentInChildren<ObstaclesStayDetectorForced>();
            obstaclesStayDetectorMovementTemplate.TheShip = TheShip;
        }

        public virtual void StartBoostExecution(ShipPositionInfo finalPositionInfo)
        {
            BoostExecutionSubPhase execution = (BoostExecutionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Boost execution",
                typeof(BoostExecutionSubPhase),
                CallBack
            );

            execution.TheShip = TheShip;
            execution.IsTractorBeamBoost = IsTractorBeamBoost;
            execution.SelectedBoostHelper = SelectedBoostHelper;
            execution.FinalPositionInfo = finalPositionInfo;
            execution.Start();
        }

        public void CancelBoost(List<ActionFailReason> boostProblems)
        {
            TheShip.IsLandedOnObstacle = false;

            MonoBehaviour.Destroy(ShipStand);

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.CollidedWith = null;
            MovementTemplates.HideLastMovementRuler();

            Rules.Actions.ActionIsFailed(TheShip, HostAction, boostProblems);
        }

        private void HidePlanningTemplates()
        {
            TheShip.GetBoosterHelper().Find(SelectedBoostHelper).gameObject.SetActive(false);
            MonoBehaviour.Destroy(ShipStand);

            Roster.SetRaycastTargets(true);
        }

        public void TryConfirmBoostPosition(System.Action<bool> canBoostCallback = null)
        {
            ShowBoosterHelper();

            obstaclesStayDetectorBase.ReCheckCollisionsStart();
            obstaclesStayDetectorMovementTemplate.ReCheckCollisionsStart();

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.FuncsToUpdate.Add(() => UpdateColisionDetection(canBoostCallback));
        }

        private bool UpdateColisionDetection(System.Action<bool> canBoostCallback = null)
        {
            bool isFinished = false;

            if (updatesCount > 1)
            {
                updatesCount = 0;
                GetResults(canBoostCallback);
                isFinished = true;
            }
            else
            {
                updatesCount++;
            }

            return isFinished;
        }

        private void GetResults(Action<bool> canBoostCallback = null)
        {
            obstaclesStayDetectorBase.ReCheckCollisionsFinish();
            obstaclesStayDetectorMovementTemplate.ReCheckCollisionsFinish();

            ShipPositionInfo shipPositionInfo = new(ShipStand.transform.position, ShipStand.transform.eulerAngles);

            HidePlanningTemplates();

            if (canBoostCallback != null)
            {
                canBoostCallback(CheckBoostProblems(true).Count == 0);
                return;
            }

            List<ActionFailReason> boostProblems = CheckBoostProblems();
            if (boostProblems.Count == 0)
            {
                CheckBoostThroughObstacle();
                CheckMines();
                TheShip.ObstaclesLanded = new List<GenericObstacle>(obstaclesStayDetectorBase.OverlappedAsteroidsNow);
                TheShip.ShipsBoostedThrough = new List<GenericShip>(obstaclesStayDetectorMovementTemplate.OverlappedShipsNow);
                obstaclesStayDetectorMovementTemplate.OverlappedAsteroidsNow
                    .Where((a) => !TheShip.ObstaclesHit.Contains(a)).ToList()
                    .ForEach(TheShip.ObstaclesHit.Add);
                StartBoostExecution(shipPositionInfo);
            }
            else
            {
                CancelBoost(boostProblems);
            }
        }

        private void CheckBoostThroughObstacle()
        {
            if (obstaclesStayDetectorBase.OverlapsAsteroidNow || obstaclesStayDetectorMovementTemplate.OverlapsAsteroidNow)
            {
                if (HostAction is BoostAction)
                {
                    (HostAction as BoostAction).IsThroughObstacle = true;
                }
            }
        }

        private void CheckMines()
        {
            foreach (Collider mineCollider in obstaclesStayDetectorMovementTemplate.OverlappedMinesNow)
            {
                GenericDeviceGameObject mineObject = mineCollider.transform.parent.GetComponent<GenericDeviceGameObject>();
                if (!TheShip.MinesHit.Contains(mineObject)) TheShip.MinesHit.Add(mineObject);
            }
        }

        private List<ActionFailReason> CheckBoostProblems(bool quiet = false)
        {
            List<ActionFailReason> result = new();

            if (obstaclesStayDetectorBase.OverlapsShipNow)
            {
                if (!quiet) Messages.ShowError("That Boost action is not allowed, as it results in this ship overlapping another ship");
                result.Add(ActionFailReason.Bumped);
            }
            else if (!TheShip.IsIgnoreObstacles && !TheShip.IsIgnoreObstaclesDuringBoost() && !IsIgnoreObstacles
                && (obstaclesStayDetectorBase.OverlapsAsteroidNow || obstaclesStayDetectorMovementTemplate.OverlapsAsteroidNow))
            {
                Debug.Log("allowed");
                if (!quiet) Messages.ShowError("That Boost action is not allowed, as it results in this ship overlapping an obstacle");
                result.Add(ActionFailReason.ObstacleHit);
            }
            else if (obstaclesStayDetectorBase.OffTheBoardNow || obstaclesStayDetectorMovementTemplate.OffTheBoardNow)
            {
                if (!quiet) Messages.ShowError("That Boost action is not allowed, as it results in this ship leaving the battlefield");
                result.Add(ActionFailReason.OffTheBoard);
            }

            return result;
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
    }

    public class BoostExecutionSubPhase : GenericSubPhase
    {
        public string SelectedBoostHelper;
        public bool IsTractorBeamBoost;
        public ShipPositionInfo FinalPositionInfo;

        private GenericMovement BoostMovement;

        public override void Start()
        {
            Name = "Boost execution";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBoostExecution();
        }

        private void StartBoostExecution()
        {
            Rules.Collision.ClearBumps(TheShip);

            BoostMovement = SelectedBoostHelper switch
            {
                "Straight 1" => new StraightBoost(1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.None),
                "Bank 1 Left" => new BankBoost(1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.None),
                "Bank 1 Right" => new BankBoost(1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.None),
                "Turn 1 Right" => new TurnBoost(1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.None),
                "Turn 1 Left" => new TurnBoost(1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.None),
                "Straight 2" => new StraightBoost(2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.None),
                "Bank 2 Left" => new BankBoost(2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.None),
                "Bank 2 Right" => new BankBoost(2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.None),
                "Turn 2 Right" => new TurnBoost(2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.None),
                "Turn 2 Left" => new TurnBoost(2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.None),
                _ => new StraightBoost(1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.None),
            };

            BoostMovement.FinalPositionInfo = FinalPositionInfo;
            BoostMovement.TheShip = TheShip;

            MovementTemplates.ApplyMovementRuler(TheShip, BoostMovement);

            GameManagerScript.Instance.StartCoroutine(BoostExecutionCoroutine());
        }

        private IEnumerator BoostExecutionCoroutine()
        {
            yield return BoostMovement.Perform();
            if (!IsTractorBeamBoost) Sounds.PlayFly(TheShip);
        }

        public virtual void FinishBoost()
        {
            Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
        }

        public override void Next()
        {
            TheShip.CallPositionIsReadyToFinish(FinishBoostAnimation);
        }

        protected virtual void FinishBoostAnimation()
        {
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            UpdateHelpInfo();

            CallBack();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }
    }
}
