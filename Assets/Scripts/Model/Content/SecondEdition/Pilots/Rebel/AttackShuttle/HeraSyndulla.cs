using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.AttackShuttle
{
    public class HeraSyndulla : AttackShuttle
    {
        public HeraSyndulla() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Hera Syndulla",
                "Spectre-2",
                Faction.Rebel,
                5,
                4,
                9,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.HeraSyndullaAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Turret,
                    UpgradeType.Crew,
                    UpgradeType.Modification,
                    UpgradeType.Title
                },
                seImageNumber: 34,
                tags: new List<Tags>
                {
                    Tags.Spectre
                },
                legality: new List<Legality>() { Legality.ExtendedLegal }
            );
        }
    }

    public class HeraSyndullaXWA : HeraSyndulla
    {
        public HeraSyndullaXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 10;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 15;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Talent,
                UpgradeType.Talent,
                UpgradeType.Crew,
                UpgradeType.Modification,
                UpgradeType.Turret,
                UpgradeType.Title
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}