using Abilities.SecondEdition;
using Actions;
using ActionsList;
using Content;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.TIEDDefender
{
    public class CaptainYorrBoE : TIEDDefender
    {
        public CaptainYorrBoE() : base()
        {
            PilotInfo = new PilotCardInfo25(
                "Captain Yorr",
                "Battle Over Endor",
                Faction.Imperial,
                4,
                6,
                loadoutValue: 0,
                tags: new List<Tags>
                {
                    Tags.Tie
                },
                isStandardLayout: true,
                isLimited: true,
                charges: 2,
                abilityType: typeof(CaptainYorrBattleOverEndorAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Cannon,
                    UpgradeType.Modification
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            ImageUrl = "https://infinitearenas.com/xw2/images/quickbuilds/captainyorr-battleoverendor.png";

            ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Configuration);
            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(EvadeAction), typeof(BarrelRollAction)));

            FullThrottleAbility oldAbility = (FullThrottleAbility)ShipAbilities.First(n => n.GetType() == typeof(FullThrottleAbility));
            ShipAbilities.Remove(oldAbility);
            ShipAbilities.Add(new ChissEngineeringAbility());

            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.NoEscape));
            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.Predator));
            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.IonCannon));
            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.ComputerAssistedHandling));

            PilotNameCanonical = "captainyorr-battleoverendor";
        }
    }

    public class CaptainYorrBoEXWA : CaptainYorrBoE
    {
        public CaptainYorrBoEXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 16;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainYorrBattleOverEndorAbility : GenericAbility
    {
        //After you perform a primary attack that hits, you may spend 1 to perform a bonus attack.
        public override void ActivateAbility()
        {
            HostShip.OnAttackHitAsAttacker += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackHitAsAttacker -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (HostShip.State.Charges < 1) return;
            if (HostShip.IsCannotAttackSecondTime) return;
            if (Combat.ChosenWeapon.WeaponType != WeaponTypes.PrimaryWeapon) return;
            if (!HasCannonWeapon()) return;

            HostShip.OnCombatCheckExtraAttack += RegisterSecondAttackTrigger;
        }

        private bool HasCannonWeapon()
        {
            return HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Count(n => n.HasType(UpgradeType.Cannon) && (n as IShipWeapon) != null) > 0;
        }

        private void RegisterSecondAttackTrigger(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterSecondAttackTrigger;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, PerformBonusAttack);
        }

        private void PerformBonusAttack(object sender, System.EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartSelectAttackTarget(
                    HostShip,
                    FinishAdditionalAttack,
                    IsCannonShot,
                    HostShip.PilotInfo.PilotName,
                    "You may spend 1 Charge perform a bonus Cannon attack",
                    HostShip
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack an additional time", HostShip.PilotInfo.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private void FinishAdditionalAttack()
        {
            //if bonus attack was skipped, allow bonus attacks again
            if (Selection.ThisShip.IsAttackSkipped)
            {
                Selection.ThisShip.IsCannotAttackSecondTime = false;
            }
            else
            {
                Selection.ThisShip.IsAttackPerformed = true;
                HostShip.SpendCharge();
            }

            Triggers.FinishTrigger();
        }

        private bool IsCannonShot(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = false;

            GenericSpecialWeapon upgradeWeapon = weapon as GenericSpecialWeapon;
            if (upgradeWeapon != null && upgradeWeapon.HasType(UpgradeType.Cannon))
            {
                result = true;
            }
            else
            {
                if (!isSilent) Messages.ShowError("This attack must be performed using a Cannon");
            }

            return result;
        }
    }
}