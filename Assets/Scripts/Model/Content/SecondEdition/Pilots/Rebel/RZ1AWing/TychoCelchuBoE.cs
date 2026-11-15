using Abilities.SecondEdition;
using Actions;
using ActionsList;
using Content;
using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.RZ1AWing
{
    public class TychoCelchuBoELSL : RZ1AWing
    {
        public TychoCelchuBoELSL() : base()
        {
            PilotInfo = new PilotCardInfo25(
                "Tycho Celchu",
                "Battle Over Endor",
                Faction.Rebel,
                5,
                4,
                loadoutValue: 0,
                isLimited: true,
                abilityType: typeof(TychoCelchuBattleOverEndorAbility),
                tags: new List<Tags>
                {
                    Tags.AWing
                },
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Missile,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                isStandardLayout: true,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            PilotNameCanonical = "tychocelchu-battleoverendor";

            ShipInfo.Shields++;

            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(FocusAction), typeof(ReloadAction)));
            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BoostAction), typeof(EvadeAction)));

            MustHaveUpgrades.Add(typeof(VectoredCannonsRZ1));
            MustHaveUpgrades.Add(typeof(ItsATrap));
            MustHaveUpgrades.Add(typeof(Juke));
            MustHaveUpgrades.Add(typeof(ProtonRockets));
            MustHaveUpgrades.Add(typeof(ChaffParticlesBoE));
        }
    }

    public class TychoCelchuBoEXWA : TychoCelchuBoELSL
    {
        public TychoCelchuBoEXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 11;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TychoCelchuBattleOverEndorAbility : GenericAbility
    {
        //While you are disarmed, you can still perform missile attacks. When you perform a missile attack while disarmed, roll a maximum of 4 dice.
        GenericSpecialWeapon secondaryWeapon;

        public override void ActivateAbility()
        {
            HostShip.OnWeaponsDisabledCheck += AllowMissileAttacks;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnWeaponsDisabledCheck -= AllowMissileAttacks;
        }

        private void AllowMissileAttacks(ref bool result)
        {
            if (HostShip.Tokens.CountTokensByType(typeof(WeaponsDisabledToken)) != 1) return;

            if (!IsMissileAttack()) return;

            Messages.ShowInfo("The attack using " + secondaryWeapon.Name + " is allowed");

            result = false;

            PrepareAttackDiceCap();
        }

        private bool IsMissileAttack()
        {
            secondaryWeapon = Combat.ChosenWeapon as GenericSpecialWeapon;

            return (secondaryWeapon != null && secondaryWeapon.HasType(UpgradeType.Missile));
        }

        private void PrepareAttackDiceCap()
        {
            HostShip.AfterGotNumberOfAttackDiceCap += SetAttackDiceCap;

            HostShip.OnAttackFinish += RemoveAttackDiceCap;
        }

        private void SetAttackDiceCap(ref int count)
        {
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName} has a disarmed token, only 4 dice may be rolled when attacking with {secondaryWeapon.Name}");

            if (count > 4) count = 4;
        }

        private void RemoveAttackDiceCap(GenericShip ship)
        {
            HostShip.AfterGotNumberOfAttackDiceCap -= SetAttackDiceCap;

            HostShip.OnAttackFinish -= RemoveAttackDiceCap;
        }
    }
}