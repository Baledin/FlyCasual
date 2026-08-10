using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class KoshkaFrost : FiresprayClassPatrolCraft
        {
            public KoshkaFrost() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Koshka Frost",
                    "Icy Professional",
                    Faction.Scum,
                    3,
                    7,
                    16,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KoshkaFrostAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Device,
                        UpgradeType.Cannon,
                        UpgradeType.Missile,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter
                    },
                    seImageNumber: 152,
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class KoshkaFrostXWA : KoshkaFrost
        {
            public KoshkaFrostXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 16;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 18;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
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
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KoshkaFrostAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsAvailable,
                GetPriority,
                DiceModificationType.Reroll,
                1
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            return ((Combat.AttackStep == CombatStep.Attack && Combat.Defender.IsStressed) ||
               (Combat.AttackStep == CombatStep.Defence && Combat.Attacker.IsStressed));
        }

        private int GetPriority()
        {
            return 90;
        }

    }
}