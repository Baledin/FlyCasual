using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.RZ1AWing
{
    public class HeraSyndulla : RZ1AWing
    {
        public HeraSyndulla() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Hera Syndulla",
                "Phoenix Leader",
                Faction.Rebel,
                6,
                4,
                5,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.HeraSyndullaABWingAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Missile,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                        Tags.AWing,
                        Tags.Spectre
                },
                skinName: "Hera Syndulla",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            PilotNameCanonical = "herasyndulla-rz1awing";
        }
    }

    public class HeraSyndullaXWA : HeraSyndulla
    {
        public HeraSyndullaXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 11;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 10;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Missile,
                    UpgradeType.Configuration
                };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}