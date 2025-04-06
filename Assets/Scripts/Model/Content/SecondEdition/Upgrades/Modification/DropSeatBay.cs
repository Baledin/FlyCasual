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
                cost: 5,
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
        Direction selectedDirection = Direction.Bottom;
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplatesOneCondition += DropSeatBayTemplate;
            HostShip.BeforeBombWillBeDropped += RegisterDeviceDropAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplatesOneCondition -= DropSeatBayTemplate;
            HostShip.BeforeBombWillBeDropped -= RegisterDeviceDropAbility;
        }

        private void RegisterDeviceDropAbility()
        {
            if (BombsManager.CurrentDevice.UpgradeInfo.SubType == UpgradeSubType.Remote)
            {
                RegisterAbilityTrigger(TriggerTypes.BeforeBombWillBeDropped, AskToUseDeviceDropAbility);
            }
        }

        private void AskToUseDeviceDropAbility(object sender, EventArgs e)
        {
            AskForDecision(
                descriptionShort: "Drop Seat Bay",
                descriptionLong: "You may drop a remote using left or right side instead of rear guides?",
                imageHolder: HostUpgrade,
                decisions: new()
                {
                    { "Left", UseDeviceAbilityLeft },
                    { "Right", UseDeviceAbilityRight },
                    { "Rear", UseDeviceAbilityRear }
                },
                tooltips: new(),
                defaultDecision: "No",
                callback: Triggers.FinishTrigger,
                showSkipButton: true
            );
        }
        private void UseDeviceAbility()
        {
            HostShip.OnGetBombTemplateDirection += GetDeviceDirection;
            Triggers.FinishTrigger();
        }

        private void UseDeviceAbilityLeft(object sender, EventArgs e)
        {
            selectedDirection = Direction.Left;
            UseDeviceAbility();
        }

        private void UseDeviceAbilityRight(object sender, EventArgs e)
        {
            selectedDirection = Direction.Right;
            UseDeviceAbility();
        }

        private void UseDeviceAbilityRear(object sender, EventArgs e)
        {
            selectedDirection = Direction.Bottom;
            UseDeviceAbility();
        }

        private void GetDeviceDirection(ref Direction direction)
        {
            HostShip.OnGetBombTemplateDirection -= GetDeviceDirection;
            direction = selectedDirection;
            selectedDirection = Direction.Bottom;
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
                        if (!availableTemplates.Any(t => t.Name == newTemplate.Name))
                        {
                            availableTemplates.Add(newTemplate);
                        }
                    }
                }
            }
        }
    }
}