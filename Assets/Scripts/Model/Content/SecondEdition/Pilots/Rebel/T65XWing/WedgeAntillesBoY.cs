using Abilities.SecondEdition;
using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.T65XWing
{
    public class WedgeAntillesBoY : T65XWing
    {
        public WedgeAntillesBoY() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Wedge Antilles",
                "Battle of Yavin",
                Faction.Rebel,
                5,
                5,
                0,
                isLimited: true,
                abilityType: typeof(WedgeAntillesBoYAbility),
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
                isStandardLayout: true,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            ShipAbilities.Add(new HopeAbility());

            MustHaveUpgrades.Add(typeof(AttackSpeed));
            MustHaveUpgrades.Add(typeof(Marksmanship));
            MustHaveUpgrades.Add(typeof(ProtonTorpedoes));
            MustHaveUpgrades.Add(typeof(R2A3BoY));

            PilotNameCanonical = "wedgeantilles-battleofyavin";
        }
    }

    public class WedgeAntillesBoYXWA : WedgeAntillesBoY
    {
        public WedgeAntillesBoYXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 13;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WedgeAntillesBoYAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += AddWedgeAntillesBoYAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= AddWedgeAntillesBoYAbility;
        }

        public void AddWedgeAntillesBoYAbility()
        {
            if (Combat.ChosenWeapon.WeaponType != WeaponTypes.PrimaryWeapon) return;

            if (Combat.ShotInfo.Range < 1) return;

            foreach (GenericShip anotherFriendlyShip in HostShip.Owner.Ships.Values)
            {
                if (anotherFriendlyShip.ShipId == HostShip.ShipId) continue;

                ShotInfo shotInfo = new ShotInfo(Combat.Defender, anotherFriendlyShip, Combat.Defender.PrimaryWeapons);

                if (shotInfo.InArc)
                {
                    Combat.Defender.AfterGotNumberOfDefenceDice += ReduceDefenseDice;

                    return;
                }
            }
        }

        protected void ReduceDefenseDice(ref int count)
        {
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: The defender's defense dice have been decreased by 1.");
            Combat.Defender.AfterGotNumberOfDefenceDice -= ReduceDefenseDice;

            count--;
        }
    }
}