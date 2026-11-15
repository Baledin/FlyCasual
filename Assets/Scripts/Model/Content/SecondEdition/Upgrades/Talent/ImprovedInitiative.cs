using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ImprovedInitiative : GenericUpgrade
    {
        public ImprovedInitiative() : base()
        {
            IsHidden = !DebugManager.FreeMode;

            UpgradeInfo = new UpgradeCardInfo(
                "Improved Initiative",
                UpgradeType.Talent,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.ImprovedInitiativeAbility)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ImprovedInitiativeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void ActivateAbilityForSquadBuilder()
        {
            HostShip.PilotInfo.Initiative++;
        }

        public override void DeactivateAbility()
        {

        }

        public override void DeactivateAbilityForSquadBuilder()
        {
            HostShip.PilotInfo.Initiative--;
        }
    }
}