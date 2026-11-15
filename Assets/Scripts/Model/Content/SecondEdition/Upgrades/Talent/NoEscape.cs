using BoardTools;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class NoEscape : GenericUpgrade
    {
        public NoEscape() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "No Escape",
                UpgradeType.Talent,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.NoEscapeAbility)
            );

            IsHidden = true;

            ImageUrl = "Pilots/captainyorr-battleoverendor";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NoEscapeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "No Escape",
                IsAvailable,
                AiPriority,
                DiceModificationType.Reroll,
                1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Blank }
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                && Combat.DiceRollAttack.Blanks > 0
                && HasMoreEnemyShipsInRange(Combat.Defender);
        }

        private int AiPriority()
        {
            return 95;
        }

        private bool HasMoreEnemyShipsInRange(GenericShip ship)
        {
            int friendlyShipsInRange = 0;
            int enemyShipsInRange = 0;

            foreach (GenericShip anotherShip in Roster.AllShips.Values)
            {
                DistanceInfo distInfo = new DistanceInfo(ship, anotherShip);
                if (distInfo.Range <= 1)
                {
                    if (Tools.IsAnotherFriendly(ship, anotherShip))
                    {
                        friendlyShipsInRange++;
                    }
                    else if (!Tools.IsFriendly(ship, anotherShip))
                    {
                        enemyShipsInRange++;
                    }
                }
            }

            return enemyShipsInRange > friendlyShipsInRange;
        }
    }
}