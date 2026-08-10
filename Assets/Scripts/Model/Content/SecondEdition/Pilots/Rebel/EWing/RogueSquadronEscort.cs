using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.EWing
    {
        public class RogueSquadronEscort : EWing
        {
            public RogueSquadronEscort() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Rogue Squadron Escort",
                    "",
                    Faction.Rebel,
                    4,
                    5,
                    12,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech,
                        UpgradeType.Modification
                    },
                    seImageNumber: 52,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }

        public class RogueSquadronEscortXWA : RogueSquadronEscort
        {
            public RogueSquadronEscortXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 14;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 17;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                    UpgradeType.Sensor,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Torpedo
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}