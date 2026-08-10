using Abilities.SecondEdition;
using Content;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ThreatSensors : GenericUpgrade
    {
        public ThreatSensors() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Threat Sensors",
                UpgradeType.Tech,
                cost: 0,
                isLimited: true,
                abilityType: typeof(ThreatSensorsAbility),
                legalityInfo: new() { Legality.XWA }
            );

            IsHidden = true;

            ImageUrl = "https://infinitearenas.com/xw2xwa/images/pilots/scorch-evacuationofdqar.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ThreatSensorsAbility : GenericAbility
    {
        // While you defend, if you are not stressed, you may reroll 1 focus result.

        public override void ActivateAbility()
        {
            AddDiceModification(
                name: HostUpgrade.UpgradeInfo.Name,
                isAvailable: ModificationIsAvailable,
                aiPriority: GetAiPriority,
                modificationType: DiceModificationType.Reroll,
                count: 1,
                sidesCanBeSelected: new() { DieSide.Focus }
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private int GetAiPriority()
        {
            int priority = 100;

            if (HostShip.Tokens.HasGreenTokens)
            {
                priority -= 50;
            }

            return priority;
        }

        private bool ModificationIsAvailable()
        {
            return Combat.Defender == HostShip
                && !HostShip.IsStressed
                && Combat.DiceRollDefence.FocusesNotRerolled > 0;
        }
    }
}