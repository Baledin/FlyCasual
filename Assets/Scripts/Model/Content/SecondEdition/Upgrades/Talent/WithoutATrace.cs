using Abilities.SecondEdition;
using RulesList;
using Ship;
using SubPhases;
using System;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class WithoutATrace : GenericUpgrade
    {
        public WithoutATrace()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Without a Trace",
                UpgradeType.Talent,
                abilityType: typeof(WithoutATraceAbility)
            );

            IsHidden = true;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WithoutATraceAbility : GenericAbility
    {
        // After you gain a cloak token, you may remove 1 red token.
        // While you are cloaked, enemy ships cannot acquire locks on you.

        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += RegisterAbility;

            TargetLocksRule.OnCheckTargetLockIsDisallowed += ForbidLockOnMe;

        }
        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= RegisterAbility;

            TargetLocksRule.OnCheckTargetLockIsDisallowed -= ForbidLockOnMe;

        }

        private void ForbidLockOnMe(ref bool isAllowed, GenericShip lockSource, ITargetLockable lockTarget)
        {
            if (lockTarget is GenericShip
                && (lockTarget as GenericShip) == HostShip
                && HostShip.IsCloaked)
            {
                isAllowed = false;
            }
        }

        private void RegisterAbility(GenericShip ship, GenericToken token)
        {
            if (token is CloakToken && HostShip.Tokens.HasTokenByColor(TokenColors.Red))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, UseAbility);
            }
        }

        private void UseAbility(object sender, EventArgs e)
        {
            WithoutATraceRemoveRedTokenAbilityDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<WithoutATraceRemoveRedTokenAbilityDecisionSubPhase>(
                $"{HostUpgrade.UpgradeInfo.Name}: After cloaking you may remove 1 red token",
                Triggers.FinishTrigger
            );
            subphase.ImageSource = HostShip;
            subphase.AbilityHostShip = HostShip;
            subphase.ShowSkipButton = false;
            subphase.Start();
        }
    }
}

namespace SubPhases
{
    public class WithoutATraceRemoveRedTokenAbilityDecisionSubPhase : RemoveRedTokenDecisionSubPhase
    {
        public GenericShip AbilityHostShip;

        public override void PrepareCustomDecisions()
        {
            DescriptionShort = "Without A Trace";
            DescriptionLong = "You may remove 1 red token";

            DecisionOwner = Selection.ThisShip.Owner;
            DefaultDecisionName = decisions.First().Name;
        }
    }
}