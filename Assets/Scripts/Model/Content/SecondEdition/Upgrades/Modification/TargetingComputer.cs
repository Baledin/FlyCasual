using Actions;
using ActionsList;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class TargetingComputer : GenericUpgrade
    {
        public TargetingComputer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Targeting Computer",
                UpgradeType.Modification,
                cost: 1,
                addAction: new ActionInfo(typeof(TargetLockAction)),
                legalityInfo: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class TargetingComputerXWA : TargetingComputer
    {
        public TargetingComputerXWA() : base()
        {
            UpgradeInfo.Cost = 2;
            UpgradeInfo.LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}