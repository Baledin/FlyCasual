using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Ship;

namespace UpgradesList.SecondEdition
{
    public class GavinDarklighterPilotAbility : GenericUpgrade
    {
        public GavinDarklighterPilotAbility() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Gavin Darklighter Pilot Ability",
                UpgradeType.Pilot,

                cost: 8,
                abilityType: typeof(Abilities.SecondEdition.GavinDarklighterAbility)
            );
            ImageUrl = "https://raw.githubusercontent.com/sampson-matt/Hotac-Upgrade-Cards/main/PilotAbilities/Rebel/gavindarklighter.png";
        }


    }
}