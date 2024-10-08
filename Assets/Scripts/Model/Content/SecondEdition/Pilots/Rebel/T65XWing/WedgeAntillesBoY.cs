using Abilities.SecondEdition;
using BoardTools;
using Conditions;
using Ship;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class WedgeAntillesBoY : WedgeAntilles
        {
            public WedgeAntillesBoY() : base()
            {
                PilotInfo.PilotTitle = "Battle of Yavin";
                PilotInfo.AbilityType = typeof(WedgeAntillesBoYAbility);
                (PilotInfo as PilotCardInfo25).LoadoutValue = 0;
                (PilotInfo as PilotCardInfo25).IsStandardLayout = true;

                ShipAbilities.Add(new HopeAbility());

                MustHaveUpgrades.Add(typeof(AttackSpeed));
                MustHaveUpgrades.Add(typeof(Marksmanship));
                MustHaveUpgrades.Add(typeof(ProtonTorpedoes));
                MustHaveUpgrades.Add(typeof(R2A3BoY));

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/a/a4/Wedgeantilles-battleofyavin.png";

                PilotNameCanonical = "wedgeantilles-battleofyavin";
            }
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
            foreach (GenericShip anotherFriendlyShip in HostShip.Owner.Ships.Values)
            {
                if (anotherFriendlyShip.ShipId == HostShip.ShipId) continue;

                ShotInfo shotInfo = new ShotInfo(Combat.Defender, anotherFriendlyShip, Combat.Defender.PrimaryWeapons);
                DistanceInfo distanceInfo = new DistanceInfo(HostShip, Combat.Defender);
                if (shotInfo.InArc && distanceInfo.Range > 0 && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
                {
                    WedgeAntillesCondition condition = new WedgeAntillesCondition(Combat.Defender, HostShip);
                    Combat.Defender.Tokens.AssignCondition(condition);

                    return;
                }
            }
        }
    }
}