using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CloneZ95Headhunter
    {
        public class Hawk : CloneZ95Headhunter
        {
            public Hawk() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Hawk\"",
                    "Valkyrie 2929",
                    Faction.Republic,
                    4,
                    3,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.HawkAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );

                PilotNameCanonical = "hawk-clonez95headhunter";
            }
        }

        public class HawkXWA : Hawk
        {
            public HawkXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 8;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 12;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Modification,
                };
            }
        }
    }
}