using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship;
using Ship.CardInfo;
using System;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace Ship.SecondEdition.NabooRoyalN1Starfighter
{
    public class NabooRoyalN1Starfighter : GenericShip
    {
        public NabooRoyalN1Starfighter() : base()
        {
            ShipInfo = new ShipCardInfo25
            (
                "Naboo Royal N-1 Starfighter",
                BaseSize.Small,
                new FactionData
                (
                    new Dictionary<Faction, Type>
                    {
                        { Faction.Republic, typeof(RicOlie) },
                        { Faction.Scum, typeof(GavynSykes) }
                    }
                ),
                new ShipArcsInfo(ArcType.Front, 2), 2, 3, 2,
                new ShipActionsInfo
                (
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(TargetLockAction)),
                    new ActionInfo(typeof(BarrelRollAction)),
                    new ActionInfo(typeof(BoostAction))
                ),
                new ShipUpgradesInfo(),
                abilityText: "<b>Full Throttle:</b> After you fully execute a speed 3-5 maneuver, you may perform an evade action."
            );

            ModelInfo = new ShipModelInfo
            (
                "Naboo Royal N-1 Starfighter",
                "Yellow",
                new Vector3(-3.75f, 8f, 5.55f),
                1.1f
            );

            DialInfo = new ShipDialInfo
            (
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.TallonRoll, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.TallonRoll, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal)
            );

            SoundInfo = new ShipSoundInfo
            (
                new List<string>()
                {
                    "XWing-Fly1",
                    "XWing-Fly2",
                    "XWing-Fly3"
                },
                "XWing-Laser", 2
            );

            ShipAbilities.Add(new Abilities.SecondEdition.FullThrottleAbility());

            ShipIconLetter = '<';
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RestoredSpeedsterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnWeaponsDisabledCheck += AllowBullseyeAttacksWhileDisarmed;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnWeaponsDisabledCheck -= AllowBullseyeAttacksWhileDisarmed;
        }

        private void AllowBullseyeAttacksWhileDisarmed(ref bool isDisarmed)
        {
            if (HostShip.Tokens.GetTokens<WeaponsDisabledToken>().Count == 1
                && Combat.ChosenWeapon is PrimaryWeaponClass
                && HostShip.SectorsInfo.IsShipInSector(Selection.AnotherShip, Arcs.ArcType.Bullseye))
            {
                Messages.ShowInfo($"Restored Speedster: Primary weapon attacks in the bullseye arc are allowed while disarmed.");
                isDisarmed = false;
            }
        }
    }
}