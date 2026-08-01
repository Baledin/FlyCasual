using BoardTools;
using Bombs;
using Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace ActionsList
{
    public class BombDropAction : GenericAction
    {
        public BombDropAction()
        {
            Name = DiceModificationName = "Drop Bomb";
        }

        public override void ActionTake()
        {
            Phases.CurrentSubPhase.Pause();

            BombsManager.CurrentDevice = Source as GenericBomb;

            Phases.StartTemporarySubPhaseOld(
                "Bomb drop planning",
                typeof(SubPhases.BombDropPlanningSubPhase),
                Phases.CurrentSubPhase.CallBack
            );
        }

        public override bool IsActionAvailable()
        {
            return !Selection.ThisShip.IsBombAlreadyDropped;
        }
    }
}

namespace SubPhases
{
    public class BombDropPlanningSubPhase : GenericSubPhase
    {
        public ManeuverTemplate SelectedBombDropHelper;
        private readonly List<ManeuverTemplate> AvailableBombDropTemplates = new();
        private readonly List<GenericDeviceGameObject> BombObjects = new();

        public bool UseFrontGuides = false;

        public override void Start()
        {
            Name = "Bomb drop planning";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBombDropPlanning();
        }

        public void StartBombDropPlanning()
        {
            GenerateAllowedBombDropTemplates();

            bool canUseFrontGuides = IsAllowedToUseFrontGuides();

            if (AvailableBombDropTemplates.Count == 1)
            {
                if (BombsManager.CurrentDevice is GenericBomb)
                {
                    ShowBombAndDropTemplate(AvailableBombDropTemplates.First());
                    WaitAndSelectBombPosition();
                }
                else if (BombsManager.CurrentDevice.UpgradeInfo.SubType == UpgradeSubType.Remote)
                {
                    if (canUseFrontGuides)
                    {
                        AskSelectOrientation(delegate
                        {
                            ShowRemoteAndDropTemplate(AvailableBombDropTemplates.First());
                            WaitAndSelectBombPosition();
                        });
                    }
                    else
                    {
                        ShowRemoteAndDropTemplate(AvailableBombDropTemplates.First());
                        WaitAndSelectBombPosition();
                    }
                }
            }
            else
            {
                if (canUseFrontGuides)
                {
                    AskSelectOrientation(delegate { AskSelectTemplate(WaitAndSelectBombPosition); });
                }
                else
                {
                    AskSelectTemplate(WaitAndSelectBombPosition);
                }
            }
        }

