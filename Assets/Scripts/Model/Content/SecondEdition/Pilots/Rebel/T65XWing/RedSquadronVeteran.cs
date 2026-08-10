using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.T65XWing
{
    public class RedSquadronVeteran : T65XWing
    {
        public RedSquadronVeteran() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Red Squadron Veteran",
                "",
                Faction.Rebel,
                3,
                5,
                3,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.XWing
                },
                seImageNumber: 10,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class RedSquadronVeteranXWA : RedSquadronVeteran
    {
        public RedSquadronVeteranXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 10;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 10;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Talent,
                UpgradeType.Astromech,
                UpgradeType.Modification,
                UpgradeType.Torpedo,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}