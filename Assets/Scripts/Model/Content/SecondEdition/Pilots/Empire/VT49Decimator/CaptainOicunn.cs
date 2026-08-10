using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.VT49Decimator
    {
        public class CaptainOicunn : VT49Decimator
        {
            public CaptainOicunn() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Captain Oicunn",
                    "Inspired Tactician",
                    Faction.Imperial,
                    3,
                    7,
                    19,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CaptainOicunnAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Modification,
                        UpgradeType.Device,
                        UpgradeType.Torpedo,
                        UpgradeType.Title
                    },
                    seImageNumber: 146,
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class CaptainOicunnXWA : CaptainOicunn
        {
            public CaptainOicunnXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 16;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 14;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Crew,
                    UpgradeType.Crew,
                    UpgradeType.Gunner,
                    UpgradeType.Gunner,
                    UpgradeType.Modification,
                    UpgradeType.Device,
                    UpgradeType.Torpedo,
                    UpgradeType.Title
                };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainOicunnAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            ShotInfo.OnRangeIsMeasured += SetMinRange;
        }

        public override void DeactivateAbility()
        {
            ShotInfo.OnRangeIsMeasured -= SetMinRange;
        }

        private void SetMinRange(GenericShip thisShip, GenericShip anotherShip, IShipWeapon chosenWeapon, ref int range)
        {
            if (Combat.Attacker == HostShip && thisShip == HostShip && range == 0)
            {
                range = 1;
            }
        }

    }
}
