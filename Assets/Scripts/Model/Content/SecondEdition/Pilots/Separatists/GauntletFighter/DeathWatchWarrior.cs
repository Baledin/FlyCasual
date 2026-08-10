using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.GauntletFighter
{
    public class DeathWatchWarrior : GauntletFighter
    {
        public DeathWatchWarrior() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Death Watch Warrior",
                "Fanatical Adheren",
                Faction.Separatists,
                2,
                7,
                10,
                isLimited: true,
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Crew,
                    UpgradeType.Gunner,
                    UpgradeType.Device,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>()
                {
                    Tags.Mandalorian
                },
                skinName: "CIS Dark",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class DeathWatchWarriorXWA : DeathWatchWarrior
    {
        public DeathWatchWarriorXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 15;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 18;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Crew,
                UpgradeType.Gunner,
                UpgradeType.Illicit,
                UpgradeType.Modification,
                UpgradeType.Modification,
                UpgradeType.Device,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}