        private void AskSelectTemplate(Action callback)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Select template to drop the bomb",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                EventHandler = StartSelectTemplateDecision
            });

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, callback);
        }

        private void AskSelectOrientation(Action callback)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Select orientation",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                EventHandler = StartSelectOrientationDecision
            });

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, callback);
        }

        private void StartSelectTemplateDecision(object sender, EventArgs e)
        {
            SelectBombDropTemplateDecisionSubPhase subphase = (SelectBombDropTemplateDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Select template to drop the bomb",
                typeof(SelectBombDropTemplateDecisionSubPhase),
                Triggers.FinishTrigger
            );

            subphase.ShowSkipButton = false;

            foreach (ManeuverTemplate bombDropTemplate in AvailableBombDropTemplates)
            {
                subphase.AddDecision(
                    bombDropTemplate.Name,
                    delegate { SelectTemplate(bombDropTemplate); },
                    isCentered: (bombDropTemplate.Direction == Movement.ManeuverDirection.Forward)
                );
            }

            subphase.DescriptionShort = "Select template to drop the device";

            subphase.DefaultDecisionName = "Straight 1";

            subphase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            subphase.Start();
        }

        private void SelectTemplate(ManeuverTemplate selectedTemplate)
        {
            BombsManager.LastManeuverTemplateUsed = selectedTemplate;

            if (BombsManager.CurrentDevice is GenericBomb)
            {
                ShowBombAndDropTemplate(selectedTemplate);
            }
            else if (BombsManager.CurrentDevice.UpgradeInfo.SubType == UpgradeSubType.Remote)
            {
                ShowRemoteAndDropTemplate(selectedTemplate);
            }

            DecisionSubPhase.ConfirmDecision();
        }

        private void StartSelectOrientationDecision(object sender, EventArgs e)
        {
            SelectBombDropOrientationDecisionSubPhase subphase = (SelectBombDropOrientationDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Place using Front or Rear guides?",
                typeof(SelectBombDropOrientationDecisionSubPhase),
                Triggers.FinishTrigger
            );

            subphase.AddDecision("Front", delegate { SelectOrientation(true); });
            subphase.AddDecision("Rear", delegate { SelectOrientation(false); });

            subphase.ShowSkipButton = false;

            subphase.DescriptionShort = "Place using Front or Rear guides?";

            subphase.DefaultDecisionName = "Front";

            subphase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            subphase.Start();
        }

        private void SelectOrientation(bool useFrontGuides)
        {
            UseFrontGuides = useFrontGuides;

            DecisionSubPhase.ConfirmDecision();
        }

        private void ShowRemoteAndDropTemplate(ManeuverTemplate bombDropTemplate)
        {
            Direction direction = Direction.Bottom;
            Selection.ThisShip.CallOnGetBombTemplateDirection(ref direction);

            Vector3 position = direction switch
            {
                Direction.Left => Selection.ThisShip.GetLeft(),
                Direction.Right => Selection.ThisShip.GetRight(),
                _ => Selection.ThisShip.GetBack(),
            };

            bombDropTemplate.ApplyTemplate(Selection.ThisShip, position, direction);

            Vector3 bombPosition = bombDropTemplate.GetFinalPosition();
            Quaternion bombRotation = bombDropTemplate.GetFinalRotation();

            GenericRemote remote = ShipFactory.SpawnRemote(
                (GenericRemote)Activator.CreateInstance(BombsManager.CurrentDevice.UpgradeInfo.RemoteType, Selection.ThisShip.Owner),
                bombPosition,
                bombRotation
            );

            if (UseFrontGuides)
            {
                remote.SetAngles(remote.GetAngles() + new Vector3(0, 180, 0));
                remote.SetPosition(remote.GetPosition() + (remote.GetJointPosition(1) - remote.GetJointPosition(2)));
            }

            SelectedBombDropHelper = bombDropTemplate;
        }

        private class SelectBombDropTemplateDecisionSubPhase : DecisionSubPhase { }

        private class SelectBombDropOrientationDecisionSubPhase : DecisionSubPhase { }

        private void CreateBombObject(Vector3 bombPosition, Quaternion bombRotation)
        {
            GenericBomb bomb = BombsManager.CurrentDevice as GenericBomb;

            GenericDeviceGameObject prefab = Resources.Load<GenericDeviceGameObject>(bomb.bombPrefabPath);
            GenericDeviceGameObject device = MonoBehaviour.Instantiate(prefab, bombPosition, bombRotation, Board.GetBoard());
            device.Initialize(bomb);
            BombObjects.Add(device);

            if (!string.IsNullOrEmpty(bomb.bombSidePrefabPath))
            {
                GenericDeviceGameObject prefabSide = Resources.Load<GenericDeviceGameObject>(bomb.bombSidePrefabPath);
                GenericDeviceGameObject extraPiece1 = MonoBehaviour.Instantiate(prefabSide, bombPosition, bombRotation, Board.GetBoard());
                GenericDeviceGameObject extraPiece2 = MonoBehaviour.Instantiate(prefabSide, bombPosition, bombRotation, Board.GetBoard());
                BombObjects.Add(extraPiece1);
                BombObjects.Add(extraPiece2);
                extraPiece1.Initialize(bomb);
                extraPiece2.Initialize(bomb);
            }
        }

        private void GenerateAllowedBombDropTemplates()
        {
            List<ManeuverTemplate> allowedTemplates = Selection.ThisShip.GetAvailableBombDropTemplates(BombsManager.CurrentDevice);

            foreach (ManeuverTemplate bombDropTemplate in allowedTemplates)
            {
                AvailableBombDropTemplates.Add(bombDropTemplate);
            }
        }

        private bool IsAllowedToUseFrontGuides()
        {
            return Selection.ThisShip.AllowBombDropFrontGuides(BombsManager.CurrentDevice);
        }

        private void ShowBombAndDropTemplate(ManeuverTemplate bombDropTemplate)
        {
            GenericBomb bomb = BombsManager.CurrentDevice as GenericBomb;

            Direction direction = Direction.Bottom;
            Selection.ThisShip.CallOnGetBombTemplateDirection(ref direction);

            Vector3 position = direction switch
            {
                Direction.Left => Selection.ThisShip.GetLeft(),
                Direction.Right => Selection.ThisShip.GetRight(),
                _ => Selection.ThisShip.GetBack(),
            };

            bombDropTemplate.ApplyTemplate(Selection.ThisShip, position, direction);

            Vector3 bombPosition = bombDropTemplate.GetFinalPosition();
            Quaternion bombRotation = bombDropTemplate.GetFinalRotation();
            CreateBombObject(bombPosition, bombRotation);

            for (int i = 0; i < BombObjects.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        BombObjects[i].transform.position = bombPosition;
                        break;
                    case 1:
                        BombObjects[i].transform.position = bombPosition
                            + BombObjects.First().transform.TransformVector(new Vector3(
                                bomb.bombSideDistanceX,
                                0,
                                bomb.bombSideDistanceZ
                            )
                        );
                        break;
                    case 2:
                        BombObjects[i].transform.position = bombPosition
                            + BombObjects.First().transform.TransformVector(new Vector3(
                                -bomb.bombSideDistanceX,
                                0,
                                bomb.bombSideDistanceZ
                            )
                        );
                        break;
                    default:
                        break;
                }

                BombObjects[i].transform.rotation = bombRotation;
            }

            SelectedBombDropHelper = bombDropTemplate;
        }

        private void WaitAndSelectBombPosition()
        {
            GameManagerScript.Wait(1f, SelectBombPosition);
        }

        private void SelectBombPosition()
        {
            HidePlanningTemplates();
            DeviceDropExecute();
        }

        private void DeviceDropExecute()
        {
            if (BombsManager.CurrentDevice is GenericBomb)
            {
                (BombsManager.CurrentDevice as GenericBomb).ActivateBombs(BombObjects, FinishAction);
            }
            else if (BombsManager.CurrentDevice.UpgradeInfo.SubType == UpgradeSubType.Remote)
            {
                // TODO: Activate remote
                FinishAction();
            }
        }

        private void FinishAction()
        {
            Phases.FinishSubPhase(typeof(BombDropPlanningSubPhase));
            CallBack();
        }

        private void HidePlanningTemplates()
        {
            SelectedBombDropHelper.DestroyTemplate();
            Roster.SetRaycastTargets(true);
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
        {
            return false;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip, int mouseKeyIsPressed)
        {
            return false;
        }
    }
}