using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.TIEVnSilencer
{
    public class SienarJaemusEngineer : TIEVnSilencer
    {
        public SienarJaemusEngineer() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Sienar-Jaemus Engineer",
                "",
                Faction.FirstOrder,
                1,
                5,
                5,
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Tech,
                    UpgradeType.Torpedo,
                    UpgradeType.Missile,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.Tie
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class SienarJaemusEngineerXWA : SienarJaemusEngineer
    {
        public SienarJaemusEngineerXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 11;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 7;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Tech,
                UpgradeType.Missile,
                UpgradeType.Torpedo,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}