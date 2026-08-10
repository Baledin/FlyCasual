using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Z95AF4Headhunter
{
    public class TalaSquadronPilot : Z95AF4Headhunter
    {
        public TalaSquadronPilot() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Tala Squadron Pilot",
                "",
                Faction.Rebel,
                2,
                3,
                4,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Modification
                },
                seImageNumber: 29,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class TalaSquadronPilotXWA : TalaSquadronPilot
    {
        public TalaSquadronPilotXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 7;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 11;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Talent,
                UpgradeType.Modification,
                UpgradeType.Missile
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}