using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.RZ1AWing
{
    public class SharaBey : RZ1AWing
    {
        public SharaBey() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Shara Bey",
                "Green Four",
                Faction.Rebel,
                4,
                4,
                7,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.SharaBeyAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                        Tags.AWing
                },
                abilityText: "While you defend or perform a primary attack, you may spend 1 lock you have on the enemy ship to add 1 focus result to your dice results.",
                skinName: "Red",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            PilotNameCanonical = "sharabey-rz1awing";
        }
    }

    public class SharaBeyXWA : SharaBey
    {
        public SharaBeyXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 9;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 12;
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