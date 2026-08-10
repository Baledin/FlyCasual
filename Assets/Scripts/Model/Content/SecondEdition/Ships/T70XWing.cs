using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship.CardInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class T70XWing : GenericShip, IMovableWings
        {
            public WingsPositions CurrentWingsPosition { get; set; }

            public T70XWing() : base()
            {
                ShipInfo = new ShipCardInfo25
                (
                    "T-70 X-wing",
                    BaseSize.Small,
                    new FactionData
                    (
                        new Dictionary<Faction, Type>
                        {
                            { Faction.Resistance, typeof(PoeDameron) }
                        }
                    ),
                    new ShipArcsInfo(ArcType.Front, 3), 2, 4, 3,
                    new ShipActionsInfo
                    (
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(TargetLockAction)),
                        new ActionInfo(typeof(BoostAction))
                    ),
                    new ShipUpgradesInfo()
                );

                DefaultUpgrades.Add(typeof(UpgradesList.SecondEdition.IntegratedSFoilsClosed));

                ShipAbilities.Add(new Abilities.SecondEdition.HardPointAbility());

                ModelInfo = new ShipModelInfo
                (
                    "T-70 X-wing",
                    "Blue",
                    new Vector3(-3.6f, 7.62f, 5.55f),
                    1.5f
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
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
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

                ShipIconLetter = 'w';
            }
        }

        public class T70XWingEoD : T70XWing
        {
            public T70XWingEoD() : base()
            {
                DefaultUpgrades.Remove(typeof(UpgradesList.SecondEdition.IntegratedSFoilsClosed));

                ShipAbilities.Remove(ShipAbilities.First(n => n.GetType() == typeof(Abilities.SecondEdition.HardPointAbility)));
                ShipAbilities.Add(new Abilities.SecondEdition.AdaptiveSFoils());

                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(FocusAction), typeof(BarrelRollAction)));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction)));
                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BoostAction), typeof(FocusAction)));
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    // After you perform a Barrel Roll action, gain a deplete token.
    // While you perform an attack, before rolling attack dice, if the defender is in your bullseye, you may remove 1 deplete token.
    public class AdaptiveSFoils : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckActionAbility;
            HostShip.OnAttackStartAsAttacker += AdaptiveSFoilsAttackAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckActionAbility;
            HostShip.OnAttackStartAsAttacker -= AdaptiveSFoilsAttackAbility;
        }

        private void CheckActionAbility(GenericAction action)
        {
            if (action is BarrelRollAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, GainDeplete);
            }
        }

        private void GainDeplete(object sender, EventArgs e)
        {
            HostShip.Tokens.AssignToken(new DepleteToken(HostShip),Triggers.FinishTrigger);
        }

        private void AdaptiveSFoilsAttackAbility()
        {
            if (HostShip.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Bullseye))
            {
                HostShip.Tokens.RemoveToken(typeof(DepleteToken),()=>{});
            }
        }
    }
}