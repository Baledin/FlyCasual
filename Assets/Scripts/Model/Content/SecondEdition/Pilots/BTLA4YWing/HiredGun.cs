﻿using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class HiredGun : BTLA4YWing
        {
            public HiredGun() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hired Gun",
                    2,
                    31,
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Illicit, UpgradeType.Modification },
                    factionOverride: Faction.Scum
                );

                ModelInfo.SkinName = "Gray";
            }
        }
    }
}
