using Abilities.SecondEdition;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ExperimentalScanners : GenericUpgrade
    {
        public ExperimentalScanners() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                name: "Experimental Scanners",
                type: UpgradeType.Tech,
                cost: 0,
                abilityType: typeof(ExperimentalScannersAbility)
            );

            IsHidden = true;

            ImageUrl = "https://infinitearenas.com/xw2xwa/images/pilots/longshot-evacuationofdqar.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    // You can acquire locks beyond range 3. You cannot acquire locks at range 1.

    public class ExperimentalScannersAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.SetTargetLockRange(2, int.MaxValue);
        }

        public override void DeactivateAbility()
        {
            HostShip.SetTargetLockRange(1, 3);
        }
    }
}