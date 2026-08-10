using BoardTools;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.EWing
    {
        public class GavinDarklighter : EWing
        {
            public GavinDarklighter() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Gavin Darklighter",
                    "Bold Wingman",
                    Faction.Rebel,
                    4,
                    5,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.GavinDarklighterAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Sensor,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech,
                        UpgradeType.Modification
                    },
                    seImageNumber: 51,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }

        public class GavinDarklighterXWA : GavinDarklighter
        {
            public GavinDarklighterXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 15;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 20;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                    UpgradeType.Sensor,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Torpedo
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //While a friendly ship performs an attack, if the defender is in your arc, the attacker may change 1 hit result to a critical hit result.
    public class GavinDarklighterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsAvailable,
                () => 20,
                DiceModificationType.Change,
                1,
                new List<DieSide> { DieSide.Success },
                DieSide.Crit,
                isGlobal: true
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker.Owner == HostShip.Owner
                && new ShotInfo(HostShip, Combat.Defender, HostShip.PrimaryWeapons).InPrimaryArc;
        }
    }
}
