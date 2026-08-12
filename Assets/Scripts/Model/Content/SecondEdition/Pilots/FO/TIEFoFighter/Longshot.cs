using BoardTools;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.TIEFoFighter
{
    public class Longshot : TIEFoFighter
    {
        public Longshot() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "\"Longshot\"",
                "Zeta Ace",
                Faction.FirstOrder,
                3,
                3,
                8,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.LongshotAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Tech,
                    UpgradeType.Tech,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Tie
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class LongshotXWA : Longshot
    {
        public LongshotXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 7;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 5;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Sensor,
                UpgradeType.Modification,
                UpgradeType.Tech
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LongshotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += LongshotPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= LongshotPilotAbility;
        }

        private void LongshotPilotAbility(ref int result)
        {
            ShotInfo shotInformation = new ShotInfo(Combat.Attacker, Combat.Defender, Combat.ChosenWeapon);
            if (shotInformation.Range == 3)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " is attacking at range 3 and gains +1 attack die");
                result++;
            }
        }
    }
}