using Arcs;
using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.T70XWing
{
    public class VenisaDoza : T70XWing
    {
        public VenisaDoza() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Venisa Doza",
                "Jade Leader",
                Faction.Resistance,
                4,
                4,
                7,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.VenisaDozaAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Tech,
                    UpgradeType.Missile,
                    UpgradeType.Missile,
                    UpgradeType.Astromech,
                    UpgradeType.Modification,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.XWing
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class VenisaDozaXWA : VenisaDoza
    {
        public VenisaDozaXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 13;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 17;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Talent,
                UpgradeType.Astromech,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Missile,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class VenisaDozaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGameStart += UpdateArcRequirements;
            HostShip.OnUpdateWeaponRange += CheckRange;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGameStart -= UpdateArcRequirements;
            HostShip.OnUpdateWeaponRange -= CheckRange;
        }

        private void CheckRange(IShipWeapon weapon, ref int minRange, ref int maxRange, GenericShip target)
        {
            if (weapon is GenericSpecialWeapon)
            {
                GenericSpecialWeapon specialWeapon = weapon as GenericSpecialWeapon;
                if (specialWeapon.UpgradeInfo.HasType(UpgradeType.Missile)
                    || specialWeapon.UpgradeInfo.HasType(UpgradeType.Torpedo)
                    && Board.GetShipsInArcAtRange(HostShip, ArcType.Rear, new UnityEngine.Vector2(0, 4), Team.Type.Enemy).Contains(target))
                {
                    minRange = 1;
                    maxRange = 2;
                }
            }
        }

        private void UpdateArcRequirements()
        {
            foreach (GenericUpgrade weaponUpgrade in HostShip.UpgradeBar.GetSpecialWeaponsAll())
            {
                IShipWeapon specialWeapon = weaponUpgrade as IShipWeapon;
                if (specialWeapon.WeaponType == WeaponTypes.Torpedo || specialWeapon.WeaponType == WeaponTypes.Missile)
                {
                    if (specialWeapon.WeaponInfo.ArcRestrictions.Contains(ArcType.Front))
                    {
                        specialWeapon.WeaponInfo.ArcRestrictions.Add(ArcType.Rear);
                    }
                }
            }
        }
    }
}