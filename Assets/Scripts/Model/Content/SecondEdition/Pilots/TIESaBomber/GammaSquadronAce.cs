﻿using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESaBomber
    {
        public class GammaSquadronAce : TIESaBomber
        {
            public GammaSquadronAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Gamma Squadron Ace",
                    3,
                    29,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 111
                );

                ModelInfo.SkinName = "Gamma Squadron";
            }
        }
    }
}
