using Actions;
using ActionsList;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class AaylaSecura : GenericUpgrade
    {
        public AaylaSecura() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Aayla Secura",
                UpgradeType.Crew,
                cost: 12,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Republic),
                addAction: new ActionInfo(typeof(FocusAction), ActionColor.White),
                addActionLink: new LinkedActionInfo(typeof(FocusAction), typeof(CoordinateAction), ActionColor.Purple),
                abilityType: typeof(Abilities.SecondEdition.AaylaSecuraAbility),
                addForce: 1
            );

            Avatar = new AvatarInfo(
                Faction.Republic,
                new Vector2(313, 3)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AaylaSecuraAbility : GenericAbility
    {
        //While an enemy ship in your Bullseye Arc performs an attack, if the defender is friendly and at range 0-2, 
        //the defender may change 1 blank result to a focus result.
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                1,
                new List<DieSide> { DieSide.Blank },
                DieSide.Focus,
                isGlobal: true
            );
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Defence
                && Combat.Defender.Owner == HostShip.Owner
                && Combat.CurrentDiceRoll.Blanks > 0
                && HostShip.SectorsInfo.IsShipInSector(Combat.Attacker, Arcs.ArcType.Bullseye)
                && HostShip.GetRangeToShip(Combat.Defender) < 3;
        }

        private int GetAiPriority()
        {
            return 100;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}