using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship.CardInfo;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace SecondEdition.Mg100StarFortress
    {
        public class Mg100StarFortress : GenericShip
        {
            public Mg100StarFortress() : base()
            {
                ShipInfo = new ShipCardInfo25
                (
                    "MG-100 StarFortress",
                    BaseSize.Large,
                    new FactionData
                    (
                        new Dictionary<Faction, System.Type>
                        {
                            { Faction.Resistance, typeof(CobaltSquadronBomber) }
                        }
                    ),
                    new ShipArcsInfo
                    (
                        new ShipArcInfo(ArcType.Front, 3),
                        new ShipArcInfo(ArcType.DoubleTurret, 2)
                    ),
                    1, 9, 3,
                    new ShipActionsInfo
                    (
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(TargetLockAction)),
                        new ActionInfo(typeof(RotateArcAction)),
                        new ActionInfo(typeof(ReloadAction))
                    ),
                    new ShipUpgradesInfo(),
                    legality: new List<Content.Legality>() { Content.Legality.ExtendedLegal, Content.Legality.XWA }
                );

                ModelInfo = new ShipModelInfo
                (
                    "B/SF-17 Bomber",
                    "Crimson",
                    new Vector3(-4.25f, 8.3f, 5.55f),
                    3f
                );

                DialInfo = new ShipDialInfo
                (
                    new ManeuverInfo(ManeuverSpeed.Speed0, ManeuverDirection.Stationary, ManeuverBearing.Stationary, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Complex)
                );

                SoundInfo = new ShipSoundInfo
                (
                    new List<string>()
                    {
                        "Falcon-Fly1",
                        "Falcon-Fly2",
                        "Falcon-Fly3"
                    },
                    "Falcon-Fire", 2
                );

                ShipIconLetter = 'Z';
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    // When you drop a device, you may set the template with its middle line aligned with the hashmark on the base in your left or right side.
    public class ModularBombingMagazine : GenericAbility
    {
        Direction selectedDirection = Direction.Bottom;

        public override void ActivateAbility()
        {
            HostShip.BeforeBombWillBeDropped += RegisterDeviceDropAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeBombWillBeDropped -= RegisterDeviceDropAbility;
        }

        private void RegisterDeviceDropAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.BeforeBombWillBeDropped, AskToUseDeviceDropAbility);
        }
        private void AskToUseDeviceDropAbility(object sender, EventArgs e)
        {
            AskForDecision(
                descriptionShort: "Modular Bombing Magazine",
                descriptionLong: "Drop device using left or right side instead of rear guides?",
                imageHolder: HostShip,
                decisions: new() {
                    { "Left", UseDeviceAbilityLeft },
                    { "Right", UseDeviceAbilityRight },
                    { "No", SkipUseAbility }
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

        private void SkipUseAbility(object sender, EventArgs e)
        {
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

        private void GetDeviceDirection(ref Direction direction)
        {
            HostShip.OnGetBombTemplateDirection -= GetDeviceDirection;
            direction = selectedDirection;
            selectedDirection = Direction.Bottom;
        }
    }
}