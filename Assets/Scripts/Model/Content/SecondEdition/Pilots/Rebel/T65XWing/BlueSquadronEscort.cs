using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.T65XWing
{
    public class BlueSquadronEscort : T65XWing
    {
        public BlueSquadronEscort() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Blue Squadron Escort",
                "",
                Faction.Rebel,
                2,
                5,
                4,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Astromech,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.XWing
                },
                seImageNumber: 11,
                skinName: "Blue",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class BlueSquadronEscortXWA : BlueSquadronEscort
    {
        public BlueSquadronEscortXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 9;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 3;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Astromech,
                UpgradeType.Modification,
                UpgradeType.Torpedo,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}