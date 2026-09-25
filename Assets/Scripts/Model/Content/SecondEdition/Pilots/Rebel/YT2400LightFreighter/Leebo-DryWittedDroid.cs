using Content;
using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.YT2400LightFreighter
{
    public class LeeboDryWittedDroid : YT2400LightFreighter
    {
        public LeeboDryWittedDroid() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Leebo",
                "Dry-Witted Droid",
                Faction.Rebel,
                3,
                8,
                16,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.LeeboAbility),
                tags: new List<Tags>
                {
                    Tags.Droid,
                    Tags.Freighter
                },
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Missile,
                    UpgradeType.Illicit,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Title
                },
                legality: new List<Legality> { Legality.StandardBanned, Legality.ExtendedBanned }
            );

            ShipInfo.ActionIcons.SwitchToDroidActions();
        }
    }

    public class LeeboDryWittedDroidXWA : LeeboDryWittedDroid
    {
        public LeeboDryWittedDroidXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 19;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 19;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Crew,
                UpgradeType.Gunner,
                UpgradeType.Illicit,
                UpgradeType.Illicit,
                UpgradeType.Modification,
                UpgradeType.Missile
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}