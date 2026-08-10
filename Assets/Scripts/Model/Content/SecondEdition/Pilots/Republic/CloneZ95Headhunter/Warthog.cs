using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CloneZ95Headhunter
    {
        public class Warthog : CloneZ95Headhunter
        {
            public Warthog() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Warthog\"",
                    "Veteran of Kadavo",
                    Faction.Republic,
                    3,
                    3,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.WarthogAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );

                PilotNameCanonical = "warthog-clonez95headhunter";
            }
        }

        public class WarthogXWA : Warthog
        {
            public WarthogXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 9;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 21;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}