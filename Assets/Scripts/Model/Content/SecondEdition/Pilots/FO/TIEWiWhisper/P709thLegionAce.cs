using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.TIEWiWhisperModifiedInterceptor
{
    public class P709thLegionAce : TIEWiWhisperModifiedInterceptor
    {
        public P709thLegionAce() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "709th Legion Ace",
                "",
                Faction.FirstOrder,
                4,
                4,
                10,
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Missile,
                    UpgradeType.Tech,
                    UpgradeType.Tech,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.Tie
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class P709thLegionAceXWA : P709thLegionAce
    {
        public P709thLegionAceXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 10;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 8;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Gunner,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Tech,
                UpgradeType.Missile,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}