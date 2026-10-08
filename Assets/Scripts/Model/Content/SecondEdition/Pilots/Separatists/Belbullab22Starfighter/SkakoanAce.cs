using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Belbullab22Starfighter
{
    public class SkakoanAce : Belbullab22Starfighter
    {
        public SkakoanAce()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Skakoan Ace",
                "",
                Faction.Separatists,
                3,
                4,
                4,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Modification
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class SkakoanAceXWA : SkakoanAce
    {
        public SkakoanAceXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 10;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 10;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}