using ActionList;
using ActionsList;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESkStriker
    {
        public class Duchess : TIESkStriker
        {
            public Duchess() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Duchess\"",
                    "Urbane Ace",
                    Faction.Imperial,
                    5,
                    4,
                    7,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    abilityType: typeof(Abilities.SecondEdition.DuchessAbility),
                    seImageNumber: 117,
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class DuchessXWA : Duchess
        {
            public DuchessXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 11;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 19;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Gunner,
                    UpgradeType.Modification,
                    UpgradeType.Modification,
                    UpgradeType.Device,
                };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DuchessAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCanPerformActionWhileStressed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCanPerformActionWhileStressed -= CheckAbility;
        }

        public void CheckAbility(GenericAction action, ref bool isAvailable)
        {
            isAvailable = isAvailable || action is AdaptiveAileronsAction;
        }
    }
}