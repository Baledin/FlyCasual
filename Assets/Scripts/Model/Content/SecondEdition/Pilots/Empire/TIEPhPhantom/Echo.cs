using BoardTools;
using Content;
using Movement;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.TIEPhPhantom
{
    public class Echo : TIEPhPhantom
    {
        public Echo() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "\"Echo\"",
                "Slippery Trickster",
                Faction.Imperial,
                4,
                5,
                9,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.EchoAbility),
                tags: new List<Tags>
                {
                    Tags.Tie
                },
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Gunner,
                    UpgradeType.Modification
                },
                seImageNumber: 132,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            ModelInfo.SkinName = "Echo";
        }
    }

    public class EchoXWA : Echo
    {
        public EchoXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 13;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 15;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Sensor,
                UpgradeType.Gunner,
                UpgradeType.Modification,
                UpgradeType.Tech
            };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EchoAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableDecloakBarrelRollTemplates += ChangeDecloakTemplates;
            HostShip.OnGetAvailableDecloakBoostTemplates += ChangeDecloakTemplates;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableDecloakBarrelRollTemplates -= ChangeDecloakTemplates;
            HostShip.OnGetAvailableDecloakBoostTemplates -= ChangeDecloakTemplates;
        }

        private void ChangeDecloakTemplates(List<ManeuverTemplate> availableTemplates)
        {
            if (availableTemplates.Any(n => n.Name == "Straight 2"))
            {
                availableTemplates.RemoveAll(n => n.Name == "Straight 2");
                availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, ManeuverSpeed.Speed2));
                availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, ManeuverSpeed.Speed2));
            }
        }
    }
}