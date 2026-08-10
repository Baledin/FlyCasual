using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.T70XWing
{
    public class RedSquadronExpert : T70XWing
    {
        public RedSquadronExpert() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Red Squadron Expert",
                "",
                Faction.Resistance,
                3,
                5,
                2,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Tech,
                    UpgradeType.Astromech,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.XWing
                },
                skinName: "Red",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class RedSquadronExpertXWA : RedSquadronExpert
    {
        public RedSquadronExpertXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 11;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 11;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Talent,
                UpgradeType.Astromech,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}