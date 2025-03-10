using System;
using System.Collections.Generic;
using Bombs;
using Upgrade;
using BoardTools;
using Movement;
using Remote;
using System.Linq;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class DropSeatBay : GenericUpgrade
    {
        public DropSeatBay()
        {
            UpgradeInfo = new UpgradeCardInfo("Drop Seat Bay",
                UpgradeType.Modification,
                cost: 1,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.GauntletFighter.GauntletFighter)),
                addSlots: new List<UpgradeSlot>
                {
                    new(UpgradeType.Crew),
                    new(UpgradeType.Crew)
                },
                forbidSlot: UpgradeType.Device,
                abilityType: typeof(Abilities.SecondEdition.DropSeatBayAbility)
            );
        }

    }
}

namespace Abilities.SecondEdition
{
    // If you would drop a [crew] remote using a [straight] template, you may use a bank [left or right] template of the same speed instead
    // and can align that template's middle line with the hashmark on your ship's left or right side instead of your rear guides.
    public class DropSeatBayAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplatesOneCondition += DropSeatBayTemplate;
            HostShip.OnGetAvailableDeviceSideDropTemplates += InitialDropSeatBayTemplate;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplatesOneCondition -= DropSeatBayTemplate;
            HostShip.OnGetAvailableDeviceSideDropTemplates -= InitialDropSeatBayTemplate;
        }

        private void InitialDropSeatBayTemplate(List<ManeuverTemplate> availableTemplates, GenericUpgrade upgrade)
        {
            if (upgrade.UpgradeInfo.SubType != UpgradeSubType.Remote) return;

            availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed1));
        }

        private void DropSeatBayTemplate(List<ManeuverTemplate> availableTemplates, GenericUpgrade upgrade)
        {
            if (upgrade.UpgradeInfo.SubType != UpgradeSubType.Remote) return;

            List<ManeuverTemplate> templatesCopy = new List<ManeuverTemplate>(availableTemplates);

            foreach (ManeuverTemplate existingTemplate in templatesCopy)
            {
                if (existingTemplate.Bearing == ManeuverBearing.Straight && existingTemplate.Direction == ManeuverDirection.Forward)
                {
                    List<ManeuverTemplate> newTemplates = new List<ManeuverTemplate>()
                    {
                        new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, existingTemplate.Speed, isBombTemplate: true),
                        new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, existingTemplate.Speed, isBombTemplate: true),
                    };

                    foreach (ManeuverTemplate newTemplate in newTemplates)
                    {
                        if (availableTemplates.All(t => t.Name != newTemplate.Name))
                        {
                            availableTemplates.Add(newTemplate);
                        }
                    }
                }
            }
        }
    }
}

namespace SubPhases
{
    public class BombSideDropPlanningSubPhase : GenericSubPhase
    {
        private List<GenericDeviceGameObject> BombObjects = new List<GenericDeviceGameObject>();
        List<ManeuverTemplate> AvailableSideDropTemplates = new List<ManeuverTemplate>();
        public ManeuverTemplate SelectedBombLaunchHelper;
        public bool useFrontGuides { get; set; }
        public Direction dropDirection { get; set; }

        public override void Start()
        {
            Name = "Device side drop planning";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBombLaunchPlanning();
        }

        public void StartBombLaunchPlanning()
        {
            Roster.SetRaycastTargets(false);

            ShowBombLaunchHelper();
        }

        private void CreateBombObject(Vector3 bombPosition, Quaternion bombRotation)
        {
            GenericBomb bomb = BombsManager.CurrentDevice as GenericBomb;

            GenericDeviceGameObject prefab = Resources.Load<GenericDeviceGameObject>(bomb.bombPrefabPath);
            var device =
                MonoBehaviour.Instantiate<GenericDeviceGameObject>(prefab, bombPosition, bombRotation,
                    Board.GetBoard());
            device.Initialize(bomb);
            BombObjects.Add(device);

            if (!string.IsNullOrEmpty(bomb.bombSidePrefabPath))
            {
                GenericDeviceGameObject prefabSide = Resources.Load<GenericDeviceGameObject>(bomb.bombSidePrefabPath);
                var extraPiece1 = MonoBehaviour.Instantiate(prefabSide, bombPosition, bombRotation, Board.GetBoard());
                var extraPiece2 = MonoBehaviour.Instantiate(prefabSide, bombPosition, bombRotation, Board.GetBoard());
                BombObjects.Add(extraPiece1);
                BombObjects.Add(extraPiece2);
                extraPiece1.Initialize(bomb);
                extraPiece2.Initialize(bomb);
            }
        }

