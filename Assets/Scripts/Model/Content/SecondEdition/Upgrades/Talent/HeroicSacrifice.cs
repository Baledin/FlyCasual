using ActionsList;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class HeroicSacrifice : GenericUpgrade
    {
        public HeroicSacrifice() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "Heroic Sacrifice",
                UpgradeType.Talent,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.HeroicSacrificeAbility)
            );

            IsHidden = true;

            ImageUrl = "Pilots/arvelcrynyd-battleoverendor";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform a SLAM action, roll 5 attack dice. Each large ship, huge ship, and scenario feature at range 0 suffers 1 damage for each hit / crit result, bypassing shields. Then this ship is destroyed.

    //You can perform SLAM actions, even while stressed.
    public class HeroicSacrificeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.ActionBar.ActionsThatCanbePreformedwhileStressed.Add(typeof(SlamAction));
            HostShip.OnActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.ActionBar.ActionsThatCanbePreformedwhileStressed.Remove(typeof(SlamAction));
            HostShip.OnActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction action)
        {
            if (action is SlamAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, HeroicSacrificeDamage);
            }
        }

        private void HeroicSacrificeDamage(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);
            PerformDiceCheck(
                HostName,
                DiceKind.Attack,
                5,
                DiceCheckFinished,
                Triggers.FinishTrigger
            );
        }

        private void DiceCheckFinished()
        {
            int damage = DiceCheckRoll.Successes + DiceCheckRoll.CriticalSuccesses;

            List<GenericShip> bumpedLargeShips = HostShip.ShipsBumped.Where(s => s.ShipBase.Size == BaseSize.Large).ToList();

            HostShip.DestroyShipForced(delegate { DealDamageToShips(bumpedLargeShips, damage, false, AbilityDiceCheck.ConfirmCheck); });
        }
    }
}