using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class BobaFettSV : FiresprayClassPatrolCraft
        {
            public BobaFettSV() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Boba Fett",
                    "Notorious Bounty Hunter",
                    Faction.Scum,
                    5,
                    8,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BobaFettScumAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
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
                    seImageNumber: 149,
                    skinName: "Boba Fett",
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class BobaFettSVXWA : BobaFettSV
        {
            public BobaFettSVXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 20;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 20;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
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
    public class BobaFettScumAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationPriority,
                DiceModificationType.Reroll,
                GetNumberOfEnemyShipsAtRange1
            );
        }

        private bool IsDiceModificationAvailable()
        {
            bool result = false;
            if ((Combat.AttackStep == CombatStep.Attack) || (Combat.AttackStep == CombatStep.Defence))
            {
                // Boba Fett will now test to see if there are any results that need changed before using his ability on a die roll.
                if (GetNumberOfEnemyShipsAtRange1() > 0 && (Combat.CurrentDiceRoll.Blanks + Combat.CurrentDiceRoll.Focuses) > 0) result = true;
            }
            return result;
        }

        private int GetNumberOfEnemyShipsAtRange1()
        {
            return BoardTools.Board.GetShipsAtRange(HostShip, new UnityEngine.Vector2(0, 1), Team.Type.Enemy).Count;
        }

        private int GetDiceModificationPriority()
        {
            return 90;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}