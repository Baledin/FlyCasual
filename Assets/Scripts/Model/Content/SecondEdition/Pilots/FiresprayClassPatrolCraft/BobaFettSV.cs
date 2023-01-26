﻿using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class BobaFettSV : FiresprayClassPatrolCraft
        {
            public BobaFettSV() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Boba Fett",
                    5,
                    91,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.BobaFettScumAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Crew },
                    seImageNumber: 149
                );

                ModelInfo.SkinName = "Boba Fett";
            }
        }
    }
}