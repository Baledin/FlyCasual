using Content;
using Movement;
using Ship;
using SubPhases;
using System;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class R0Astromech : GenericUpgrade
    {
        public R0Astromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R0 Astromech",
                UpgradeType.Astromech,
                abilityType: typeof(Abilities.SecondEdition.R0AstromechAbility),
                legalityInfo: new() { Legality.XWA }
            );

            IsHidden = true;
            ImageUrl = "https://infinitearenas.com/xw2xwa/images/pilots/jaycristubbs-evacuationofdqar.png";
        }
    }
}
namespace Abilities.SecondEdition
{
    // After you reveal a basic maneuver, you may reduce its difficulty.
    // If you do, after the Check Difficulty step, gain 1 strain token.
    public class R0AstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += CheckManeuverForAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= CheckManeuverForAbility;
        }

        private void CheckManeuverForAbility(GenericShip ship)
        {
            if (HostShip.AssignedManeuver.IsBasicManeuver && (HostShip.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Normal || HostShip.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Complex))
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, RegisterR0AstromechAbility);
            }
        }

        private void RegisterR0AstromechAbility(object sender, EventArgs e)
        {
            AskToUseAbility
            (
                HostUpgrade.UpgradeInfo.Name,
                DoesAiUse,
                UseR0AstromechAbility,
                descriptionLong: "Do you want to reduce the difficulty of your maneuver? If you do, gain 1 Strain token after the check difficulty step.",
                imageHolder: HostUpgrade
            );
        }

        private void UseR0AstromechAbility(object sender, EventArgs e)
        {
            HostShip.AssignedManeuver.ColorComplexity = GenericMovement.ReduceComplexity(HostShip.AssignedManeuver.ColorComplexity);
            HostShip.OnMovementFinish += GainStrain;
            DecisionSubPhase.ConfirmDecision();
        }

        private void GainStrain(GenericShip ship)
        {
            HostShip.OnMovementFinish -= GainStrain;
            HostShip.Tokens.AssignToken(new StrainToken(HostShip),()=>{});
        }

        private bool DoesAiUse()
        {
            return HostShip.Tokens.HasToken<StressToken>()
                || HostShip.Tokens.HasToken<StrainToken>()
                || HostShip.Tokens.HasToken<DepleteToken>(); // This could be improved to only swap deplete for strain when it expects to attack this turn.
        }
    }
}
