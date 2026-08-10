using Bombs;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Mg100StarFortress
{
    public class Cat : Mg100StarFortress
    {
        public Cat() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Cat",
                "Cobalt Wasp",
                Faction.Resistance,
                1,
                5,
                17,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.CatAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Crew,
                    UpgradeType.Sensor,
                    UpgradeType.Gunner,
                    UpgradeType.Gunner,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Device,
                    UpgradeType.Device
                },
                legality: new List<Legality>() { Legality.ExtendedLegal }
            );

            ModelInfo.SkinName = "Cobalt";
        }
    }

    public class CatXWA : Cat
    {
        public CatXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 15;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 22;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Crew,
                UpgradeType.Sensor,
                UpgradeType.Gunner,
                UpgradeType.Gunner,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Device,
                UpgradeType.Device
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you perform a primary attack, if the defender is at range 0-1 of at least 1 friendly device, roll 1 additional die.
    public class CatAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckCatAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckCatAbility;
        }

        private void CheckCatAbility(ref int count)
        {
            if (Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
            {
                foreach (KeyValuePair<GenericDeviceGameObject, GenericBomb> bombHolder in BombsManager.GetBombsOnBoard())
                {
                    if (bombHolder.Value.HostShip.Owner.PlayerNo == HostShip.Owner.PlayerNo
                        && BombsManager.IsShipInRange(Combat.Defender, bombHolder.Key, 1))
                    {
                        Messages.ShowInfo(HostShip.PilotInfo.PilotName + " gains +1 attack die");
                        count++;
                        return;
                    }
                }
            }
        }
    }
}