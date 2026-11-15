using Content;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class R2D2 : GenericUpgrade
    {
        public R2D2() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R2-D2",
                UpgradeType.Astromech,
                cost: 8,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.R2AstromechAbility),
                restriction: new FactionRestriction(Faction.Rebel),
                charges: 3,
                seImageNumber: 100,
                legalityInfo: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            NameCanonical = "r2d2";

            ImageUrl = "Upgrades/r2d2-crew";
        }
    }

    public class R2D2XWA : R2D2
    {
        public R2D2XWA() : base()
        {
            UpgradeInfo.Cost = 8;
            UpgradeInfo.LegalityInfo = new List<Legality> { Legality.XWA };
            NameCanonical = "r2d2-crew";
        }
    }
}