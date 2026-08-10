using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.RZ2AWing
{
    public class GreenSquadronExpert : RZ2AWing
    {
        public GreenSquadronExpert() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Green Squadron Expert",
                "",
                Faction.Resistance,
                3,
                4,
                2,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Tech
                },
                tags: new List<Tags>
                {
                    Tags.AWing
                },
                skinName: "Green",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class GreenSquadronExpertXWA : GreenSquadronExpert
    {
        public GreenSquadronExpertXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 9;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 10;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Talent,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Missile
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}