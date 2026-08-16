using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Delta7BAethersprite
{
    public class AnakinSkywalker7B : Delta7BAethersprite
    {
        public AnakinSkywalker7B()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Anakin Skywalker",
                "Hero of the Republic",
                Faction.Republic,
                6,
                7,
                15,
                true,
                force: 3,
                abilityType: typeof(Abilities.SecondEdition.AnakinSkywalkerAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
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
                skinName: "Anakin Skywalker",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            PilotNameCanonical = "anakinskywalker-delta7baethersprite";
        }
    }

    public class AnakinSkywalker7BXWA : AnakinSkywalker7B
    {
        public AnakinSkywalker7BXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 17;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 14;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.ForcePower,
                    UpgradeType.Astromech,
                    UpgradeType.Modification,
                };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}