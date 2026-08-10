using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CloneZ95Headhunter
    {
        public class SeventhSkyCorpsPilot : CloneZ95Headhunter
        {
            public SeventhSkyCorpsPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "7th Sky Corps Pilot",
                    "",
                    Faction.Republic,
                    2,
                    3,
                    3,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class SeventhSkyCorpsPilotXWA : SeventhSkyCorpsPilot
        {
            public SeventhSkyCorpsPilotXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 7;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 10;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}