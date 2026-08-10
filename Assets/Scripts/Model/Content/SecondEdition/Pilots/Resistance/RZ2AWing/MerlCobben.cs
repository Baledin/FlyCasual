using Arcs;
using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.RZ2AWing
{
    public class MerlCobben : RZ2AWing
    {
        public MerlCobben() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Merl Cobben",
                "Distracting Daredevil",
                Faction.Resistance,
                1,
                3,
                4,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.MerlCobbenAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Missile
                },
                tags: new List<Tags>
                {
                    Tags.AWing
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class MerlCobbenXWA : MerlCobben
    {
        public MerlCobbenXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 8;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 5;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Missile
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MerlCobbenAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.AfterGotNumberOfDefenceDiceGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.AfterGotNumberOfDefenceDiceGlobal -= CheckAbility;
        }

        private void CheckAbility(ref int defenseDiceCount)
        {
            if (AreConditionsMet())
            {
                Messages.ShowInfo("Merl Cobben: Defender rolls 1 fewer defense die");
                defenseDiceCount--;
            }
        }

        private bool AreConditionsMet()
        {
            return (Tools.IsFriendly(Combat.Attacker, HostShip)
                && new DistanceInfo(Combat.Attacker, HostShip).Range < 3
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                && Combat.Defender.SectorsInfo.IsShipInSector(HostShip, ArcType.Bullseye));
        }
    }
}