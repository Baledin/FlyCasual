﻿using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEVnSilencer
    {
        public class SienarJaemusEngineer : TIEVnSilencer
        {
            public SienarJaemusEngineer() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Sienar-Jaemus Engineer",
                    "",
                    Faction.FirstOrder,
                    1,
                    5,
                    5,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Torpedo,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/8f7c4680fbc001169baf6538ab259e9b.png";
            }
        }
    }
}
