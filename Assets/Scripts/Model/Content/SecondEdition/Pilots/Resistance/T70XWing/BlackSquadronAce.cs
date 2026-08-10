using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.T70XWing
{
    public class BlackSquadronAce : T70XWing
    {
        public BlackSquadronAce() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Black Squadron Ace",
                "",
                Faction.Resistance,
                4,
                5,
                10,
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
                skinName: "Black One",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            PilotNameCanonical = "blacksquadronace-t70xwing";
        }
    }

    public class BlackSquadronAceXWA : BlackSquadronAce
    {
        public BlackSquadronAceXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 12;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 14;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Talent,
                UpgradeType.Astromech,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Torpedo,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}