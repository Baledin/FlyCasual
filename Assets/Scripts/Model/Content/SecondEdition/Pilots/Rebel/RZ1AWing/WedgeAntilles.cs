using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.RZ1AWing
{
    public class WedgeAntilles : RZ1AWing
    {
        public WedgeAntilles() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Wedge Antilles",
                "Promising Pilot",
                Faction.Rebel,
                4,
                3,
                5,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.WedgeAntillesAWingAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.AWing
                },
                abilityText: "While you perform a primary attack, if the defender is your front arc. The defender rolls 1 fewer defense die.",
                skinName: "Blue",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            PilotNameCanonical = "wedgeantilles-rz1awing";
        }
    }

    public class WedgeAntillesXWA : WedgeAntilles
    {
        public WedgeAntillesXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 9;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 6;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Missile,
                    UpgradeType.Configuration
                };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WedgeAntillesAWingAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += TryAddWedgeAntillesAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= TryAddWedgeAntillesAbility;
        }

        public void TryAddWedgeAntillesAbility()
        {
            if (Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon &&
                HostShip.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Front) &&
                Combat.ShotInfo.Range > 0)
            {
                Combat.Defender.AfterGotNumberOfDefenceDice += ReduceDefenseDice;
            }
        }

        private void ReduceDefenseDice(ref int count)
        {
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: The defender's defense dice have been decreased by 1.");
            Combat.Defender.AfterGotNumberOfDefenceDice -= ReduceDefenseDice;

            count--;
        }
    }
}