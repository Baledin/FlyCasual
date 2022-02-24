﻿using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ2AWing
    {
        public class GreenSquadronExpert : RZ2AWing
        {
            public GreenSquadronExpert() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Green Squadron Expert",
                    "",
                    Faction.Resistance,
                    3,
                    4,
                    2,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.AWing
                    },
                    skinName: "Green"
                );

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/3f7ad9efb4c5af8b4d1f5c07a3c7538b.png";
            }
        }
    }
}