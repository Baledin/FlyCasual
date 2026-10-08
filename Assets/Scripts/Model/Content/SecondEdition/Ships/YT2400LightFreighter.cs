using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship;
using Ship.CardInfo;
using System.Collections.Generic;
using UnityEngine;

namespace Ship.SecondEdition.YT2400LightFreighter
{
    public class YT2400LightFreighter : GenericShip
    {
        public YT2400LightFreighter() : base()
        {
            ShipInfo = new ShipCardInfo25
            (
                "YT-2400 Light Freighter",
                BaseSize.Large,
                new FactionData
                (
                    new Dictionary<Faction, System.Type>
                    {
                        { Faction.Rebel, typeof(DashRendar) }
                    }
                ),
                new ShipArcsInfo(ArcType.DoubleTurret, 3), 2, 6, 4,
                new ShipActionsInfo
                (
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(TargetLockAction)),
                    new ActionInfo(typeof(BarrelRollAction), ActionColor.Red),
                    new ActionInfo(typeof(RotateArcAction))
                ),
                new ShipUpgradesInfo()
            );

            ModelInfo = new ShipModelInfo
            (
                "YT-2400",
                "Red",
                new Vector3(-3.35f, 8.2f, 5.55f),
                3.25f
            );

            DialInfo = new ShipDialInfo
            (
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex)
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

            ShipIconLetter = 'o';

            ShipAbilities.Add(new Abilities.SecondEdition.SensorBlackout());
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SensorBlackout : GenericAbility
    {
        public override string Name { get { return "Sensor Blackout"; } }

        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckSensorBlackout;
            HostShip.AfterGotNumberOfDefenceDice += CheckSensorBlackoutDefense;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckSensorBlackout;
            HostShip.AfterGotNumberOfDefenceDice -= CheckSensorBlackoutDefense;
        }

        private void CheckSensorBlackout(ref int count)
        {
            if (Combat.ChosenWeapon.WeaponType == Ship.WeaponTypes.PrimaryWeapon && Combat.ShotInfo.Range < 2) count -= 2;
        }

        private void CheckSensorBlackoutDefense(ref int count)
        {
            if (Combat.ShotInfo.Range < 2) count -= 1;
        }
    }

    public class SensorBlindspot : GenericAbility
    {
        // While you perform a primary attack at range 0-1, do not apply the range 0-1 bonus and roll 1 fewer attack die.

        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += ReduceAttackDice;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= ReduceAttackDice;
        }

        private void ReduceAttackDice(ref int count)
        {
            if (Combat.ChosenWeapon is not PrimaryWeaponClass) return;

            switch (Combat.ShotInfo.Range)
            {
                case 1:
                    Messages.ShowInfoToHuman($"Sensor Blindspot: {HostShip.PilotInfo.PilotName} loses Range 1 bonus and -1 attack die.");
                    count -= 2; // Remove +1 for range, plus further -1
                    break;
                case 0:
                    Messages.ShowInfoToHuman($"Sensor Blindspot: {HostShip.PilotInfo.PilotName} gets -1 attack die.");
                    count -= 1;
                    break;
            }
        }
    }
}