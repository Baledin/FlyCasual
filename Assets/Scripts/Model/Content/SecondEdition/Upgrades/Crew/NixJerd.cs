using Abilities.SecondEdition;
using ActionsList;
using Content;
using SubPhases;
using System;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class NixJerd : GenericUpgrade
    {
        public NixJerd() : base()
        {
            UpgradeInfo = new
            (
                name: "Nix Jerd",
                type: UpgradeType.Crew,
                cost: 0,
                isLimited: true,
                abilityType: typeof(NixJerdAbility),
                charges: 2,
                legalityInfo: new() { Legality.XWA }
            );

            IsHidden = true;
        }
	}
}

namespace Abilities.SecondEdition
{
    // While you perform a reload action, you may spend 1 charge and gain 1 strain token.
    // If you do, you may recover 1 additional charge on a device upgrade.
    public class NixJerdAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= RegisterAbility;
        }

        private void RegisterAbility(GenericAction action)
        {
            if(action is ReloadAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            if (HostUpgrade.State.Charges > 0 && HostShip.UpgradeBar.GetInstalledUpgrades(UpgradeType.Device).Any(u => !u.UpgradeInfo.CannotBeRecharged && u.State.MaxCharges - u.State.Charges > 1))
            {
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    NeverUseByDefault,
                    GetExtraReloadCharge,
                    descriptionLong: "Spend 1 charge and gain 1 strain to reload an extra charge?"
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void GetExtraReloadCharge(object sender, EventArgs e)
        {
            HostShip.OnGetReloadChargesCount += AddExtraCharge;
            HostUpgrade.State.SpendCharge();
            HostShip.Tokens.AssignToken(new StrainToken(HostShip), DecisionSubPhase.ConfirmDecision);
        }

        private void AddExtraCharge(GenericUpgrade upgrade, ref int count)
        {
            count++;
            HostShip.OnGetReloadChargesCount -= AddExtraCharge;
        }
    }
}