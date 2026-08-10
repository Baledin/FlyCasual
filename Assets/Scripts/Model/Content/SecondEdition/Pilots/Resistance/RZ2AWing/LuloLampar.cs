using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.RZ2AWing
{
    public class LuloLampar : RZ2AWing
    {
        public LuloLampar() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "L'ulo L'ampar",
                "Luminous Mentor",
                Faction.Resistance,
                5,
                4,
                10,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.LuloLamparAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Tech
                },
                tags: new List<Tags>
                {
                    Tags.AWing
                },
                skinName: "Red",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class LuloLamparXWA : LuloLampar
    {
        public LuloLamparXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 9;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 5;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Talent,
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
    public class LuloLamparAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += LuloLamparAbilityAtkPilotAbility;
            HostShip.AfterGotNumberOfDefenceDice += LuloLamparAbilityDefPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= LuloLamparAbilityAtkPilotAbility;
            HostShip.AfterGotNumberOfDefenceDice -= LuloLamparAbilityDefPilotAbility;
        }

        private void LuloLamparAbilityAtkPilotAbility(ref int result)
        {
            if (HostShip.IsStressed && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " is stressed and gains +1 attack die");
                result++;
            }
        }

        private void LuloLamparAbilityDefPilotAbility(ref int result)
        {
            if (HostShip.IsStressed)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " is stressed and gains -1 defense die");
                result--;
            }
        }
    }
}