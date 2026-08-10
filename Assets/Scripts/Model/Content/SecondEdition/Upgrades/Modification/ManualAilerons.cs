using BoardTools;
using Movement;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ManualAilerons : GenericUpgrade
    {
        public ManualAilerons()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Manual Ailerons",
                UpgradeType.Modification,
                abilityType: typeof(Abilities.SecondEdition.ManualAileronsAbility),
                charges: 2
            );

            IsHidden = true;
        }
    }
}

namespace Abilities.SecondEdition
{
    // When you decloack, you may spend 1 charge to use the 2 bank template instead of the straight 2 template

    public class ManualAileronsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnBeforeDecloak += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnBeforeDecloak -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnBeforeDecloak, ActivateAbility);
        }

        private void ActivateAbility(object sender, EventArgs e)
        {
            if (HostUpgrade.State.Charges > 0)
            {
                AskToUseAbility(HostUpgrade.UpgradeInfo.Name,
                    NeverUseByDefault,
                    descriptionLong: "You may spend 1 charge to use a 2-speed bank template instead of the 2-speed straight template.",
                    useAbility: UseManualAilerons
                    , callback: Triggers.FinishTrigger
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseManualAilerons(object sender, EventArgs e)
        {
            HostShip.OnGetAvailableDecloakBarrelRollTemplates += GetBarrelRollDecloackTemplates;
            HostShip.OnGetAvailableDecloakBoostTemplates += GetBoostDecloakTemplates;
            HostShip.OnDecloak += SpendCharge;

            DecisionSubPhase.ConfirmDecision();
        }

        private void GetBarrelRollDecloackTemplates(List<ManeuverTemplate> availableTemplates)
        {
            HostShip.OnGetAvailableDecloakBarrelRollTemplates -= GetBarrelRollDecloackTemplates;
            GetDecloakTemplates(availableTemplates);
        }

        private void GetBoostDecloakTemplates(List<ManeuverTemplate> availableTemplates)
        {
            HostShip.OnGetAvailableDecloakBoostTemplates -= GetBoostDecloakTemplates;
            GetDecloakTemplates(availableTemplates);
        }

        private void GetDecloakTemplates(List<ManeuverTemplate> availableTemplates)
        {
            if (availableTemplates.Any(n => n.Name == "Straight 2"))
            {
                availableTemplates.RemoveAll(n => n.Name == "Straight 2");
                availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, ManeuverSpeed.Speed2));
                availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, ManeuverSpeed.Speed2));
            }
        }

        private void SpendCharge()
        {
            HostUpgrade.State.SpendCharge();
            HostShip.OnDecloak -= SpendCharge;
        }
    }
}