using Abilities.SecondEdition;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class WedgeAntilles : GenericUpgrade
    {
        public WedgeAntilles() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Wedge Antilles",
                UpgradeType.Gunner,
                cost: 10,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                abilityType: typeof(WedgeAntillesGunnerAbility),
                legalityInfo: new List<Legality>() { Legality.XWA }
            );

            NameCanonical = "wedgeantilles-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    // While you perform a turret attack, if you are not in the defender's firing arc, the defender rolls 1 fewer defense die.

    public class WedgeAntillesGunnerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += AddWedgeAntillesGunnerAbility;
        }
        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= AddWedgeAntillesGunnerAbility;
        }
        public void AddWedgeAntillesGunnerAbility()
        {

            bool inArc = false;

            foreach (PrimaryWeaponClass weapon in Combat.Defender.PrimaryWeapons)
            {
                if (weapon.IsShotAvailable(HostShip))
                {
                    inArc = true;
                    break;
                }
            }

            if (Combat.ArcForShot.IsTurretArc && !inArc && Combat.ShotInfo.Range > 0)
            {
                Combat.Defender.AfterGotNumberOfDefenceDice += ReduceDefenseDice;
            }
        }

        public void ReduceDefenseDice(ref int count)
        {
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: The defender's defense dice have been decreased by 1.");
            Combat.Defender.AfterGotNumberOfDefenceDice -= ReduceDefenseDice;

            count--;
        }
    }
}
