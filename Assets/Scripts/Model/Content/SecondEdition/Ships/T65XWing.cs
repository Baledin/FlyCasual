using Abilities.SecondEdition;
using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship.CardInfo;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship.SecondEdition.T65XWing
{
    public class T65XWing : GenericShip, IMovableWings
    {
        public WingPosition CurrentWingPosition { get; set; }

        public T65XWing() : base()
        {
            ShipInfo = new ShipCardInfo25
            (
                "T-65 X-wing",
                BaseSize.Small,
                new FactionData
                (
                    new Dictionary<Faction, System.Type>
                    {
                        { Faction.Rebel, typeof(LukeSkywalker) }
                    }
                ),
                new ShipArcsInfo(ArcType.Front, 3), 2, 4, 2,
                new ShipActionsInfo
                (
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(TargetLockAction)),
                    new ActionInfo(typeof(BarrelRollAction))
                ),
                new ShipUpgradesInfo()
            );

            DefaultUpgrades.Add(typeof(UpgradesList.SecondEdition.ServomotorSFoilsClosed));

            ModelInfo = new ShipModelInfo(
                "X-wing",
                "Red",
                new Vector3(-3.62f, 7.62f, 5.55f),
                1.5f,
                wingsPositions: WingPosition.Closed
            );

            DialInfo = new ShipDialInfo
            (
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.TallonRoll, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.TallonRoll, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex)
            );

            SoundInfo = new ShipSoundInfo
            (
                new List<string>()
                {
                    "XWing-Fly1",
                    "XWing-Fly2",
                    "XWing-Fly3"
                },
                "XWing-Laser", 3
            );

            ShipIconLetter = 'x';
        }
    }

    public class T65XWingBoE : T65XWing
    {
        public T65XWingBoE() : base()
        {
            DefaultUpgrades.Remove(typeof(UpgradesList.SecondEdition.ServomotorSFoilsClosed));

            ShipAbilities.Add(new LockedSFoilsAbility());

            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(FocusAction), typeof(BoostAction)));
            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(FocusAction)));
            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BoostAction)));
        }
    }
}

namespace Abilities.SecondEdition
{
    // After you perform a boost action, gain a deplete token

    public class LockedSFoilsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
        }

        public void CheckConditions(GenericAction action)
        {
            if (action is BoostAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AssignDepleteToken);
            }
        }

        public void AssignDepleteToken(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(Tokens.DepleteToken), Triggers.FinishTrigger);
        }
    }
}