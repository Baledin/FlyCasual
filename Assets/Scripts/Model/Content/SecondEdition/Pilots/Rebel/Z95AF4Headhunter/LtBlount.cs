using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship.SecondEdition.Z95AF4Headhunter
{
    public class LtBlount : Z95AF4Headhunter
    {
        public LtBlount() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Lieutenant Blount",
                "Team Player",
                Faction.Rebel,
                4,
                3,
                11,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.LtBlountAbiliity),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Modification
                },
                seImageNumber: 28,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class LtBlountXWA : LtBlount
    {
        public LtBlountXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 8;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 10;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Talent,
                UpgradeType.Modification,
                UpgradeType.Missile
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LtBlountAbiliity : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterLtBlountAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterLtBlountAbility;
        }

        private void RegisterLtBlountAbility()
        {
            if (Combat.Attacker == HostShip)
            {
                bool isFriendlyShips = false;

                foreach (GenericShip ship in Board.GetShipsAtRange(Combat.Defender, new Vector2(0, 1), Team.Type.Enemy))
                {
                    if (ship != HostShip)
                        isFriendlyShips = true;
                }

                if (isFriendlyShips)
                {
                    HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += LtBlountAddAttackDice;
                }
            }
        }

        private void LtBlountAddAttackDice(ref int value)
        {
            value++;
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= LtBlountAddAttackDice;
        }
    }
}
