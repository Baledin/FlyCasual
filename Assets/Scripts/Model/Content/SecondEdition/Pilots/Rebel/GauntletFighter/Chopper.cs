using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.GauntletFighter
{
    public class Chopper : GauntletFighter
    {
        public Chopper() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "\"Chopper\"",
                "Spectre-3",
                Faction.Rebel,
                2,
                6,
                10,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.ChopperPilotAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Modification,
                        UpgradeType.Configuration,
                        UpgradeType.Title
                },
                tags: new List<Tags>()
                {
                        Tags.Droid,
                        Tags.Spectre
                },
                skinName: "Red",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            ShipInfo.ActionIcons.SwitchToDroidActions();

            PilotNameCanonical = "chopper-gauntletfighter";
        }
    }

    public class ChopperXWA : Chopper
    {
        public ChopperXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 15;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 16;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
                {
                    UpgradeType.Crew,
                    UpgradeType.Gunner,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Modification,
                    UpgradeType.Device,
                    UpgradeType.Configuration,
                    UpgradeType.Title
                };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}