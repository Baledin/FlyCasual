using Abilities;
using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class CloneCaptainRex : GenericUpgrade
    {
        public CloneCaptainRex() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Clone Captain Rex",
                UpgradeType.Gunner,
                cost: 2,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Republic),
                abilityType: typeof(Abilities.SecondEdition.CloneCaptainRexAbility),
                legalityInfo: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            Avatar = new AvatarInfo(
                Faction.Republic,
                new Vector2(278, 0),
                new Vector2(118, 118)
            );
        }
    }

    public class CloneCaptainRexXWA : CloneCaptainRex
    {
        public CloneCaptainRexXWA() : base()
        {
            UpgradeInfo.Cost = 1;
            UpgradeInfo.LegalityInfo = new List<Legality>
            {
                Legality.XWA
            };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CloneCaptainRexAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += CloneCaptainRexEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= CloneCaptainRexEffect;
        }

        private void CloneCaptainRexEffect(GenericShip host)
        {
            GenericAction newAction = new ActionsList.SecondEdition.CloneCaptainRexEffect()
            {
                HostShip = host,
                HostUpgrade = HostUpgrade,
                Ability = this,
                ImageUrl = HostUpgrade.ImageUrl
            };
            host.AddAvailableDiceModificationOwn(newAction);
        }
    }
}

namespace ActionsList.SecondEdition
{
    public class CloneCaptainRexEffect : GenericAction
    {
        public GenericUpgrade HostUpgrade { get; set; }
        public GenericAbility Ability { get; set; }

        public CloneCaptainRexEffect()
        {
            Name = DiceModificationName = "Clone Captain Rex";
        }

        public override void ActionEffect(Action callBack)
        {
            Combat.DiceRollAttack.RemoveType(DieSide.Focus);

            EachShipCanDoAction action = new 
            (
                EachShipAction,
                onFinish: delegate
                {
                    Selection.ChangeActiveShip(HostShip);
                    callBack();
                },
                conditions: new ConditionsBlock
                (
                    new TeamCondition(ShipTypes.Friendly),
                    new DefenderInSectorCondition(Arcs.ArcType.Bullseye)
                ),
                description: new Abilities.Parameters.AbilityDescription
                (
                    "Clone Captain Rex",
                    "Each friendly ship that has the defender in its Bullseye Arc may gain 1 Strain to perform a Focus action",
                    HostUpgrade
                )
            );

            action.DoAction(Ability);
        }

        private void EachShipAction(GenericShip ship, Action action)
        {
            Selection.ChangeActiveShip(ship);

            ship.Tokens.AssignToken(typeof(Tokens.StrainToken), delegate
            {
                ship.AskPerformFreeAction(new FocusAction(),
                    action,
                    descriptionShort: "Clone Captain Rex",
                    descriptionLong: "You may perform a Focus action",
                    imageHolder: HostUpgrade
                );
            });
        }

        public override bool IsDiceModificationAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.DiceRollAttack.HasResult(DieSide.Focus);
        }

        public override int GetDiceModificationPriority()
        {
            return 0;
        }
    }
}