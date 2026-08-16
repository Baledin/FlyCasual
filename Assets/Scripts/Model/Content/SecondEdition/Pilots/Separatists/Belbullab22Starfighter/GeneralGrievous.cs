using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Belbullab22Starfighter
{
    public class GeneralGrievous : Belbullab22Starfighter
    {
        public GeneralGrievous()
        {
            PilotInfo = new PilotCardInfo25
            (
                "General Grievous",
                "Ambitious Cyborg",
                Faction.Separatists,
                4,
                5,
                14,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.GeneralGrievousAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Missile,
                    UpgradeType.Title,
                    UpgradeType.Modification,
                    UpgradeType.Modification
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class GeneralGrievousXWA : GeneralGrievous
    {
        public GeneralGrievousXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 14;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 22;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Modification,
                UpgradeType.Modification,
                UpgradeType.Title
            };
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you perform a primary attack, if you are not in the defender’s firing arc, you may reroll up to 2 attack dice.
    public class GeneralGrievousAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Reroll,
                2
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsDiceModificationAvailable()
        {
            return (Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker == HostShip
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                && !new ShotInfo(Combat.Defender, HostShip, Combat.Defender.PrimaryWeapons.First()).InArc);
        }

        public int GetDiceModificationAiPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.FocusesNotRerolled;
                int attackBlanks = Combat.DiceRollAttack.BlanksNotRerolled;

                if (Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
                {
                    if (attackBlanks > 0) result = 90;
                }
                else
                {
                    if (attackBlanks + attackFocuses > 0) result = 90;
                }
            }

            return result;
        }
    }
}
