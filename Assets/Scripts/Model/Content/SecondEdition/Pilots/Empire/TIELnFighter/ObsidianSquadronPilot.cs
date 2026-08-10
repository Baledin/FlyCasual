using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class ObsidianSquadronPilot : TIELnFighter
        {
            public ObsidianSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Obsidian Squadron Pilot",
                    "",
                    Faction.Imperial,
                    2,
                    2,
                    0,
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 91,
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class ObsidianSquadronPilotXWA : ObsidianSquadronPilot
        {
            public ObsidianSquadronPilotXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 5;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 0;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Modification
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}
