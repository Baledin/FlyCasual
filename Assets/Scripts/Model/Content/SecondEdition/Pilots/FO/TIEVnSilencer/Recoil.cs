using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.TIEVnSilencer
{
    public class Recoil : TIEVnSilencer
    {
        public Recoil() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "\"Recoil\"",
                "Quantity Over Quality",
                Faction.FirstOrder,
                4,
                5,
                10,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.RecoilAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Tech,
                    UpgradeType.Torpedo,
                    UpgradeType.Missile,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.Tie
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class RecoilXWA : Recoil
    {
        public RecoilXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 13;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 8;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Missile,
                UpgradeType.Torpedo,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RecoilAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnBullseyeArcCheck += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnBullseyeArcCheck -= CheckAbility;
        }

        private void CheckAbility(GenericShip anotherShip, ref bool isInBullseyeArc)
        {
            if (isInBullseyeArc) return;

            if (!HostShip.IsStressed) return;
            if (!HostShip.SectorsInfo.IsShipInSector(anotherShip, Arcs.ArcType.Front)) return;
            if (HostShip.SectorsInfo.RangeToShipBySector(anotherShip, Arcs.ArcType.Front) > 1) return;

            isInBullseyeArc = true;
        }
    }
}