using ActionsList;
using BoardTools;
using Content;
using Movement;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class RepulsorliftEngines : GenericUpgrade
    {
        public RepulsorliftEngines() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Repulsorlift Engines",
                UpgradeType.Modification,
                abilityType: typeof(Abilities.SecondEdition.RepulsorliftEnginesAbility),
                legalityInfo: new() { Legality.XWA }
            );

            IsHidden = true;
            ImageUrl = "https://infinitearenas.com/xw2xwa/images/pilots/caithrenalli-evacuationofdqar.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    // While you perform a Barrel Roll action, you may gain 1 strain token to use the bank-left or bank-right template instead of the straight template.
    public class RepulsorliftEnginesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBarrelRollActionTemplates += AddBarrelRollTemplates;
            HostShip.OnActionIsPerformed += CheckForGainStrain;
            HostShip.OnActionIsReallyFailed += CheckForGainStrain;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBarrelRollActionTemplates -= AddBarrelRollTemplates;
            HostShip.OnActionIsPerformed -= CheckForGainStrain;
            HostShip.OnActionIsReallyFailed -= CheckForGainStrain;
        }

        private void AddBarrelRollTemplates(List<ManeuverTemplate> availableTemplates, GenericAction action)
        {
            availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, ManeuverSpeed.Speed1));
            availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, ManeuverSpeed.Speed1));
        }

        private void CheckForGainStrain(GenericAction action)
        {
            if (action is BarrelRollAction)
            {
                if ((action as BarrelRollAction).SelectedTemplate.Name == "Bank 1 Left" ||
                    (action as BarrelRollAction).SelectedTemplate.Name == "Bank 1 Right")
                {
                    RegisterAbilityTrigger(TriggerTypes.OnFreeAction, GainStrain);
                }
            }
        }

        private void GainStrain(object sender, EventArgs e)
        {
            HostShip.Tokens.AssignToken(new StrainToken(HostShip),Triggers.FinishTrigger);
        }
    }
}