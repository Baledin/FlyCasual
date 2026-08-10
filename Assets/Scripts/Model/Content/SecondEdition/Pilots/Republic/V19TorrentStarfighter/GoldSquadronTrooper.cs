using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class GoldSquadronTrooper : V19TorrentStarfighter
    {
        public GoldSquadronTrooper()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Gold Squadron Trooper",
                "",
                Faction.Republic,
                2,
                4,
                6,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Missile,
                },
                tags: new List<Tags>
                {
                    Tags.Clone
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class GoldSquadronTrooperXWA : GoldSquadronTrooper
    {
        public GoldSquadronTrooperXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 6;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 2;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Modification,
                UpgradeType.Modification,
                UpgradeType.Missile
            };
        }
    }
}