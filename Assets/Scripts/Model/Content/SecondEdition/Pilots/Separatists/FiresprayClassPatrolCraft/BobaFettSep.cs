using BoardTools;
using Content;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace Ship.SecondEdition.FiresprayClassPatrolCraft
{
    public class BobaFettSep : FiresprayClassPatrolCraft
    {
        public BobaFettSep() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Boba Fett",
                "Survivor",
                Faction.Separatists,
                3,
                7,
                16,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.BobaFettSeparatistAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Cannon,
                    UpgradeType.Missile,
                    UpgradeType.Crew,
                    UpgradeType.Device,
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

            PilotNameCanonical = "bobafett-firesprayclasspatrolcraft";
        }
    }

    public class BobaFettSepXWA : BobaFettSep
    {
        public BobaFettSepXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 16;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 9;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
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
    public class BobaFettSeparatistAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification
            (
                "Boba Fett",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Blank },
                sideCanBeChangedTo: DieSide.Focus
            );
        }

        private bool IsAvailable()
        {
            int friendlyShipsInRange = Board.GetShipsAtRange(HostShip, new Vector2(0, 2), Team.Type.Friendly).Count();

            return Combat.AttackStep == CombatStep.Defence
                && Combat.DiceRollDefence.HasResult(DieSide.Blank)
                && friendlyShipsInRange == 1;
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