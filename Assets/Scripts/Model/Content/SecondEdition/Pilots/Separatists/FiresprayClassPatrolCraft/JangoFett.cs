using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.FiresprayClassPatrolCraft
{
    public class JangoFett : FiresprayClassPatrolCraft
    {
        public JangoFett() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Jango Fett",
                "Simple Man",
                Faction.Separatists,
                6,
                8,
                22,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.JangoFettAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Cannon,
                    UpgradeType.Missile,
                    UpgradeType.Crew,
                    UpgradeType.Device,
                    UpgradeType.Illicit,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Title
                },
                tags: new List<Tags>
                {
                    Tags.BountyHunter
                },
                skinName: "Jango Fett",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class JangoFettXWA : JangoFett
    {
        public JangoFettXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 19;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 20;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Crew,
                UpgradeType.Illicit,
                UpgradeType.Modification,
                UpgradeType.Device,
                UpgradeType.Cannon,
                UpgradeType.Missile,
                UpgradeType.Title
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JangoFettAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Jango Fett",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Focus },
                sideCanBeChangedTo: DieSide.Blank,
                timing: DiceModificationTimingType.Opposite
            );
        }

        private bool IsAvailable()
        {
            bool result = false;

            switch (Combat.AttackStep)
            {
                case CombatStep.Attack:
                    if (Combat.Defender.ShipId == HostShip.ShipId)
                    {
                        result = IsMyRevealedManeuverIsLess(Combat.Attacker);
                    }
                    break;
                case CombatStep.Defence:
                    if (Combat.Attacker.ShipId == HostShip.ShipId && Combat.ChosenWeapon.WeaponType == Ship.WeaponTypes.PrimaryWeapon)
                    {
                        result = IsMyRevealedManeuverIsLess(Combat.Defender);
                    }
                    break;
                default:
                    break;
            }

            return result;
        }

        private bool IsMyRevealedManeuverIsLess(GenericShip anotherShip)
        {
            bool result = false;

            if (HostShip.RevealedManeuver != null && anotherShip.RevealedManeuver != null)
            {
                if (HostShip.RevealedManeuver.ColorComplexity < anotherShip.RevealedManeuver.ColorComplexity) result = true;
            }

            return result;
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