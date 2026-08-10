using Arcs;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.TIESfFighter
{
    public class Backdraft : TIESfFighter
    {
        public Backdraft() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "\"Backdraft\"",
                "Fiery Fanatic",
                Faction.FirstOrder,
                4,
                4,
                5,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.BackdraftAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Tech,
                    UpgradeType.Missile,
                    UpgradeType.Gunner,
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

    public class BackdraftXWA : Backdraft
    {
        public BackdraftXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 11;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 15;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Sensor,
                UpgradeType.Gunner,
                UpgradeType.Modification,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Missile
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BackdraftAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckBackdraftAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckBackdraftAbility;
        }

        private void CheckBackdraftAbility(ref int count)
        {
            if (Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon && Combat.ChosenWeapon.WeaponInfo.ArcRestrictions.Contains(ArcType.SingleTurret))
            {
                if (HostShip.SectorsInfo.IsShipInSector(Combat.Defender, ArcType.Rear))
                {
                    Messages.ShowInfo(HostShip.PilotInfo.PilotName + " gains +1 attack die");
                    count++;
                }
            }
        }
    }
}
