using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.RZ1AWing
{
    public class GreenSquadronPilot : RZ1AWing
    {
        public GreenSquadronPilot() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Green Squadron Pilot",
                "",
                Faction.Rebel,
                3,
                4,
                4,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.AWing
                },
                seImageNumber: 21,
                skinName: "Green",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class GreenSquadronPilotXWA : GreenSquadronPilot
    {
        public GreenSquadronPilotXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 8;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 6;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Missile,
                    UpgradeType.Configuration
                };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}