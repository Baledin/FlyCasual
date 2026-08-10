using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.RZ2AWing
{
    public class BlueSquadronRecruit : RZ2AWing
    {
        public BlueSquadronRecruit() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Blue Squadron Recruit",
                "",
                Faction.Resistance,
                1,
                4,
                4,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Tech
                },
                skinName: "Blue",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class BlueSquadronRecruitXWA : BlueSquadronRecruit
    {
        public BlueSquadronRecruitXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 8;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 4;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Missile
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}