using Actions;
using ActionsList;
using Ship;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class BlackOneEoD : GenericUpgrade
    {
        public BlackOneEoD() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Black One",
                UpgradeType.Title,
                cost: 0,
                isLimited: true,
                charges: 2,
                addAction: new ActionInfo(typeof(SlamAction)),
                abilityType: typeof(Abilities.SecondEdition.BlackOneEoDAbility)
            );

            IsHidden = true;
            ImageUrl = "https://infinitearenas.com/xw2xwa/images/pilots/poedameron-evacuationofdqar.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class BlackOneEoDAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTryAddAction += RestrictSlam;
            HostShip.OnSlam += LoseCharge;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTryAddAction -= RestrictSlam;
            HostShip.OnSlam -= LoseCharge;
        }

        private void RestrictSlam(GenericShip ship, GenericAction action, ref bool canBeUsed)
        {
            if (action is SlamAction)
            {
                canBeUsed = canBeUsed && HostUpgrade.State.Charges > 0;
            }
        }

        private void LoseCharge()
        {
            if (HostUpgrade.State.Charges > 0)
            {
                HostUpgrade.State.LoseCharge();
            }
        }
    }
}
