using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLBYWing
    {
        public class AnakinSkywalker : BTLBYWing
        {
            public AnakinSkywalker() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Anakin Skywalker",
                    "Hero of the Republic",
                    Faction.Republic,
                    6,
                    6,
                    20,
                    isLimited: true,
                    force: 3,
                    abilityText: "After you fully execute a maneuver, if there is an enemy ship in your standard front arc at range 0-1 or in your bullseye arc, you may spend 1 force to remove 1 stress token.",
                    abilityType: typeof(Abilities.SecondEdition.AnakinSkywalkerAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.Turret,
                        UpgradeType.Torpedo,
                        UpgradeType.Gunner,
                        UpgradeType.Astromech,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Jedi,
                        Tags.LightSide,
                        Tags.YWing
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );

                PilotNameCanonical = "anakinskywalker-btlbywing";
            }
        }

        public class AnakinSkywalkerXWA : AnakinSkywalker
        {
            public AnakinSkywalkerXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 12;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 14;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.ForcePower,
                    UpgradeType.Astromech,
                    UpgradeType.Gunner,
                    UpgradeType.Modification,
                    UpgradeType.Device,
                    UpgradeType.Turret,
                    UpgradeType.Torpedo
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}
