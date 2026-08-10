using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Delta7BAethersprite
{
    public class MaceWindu7B : Delta7BAethersprite
    {
        public MaceWindu7B()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Mace Windu",
                "Harsh Traditionalist",
                Faction.Republic,
                4,
                5,
                7,
                isLimited: true,
                force: 3,
                abilityType: typeof(Abilities.SecondEdition.MaceWinduAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.ForcePower,
                    UpgradeType.Astromech,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Mace Windu",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            PilotNameCanonical = "macewindu-delta7baethersprite";
        }
    }

    public class MaceWindu7BXWA : MaceWindu7B
    {
        public MaceWindu7BXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 13;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 9;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}