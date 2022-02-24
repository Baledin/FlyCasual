﻿using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESfFighter
    {
        public class OmegaSquadronExpert : TIESfFighter
        {
            public OmegaSquadronExpert() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Omega Squadron Expert",
                    "",
                    Faction.FirstOrder,
                    3,
                    4,
                    10,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/784d00f653ff7cd58cb634c7a59e47c1.png";
            }
        }
    }
}
