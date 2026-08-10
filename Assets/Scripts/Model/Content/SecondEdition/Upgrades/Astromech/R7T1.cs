using Actions;
using ActionsList;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class R7T1 : GenericUpgrade
    {
        public R7T1() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R7-T1",
                UpgradeType.Astromech,
                cost: 5,
                isLimited: true,
                addActionLink: new LinkedActionInfo(typeof(TargetLockAction), typeof(BoostAction)),
                restrictions: new UpgradeCardRestrictions(
                    new ActionBarRestriction(typeof(TargetLockAction), ActionColor.White),
                    new BaseSizeRestriction(Ship.BaseSize.Small),
                    new FactionRestriction(Faction.Resistance, Faction.Rebel)
                ),
                legalityInfo: new List<Legality>() { Legality.XWA }
            );

            NameCanonical = "r7t1-legendsandrelics";
        }
    }
}