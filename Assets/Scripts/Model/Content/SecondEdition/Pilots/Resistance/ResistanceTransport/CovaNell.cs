using Abilities.SecondEdition;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.ResistanceTransport
{
    public class CovaNell : ResistanceTransport
    {
        public CovaNell() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Cova Nell",
                "Evacuation Escort",
                Faction.Resistance,
                4,
                5,
                20,
                isLimited: true,
                abilityText: "While you defend or perform a primary attack, if your revealed maneuver is red, roll 1 additional die.",
                abilityType: typeof(CovaNellAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                    UpgradeType.Crew,
                    UpgradeType.Crew,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Cannon,
                    UpgradeType.Cannon,
                    UpgradeType.Torpedo
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class CovaNellXWA : CovaNell
    {
        public CovaNellXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 11;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 13;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Astromech,
                UpgradeType.Crew,
                UpgradeType.Crew,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Cannon,
                UpgradeType.Cannon,
                UpgradeType.Torpedo
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CovaNellAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckAbility;
            HostShip.AfterGotNumberOfDefenceDice += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckAbility;
            HostShip.AfterGotNumberOfDefenceDice -= CheckAbility;
        }

        private void CheckAbility(ref int count)
        {
            if (HostShip.RevealedManeuver == null) return;

            if (HostShip.RevealedManeuver.ColorComplexity == Movement.MovementComplexity.Complex)
            {
                if (Combat.AttackStep == CombatStep.Defence)
                {
                    Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": +1 defense die");
                    count++;
                }
                else if (Combat.AttackStep == CombatStep.Attack && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
                {
                    Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": +1 attack die");
                    count++;
                }
            }
        }
    }
}