        private void ShowBombLaunchHelper()
        {
            GenerateAllowedDeviceSideDropDirections();

            if (AvailableSideDropTemplates.Count == 1)
            {
                if (BombsManager.CurrentDevice is GenericBomb)
                {
                    ShowBombAndLaunchTemplate(AvailableSideDropTemplates.First());
                }
                else if (BombsManager.CurrentDevice.UpgradeInfo.SubType == UpgradeSubType.Remote)
                {
                    ShowRemoteAndLaunchTemplate(AvailableSideDropTemplates.First());
                }

                WaitAndSelectBombPosition();
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
                Name = "Select template to launch the bomb",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                EventHandler = StartSelectTemplateDecision
            });

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, WaitAndSelectBombPosition);
        }

        private void StartSelectTemplateDecision(object sender, System.EventArgs e)
        {
            SelectBarrelRollTemplateDecisionSubPhase selectBarrelRollTemplateDecisionSubPhase =
                (SelectBarrelRollTemplateDecisionSubPhase) Phases.StartTemporarySubPhaseNew(
                    "Select template to launch the bomb",
                    typeof(SelectBarrelRollTemplateDecisionSubPhase),
                    Triggers.FinishTrigger
                );

            selectBarrelRollTemplateDecisionSubPhase.ShowSkipButton = false;

            foreach (var bombDropTemplate in AvailableSideDropTemplates)
            {
                selectBarrelRollTemplateDecisionSubPhase.AddDecision(
                    bombDropTemplate.Name,
                    delegate { SelectTemplate(bombDropTemplate); },
                    isCentered: (bombDropTemplate.Direction == Movement.ManeuverDirection.Forward)
                );
            }

            selectBarrelRollTemplateDecisionSubPhase.DescriptionShort = "Select template to launch the bomb";

            selectBarrelRollTemplateDecisionSubPhase.DefaultDecisionName =
                selectBarrelRollTemplateDecisionSubPhase.GetDecisions().First().Name;

            selectBarrelRollTemplateDecisionSubPhase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            selectBarrelRollTemplateDecisionSubPhase.Start();
        }

        private void SelectTemplate(ManeuverTemplate selectedTemplate)
        {
            if (BombsManager.CurrentDevice is GenericBomb)
            {
                ShowBombAndLaunchTemplate(selectedTemplate);
            }
            else if (BombsManager.CurrentDevice.UpgradeInfo.SubType == UpgradeSubType.Remote)
            {
                ShowRemoteAndLaunchTemplate(selectedTemplate);
            }

            DecisionSubPhase.ConfirmDecision();
        }

        private class SelectBarrelRollTemplateDecisionSubPhase : DecisionSubPhase
        {
        }

        private void GenerateAllowedDeviceSideDropDirections()
        {
            List<ManeuverTemplate> allowedTemplates =
                Selection.ThisShip.GetAvailableDeviceSideDropTemplates(BombsManager.CurrentDevice);

            foreach (ManeuverTemplate bombLaunchTemplate in allowedTemplates)
            {
                AvailableSideDropTemplates.Add(bombLaunchTemplate);
            }
        }

        private void ShowBombAndLaunchTemplate(ManeuverTemplate bombDropTemplate)
        {
            Vector3 dropPosition = dropDirection == Direction.Right
                ? Selection.ThisShip.GetRight()
                : Selection.ThisShip.GetLeft();
            bombDropTemplate.ApplyTemplate(Selection.ThisShip, dropPosition, dropDirection);

            Vector3 bombPosition = bombDropTemplate.GetFinalPosition();
            Quaternion bombRotation = bombDropTemplate.GetFinalRotation();
            CreateBombObject(bombPosition, bombRotation);
            BombObjects[0].transform.position = bombPosition;

            SelectedBombLaunchHelper = bombDropTemplate;
        }

        private void ShowRemoteAndLaunchTemplate(ManeuverTemplate bombDropTemplate)
        {
            Vector3 dropPosition = dropDirection == Direction.Right
                ? Selection.ThisShip.GetRight()
                : Selection.ThisShip.GetLeft();
            bombDropTemplate.ApplyTemplate(Selection.ThisShip, dropPosition, dropDirection);

            Vector3 bombPosition = bombDropTemplate.GetFinalPosition();
            Quaternion bombRotation = bombDropTemplate.GetFinalRotation();

            // TODO: get type of remote from upgrade
            GenericRemote remote = ShipFactory.SpawnRemote(
                (GenericRemote) Activator.CreateInstance(BombsManager.CurrentDevice.UpgradeInfo.RemoteType,
                    Selection.ThisShip.Owner),
                bombPosition,
                bombRotation
            );

            if (useFrontGuides)
            {
                remote.SetAngles(remote.GetAngles() + new Vector3(0, 180, 0));
                remote.SetPosition(remote.GetPosition() + (remote.GetJointPosition(1) - remote.GetJointPosition(2)));
            }

            SelectedBombLaunchHelper = bombDropTemplate;
        }

        private void WaitAndSelectBombPosition()
        {
            GameManagerScript.Wait(1f, SelectBombPosition);
        }

        private void SelectBombPosition()
        {
            HidePlanningTemplates();
            BombLaunchExecute();
        }

        private void BombLaunchExecute()
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
            Phases.FinishSubPhase(typeof(BombSideDropPlanningSubPhase));
            CallBack();
        }

        private void HidePlanningTemplates()
        {
            SelectedBombLaunchHelper.DestroyTemplate();
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