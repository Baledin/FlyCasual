using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.ARC170Starfighter
{
    public class NorraWexley : ARC170Starfighter
    {
        public NorraWexley() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Norra Wexley",
                "Gold Nine",
                Faction.Rebel,
                5,
                5,
                8,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.NorraWexleyAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Torpedo,
                    UpgradeType.Gunner,
                    UpgradeType.Astromech,
                    UpgradeType.Modification
                },
                seImageNumber: 65,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class NorraWexleyXWA : NorraWexley
    {
        public NorraWexleyXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 13;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 16;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Astromech,
                UpgradeType.Crew,
                UpgradeType.Gunner,
                UpgradeType.Modification,
                UpgradeType.Torpedo
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}