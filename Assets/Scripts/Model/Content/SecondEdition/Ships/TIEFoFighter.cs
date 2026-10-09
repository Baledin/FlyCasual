using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship;
using Ship.CardInfo;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;

namespace Ship.SecondEdition.TIEFoFighter
{
    public class TIEFoFighter : GenericShip
    {
        public TIEFoFighter() : base()
        {
            ShipInfo = new ShipCardInfo25
            (
                "TIE/fo Fighter",
                BaseSize.Small,
                new FactionData
                (
                    new Dictionary<Faction, Type>
                    {
                        { Faction.FirstOrder, typeof(Midnight) }
                    }
                ),
                new ShipArcsInfo(ArcType.Front, 2), 3, 3, 1,
                new ShipActionsInfo
                (
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(TargetLockAction)),
                    new ActionInfo(typeof(EvadeAction)),
                    new ActionInfo(typeof(BarrelRollAction))
                )
            );

            ModelInfo = new ShipModelInfo
            (
                "TIE/FO Fighter",
                "First Order",
                previewScale: 2f
            );

            DialInfo = new ShipDialInfo
            (
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal)
            );

            SoundInfo = new ShipSoundInfo
            (
                new List<string>()
                {
                    "TIE-Fly1",
                    "TIE-Fly2",
                    "TIE-Fly3",
                    "TIE-Fly4",
                    "TIE-Fly5",
                    "TIE-Fly6",
                    "TIE-Fly7"
                },
                "TIE-Fire", 2
            );

            ShipIconLetter = 'O';
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Merciless : GenericAbility
    {
        // Merciless: While you perform an attack, you may choose another friendly ship at range 0-1 of the defender. If you do, that ship gains a strain token and you may reroll a blank result.

        public override void ActivateAbility()
        {
            AddDiceModification(
                name: "Merciless",
                isAvailable: IsAvailable,
                aiPriority: AiPriority,
                modificationType: DiceModificationType.Reroll,
                count: 1,
                sidesCanBeSelected: new() { DieSide.Blank },
                payAbilityCost: RegisterStrainAnotherFriendly
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private void RegisterStrainAnotherFriendly(Action<bool> callback)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAbilityDirect, StrainAnotherFriendly);

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, delegate { callback(true); });
        }

        private void StrainAnotherFriendly(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                selectTargetAction: AssignStrain,
                filterTargets: FilterTargets,
                getAiPriority: AiPriority,
                subphaseOwnerPlayerNo: Combat.Attacker.Owner.PlayerNo,
                name: "Merciless: Assign Strain",
                description: "Select target to assign strain",
                showSkipButton: false
            );
        }

        private void AssignStrain()
        {
            TargetShip.Tokens.AssignToken(new StrainToken(TargetShip), SelectShipSubPhase.FinishSelection);
        }

        private bool FilterTargets(GenericShip ship)
        {
            return Tools.IsAnotherFriendly(Selection.ThisShip, ship) && Combat.Defender.GetRangeToShip(ship) < 2;
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker == HostShip
                && Roster.AllShips.Values.Any(s => Tools.IsAnotherFriendly(HostShip, s) && Combat.Defender.GetRangeToShip(s) < 2);
        }

        private int AiPriority()
        {
            return 0;
        }

        private int AiPriority(GenericShip ship)
        {
            return 100;
        }
    }
}