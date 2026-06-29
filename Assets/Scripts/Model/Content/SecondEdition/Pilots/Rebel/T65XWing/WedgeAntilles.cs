using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.T65XWing
{
    public class WedgeAntilles : T65XWing
    {
        public WedgeAntilles() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Wedge Antilles",
                "Red Two",
                Faction.Rebel,
                6,
                5,
                9,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.WedgeAntillesAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                    UpgradeType.Modification,
                    UpgradeType.Torpedo,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.XWing
                },
                seImageNumber: 1,
                skinName: "Wedge Antilles",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class WedgeAntillesXWA : WedgeAntilles
    {
        public WedgeAntillesXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 14;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 13;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Talent,
                UpgradeType.Astromech,
                UpgradeType.Modification,
                UpgradeType.Torpedo,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WedgeAntillesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += AddWedgeAntillesAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= AddWedgeAntillesAbility;
        }

        protected void AddWedgeAntillesAbility()
        {
            if (Combat.ShotInfo.Range < 1) return;

            Combat.Defender.AfterGotNumberOfDefenceDice += ReduceDefenseDice;
        }

        protected void ReduceDefenseDice(ref int count)
        {
            Combat.Defender.AfterGotNumberOfDefenceDice -= ReduceDefenseDice;

            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: The defender's defense dice have been decreased by 1.");

            count--;
        }
    }
}