using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.BTLA4YWing
{
    public class GraySquadronBomber : BTLA4YWing
    {
        public GraySquadronBomber() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Gray Squadron Bomber",
                "",
                Faction.Rebel,
                2,
                4,
                8,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Device,
                    UpgradeType.Missile,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.YWing
                },
                seImageNumber: 18,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class GraySquadronBomberXWA : GraySquadronBomber
    {
        public GraySquadronBomberXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 7;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 4;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Astromech,
                    UpgradeType.Modification,
                    UpgradeType.Device,
                    UpgradeType.Turret,
                    UpgradeType.Torpedo
                };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}