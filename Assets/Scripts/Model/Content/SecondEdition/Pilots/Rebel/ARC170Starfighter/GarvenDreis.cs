using Content;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.ARC170Starfighter
{
    public class GarvenDreis : ARC170Starfighter
    {
        public GarvenDreis() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Garven Dreis",
                "Red Leader",
                Faction.Rebel,
                4,
                4,
                7,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.GarvenDreisArcAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Cannon,
                    UpgradeType.Missile,
                    UpgradeType.Gunner,
                    UpgradeType.Astromech,
                    UpgradeType.Modification
                },
                seImageNumber: 66,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class GarvenDreisXWA : GarvenDreis
    {
        public GarvenDreisXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 12;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 15;
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

namespace Abilities.SecondEdition
{
    public class GarvenDreisArcAbility : GarvenDreisXWingAbility
    {
        protected override bool FilterAbilityTarget(GenericShip ship)
        {
            // Change the ability to work at r3.
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 3);
        }
    }
}