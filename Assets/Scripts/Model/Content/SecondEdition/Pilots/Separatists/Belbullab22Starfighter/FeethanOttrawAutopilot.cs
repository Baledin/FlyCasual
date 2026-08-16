using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Belbullab22Starfighter
{
    public class FeethanOttrawAutopilot : Belbullab22Starfighter
    {
        public FeethanOttrawAutopilot()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Feethan Ottraw Autopilot",
                "",
                Faction.Separatists,
                1,
                4,
                5,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.TacticalRelay,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            ShipInfo.ActionIcons.SwitchToDroidActions();
        }
    }

    public class FeethanOttrawAutopilotXWA : FeethanOttrawAutopilot
    {
        public FeethanOttrawAutopilotXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 9;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 6;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Modification,
                UpgradeType.Modification,
                UpgradeType.TacticalRelay
            };
        }
    }
}