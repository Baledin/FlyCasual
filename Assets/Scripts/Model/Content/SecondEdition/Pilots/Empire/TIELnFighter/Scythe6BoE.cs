using Actions;
using ActionsList;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.TIELnFighter
{
    public class Scythe6BoE : TIELnFighter
    {
        public Scythe6BoE() : base()
        {
            PilotInfo = new PilotCardInfo25(
                "Scythe 6",
                "Battle Over Endor",
                Faction.Imperial,
                2,
                3,
                loadoutValue: 0,
                isStandardLayout: true,
                isLimited: true,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Modification,
                },
                tags: new List<Tags>
                {
                    Tags.Tie
                },
                abilityType: typeof(Abilities.SecondEdition.Scythe6Ability),
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            ShipAbilities.Add(new Abilities.SecondEdition.FormedUpAbility());
            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(EvadeAction)));
            ShipInfo.Hull++;

            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.NoEscape));
            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.Predator));
            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.IonManeuveringJet));
            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.TargetingMatrix));

            PilotNameCanonical = "scythe6-battleoverendor";
        }
    }

    public class ScytheBoEXWA : Scythe6BoE
    {
        public ScytheBoEXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 9;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Scythe6Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckAbility;
        }

        private void CheckAbility(ref int value)
        {
            if (Combat.ShotInfo.Range >= 1 && Combat.ShotInfo.Range <= 2)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": The attack is at range 1-2, attacker gains +1 attack die");
                value++;
            }
        }
    }
}