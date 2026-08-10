using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.NantexClassStarfighter
{
    public class SunFac : NantexClassStarfighter
    {
        public SunFac() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Sun Fac",
                "Archduke’s Enforcer",
                Faction.Separatists,
                6,
                5,
                15,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.SunFacAbility),
                abilityText: "while you perform a primary attack, if the defender is tractored, roll 1 additional attack die.",
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Modification
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class SunFacXWA : SunFac
    {
        public SunFacXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 13;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 17;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Talent,
                UpgradeType.Talent,
                UpgradeType.Modification,
                UpgradeType.Modification,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SunFacAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= CheckAbility;
        }

        protected virtual void CheckAbility(ref int count)
        {
            if (Combat.Defender.IsTractored)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": The defender is tractored, you roll an additional attack die");
                count++;
            }
        }
    }
}