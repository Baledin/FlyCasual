using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Delta7BAethersprite
{
    public class AdiGallia : Delta7BAethersprite
    {
        public AdiGallia()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Adi Gallia",
                "Shooting Star",
                Faction.Republic,
                5,
                7,
                18,
                isLimited: true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.AdiGalliaAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Plo Koon",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            PilotNameCanonical = "adigallia-delta7baethersprite";
        }
    }

    public class AdiGalliaXWA : AdiGallia
    {
        public AdiGalliaXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 13;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 7;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.ForcePower,
                    UpgradeType.Astromech,
                    UpgradeType.Modification
                };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}