using Abilities.SecondEdition;
using ActionsList;
using System;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class AdvancedWarningSystems : GenericUpgrade
    {
        public AdvancedWarningSystems() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                name: "Advanced Warning Systems",
                type: UpgradeType.Modification,
                cost: 0,
                abilityType: typeof(AdvancedWarningSystemsAbility)
            );

            IsHidden = true;

            ImageUrl = "https://infinitearenas.com/xw2xwa/images/pilots/midnight-evacuationofdqar.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    // After you are declared the defender of an attack, if a friendly ship at range 0-2 has a lock on the attacker, you may perform a red evade action.

    public class AdvancedWarningSystemsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsDefender += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsDefender -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            if(IsAvailable())
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskPerformEvadeAction);
            }
        }

        private void AskPerformEvadeAction(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            HostShip.AskPerformFreeAction(
                new EvadeAction() { Color = Actions.ActionColor.Red },
                delegate
                {
                    Selection.ChangeActiveShip(Combat.Attacker);
                    Triggers.FinishTrigger();
                },
                HostUpgrade.UpgradeInfo.Name,
                "You may perform a red Evade action.",
                HostUpgrade
            );
        }

        private bool IsAvailable()
        {
            return Combat.Defender == HostShip
                && !HostShip.IsStressed
                && HostShip.ActionBar.HasAction(typeof(EvadeAction))
                && Roster.AllShips.Values.Any(s => Tools.IsFriendly(HostShip, s) && HostShip.GetRangeToShip(s) <= 2 && s.GetTargetLockLetterPairsOn(Combat.Attacker).Any());
        }
    }
}