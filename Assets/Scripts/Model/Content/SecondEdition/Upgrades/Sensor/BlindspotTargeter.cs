using Abilities.SecondEdition;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class BlindspotTargeter : GenericUpgrade
    {
        public BlindspotTargeter() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                name: "Blindspot Targeter",
                type: UpgradeType.Sensor,
                cost: 7,
                abilityType: typeof(BlindspotTargeterAbility),
                legalityInfo: new List<Legality>() { Legality.XWA }
            );

            NameCanonical = "blindspottargeter-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    // While you perform a primary attack, if you are not in the defender's firing arc, you may change 1 focus result to a hit result.

    public class BlindspotTargeterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                name: HostUpgrade.UpgradeInfo.Name,
                isAvailable: IsAvailable,
                aiPriority: GetAiPriority,
                modificationType: DiceModificationType.Change,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Focus },
                sideCanBeChangedTo: DieSide.Success
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            bool inArc = false;

            foreach (PrimaryWeaponClass weapon in Combat.Defender.PrimaryWeapons)
            {
                if (weapon.IsShotAvailable(HostShip))
                {
                    inArc = true;
                    break;
                }
            }

            return Combat.Attacker == HostShip &&
                Combat.ChosenWeapon is PrimaryWeaponClass &&
                Combat.DiceRollAttack.Focuses > 0 &&
                !inArc;
        }

        private int GetAiPriority()
        {
            return 100;
        }
    }
}