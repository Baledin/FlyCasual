using Abilities.SecondEdition;
using ActionsList;
using System;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class EfficientProcessing : GenericUpgrade
    {
        public EfficientProcessing() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                name: "Efficient Processing",
                type: UpgradeType.Talent,
                abilityType: typeof(EfficientProcessingAbility)
            );

            IsHidden = true;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EfficientProcessingAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAction;
        }

        private void CheckAction(GenericAction action)
        {
            if (action is CalculateAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AddCalculateToken);
            }
        }

        private void AddCalculateToken(object sender, EventArgs e)
        {
            Messages.ShowInfoToHuman($"{HostUpgrade.UpgradeInfo.Name}: {HostShip.PilotInfo.PilotName} gains an extra Calculate token.");
            HostShip.Tokens.AssignToken(new Tokens.CalculateToken(HostShip), Triggers.FinishTrigger);
        }
    }
}