using Content;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class SyncedLaserCannons : GenericSpecialWeapon
    {
        public SyncedLaserCannons() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Synced Laser Cannons",
                types: new List<UpgradeType>()
                {
                    UpgradeType.Cannon,
                    UpgradeType.Cannon
                },
                cost: 6,
                weaponInfo: new SyncedLaserCannonsWeaponInfo(this),
                legalityInfo: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }

        private class SyncedLaserCannonsWeaponInfo : SpecialWeaponInfo
        {
            private GenericUpgrade HostUpgrade;
            public SyncedLaserCannonsWeaponInfo(GenericUpgrade hostUpgrade) : base(3, 2, 3)
            {
                HostUpgrade = hostUpgrade;
            }

            public override bool NoRangeBonus
            {
                get
                {
                    if (Combat.AttackStep == CombatStep.Defence
                        && Combat.Attacker == HostUpgrade.HostShip
                        && HostUpgrade.HostShip.Tokens.HasToken<Tokens.CalculateToken>())
                        return true;
                    else
                        return false;
                }
            }
        }
    }

    public class SyncedLaserCannonsXWA : SyncedLaserCannons
    {
        public SyncedLaserCannonsXWA() : base()
        {
            UpgradeInfo.Cost = 9;
            UpgradeInfo.LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}