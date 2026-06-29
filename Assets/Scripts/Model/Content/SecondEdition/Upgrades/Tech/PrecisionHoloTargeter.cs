using Ship;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class PrecisionHoloTargeter : GenericUpgrade
    {
        public PrecisionHoloTargeter() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Precision Holo-Targeter",
                UpgradeType.Tech,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.PrecisionHoloTargeterAbility),
                legalityInfo: new () { Content.Legality.XWA }
            );

            IsHidden = true;
            ImageUrl = "https://infinitearenas.com/xw2xwa/images/pilots/zizitlo-evacuationofdqar.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    // While you perform a primary attack, if you are not in the defender's firing arc, the defender rolls 1 fewer defense die.
    public class PrecisionHoloTargeterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += TryAddPrecisionHoloTargeterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= TryAddPrecisionHoloTargeterAbility;
        }

        public void TryAddPrecisionHoloTargeterAbility()
        {
            if (CanAbilityBeUsed())
            {
                Combat.Defender.AfterGotNumberOfDefenceDice += ReduceDefenseDice;
            }
        }

        private bool CanAbilityBeUsed()
        {
            if (Combat.ChosenWeapon.WeaponType != WeaponTypes.PrimaryWeapon || Combat.ShotInfo.Range < 1)
            {
                return false;
            }

            BoardTools.ShotInfo reverseShotInfo = new BoardTools.ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapons);
            return !reverseShotInfo.InArc;
        }

        private void ReduceDefenseDice(ref int count)
        {
            Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name}: The defender's defense dice have been decreased by 1.");
            Combat.Defender.AfterGotNumberOfDefenceDice -= ReduceDefenseDice;

            count--;
        }
    }
}
