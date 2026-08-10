using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.TIEVnSilencer
{
    public class FirstOrderTestPilot : TIEVnSilencer
    {
        public FirstOrderTestPilot() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "First Order Test Pilot",
                "",
                Faction.FirstOrder,
                4,
                5,
                5,
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Tech,
                    UpgradeType.Torpedo,
                    UpgradeType.Missile,
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

    public class FirstOrderTestPilotXWA : FirstOrderTestPilot
    {
        public FirstOrderTestPilotXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 12;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 7;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Missile,
                UpgradeType.Torpedo,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}