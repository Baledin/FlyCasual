using Abilities.SecondEdition;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.ResistanceTransport
{
    public class PammichNerroGoode : ResistanceTransport
    {
        public PammichNerroGoode()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Pammich Nerro Goode",
                "D’Qar Dispatcher",
                Faction.Resistance,
                3,
                4,
                12,
                isLimited: true,
                abilityType: typeof(PammichNerroGoodeAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Astromech,
                    UpgradeType.Crew,
                    UpgradeType.Crew,
                    UpgradeType.Sensor,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Cannon,
                    UpgradeType.Cannon,
                    UpgradeType.Torpedo
                },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
        }
    }

    public class PammichNerroGoodeXWA : PammichNerroGoode
    {
        public PammichNerroGoodeXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 11;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 22;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Astromech,
                UpgradeType.Crew,
                UpgradeType.Crew,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Cannon,
                UpgradeType.Cannon,
                UpgradeType.Torpedo
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PammichNerroGoodeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTryCanPerformRedManeuverWhileStressed += CheckRedManeuversWhileStressed;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTryCanPerformRedManeuverWhileStressed -= CheckRedManeuversWhileStressed;
        }

        private void CheckRedManeuversWhileStressed(ref bool isAllowed)
        {
            if (HostShip.Tokens.CountTokensByType(typeof(Tokens.StressToken)) <= 2)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": Red maneuver is allowed");
                isAllowed = true;
            }
        }
    }
}