using Abilities.SecondEdition;
using BoardTools;
using Bombs;
using Movement;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class TopCover : GenericUpgrade
    {
        public TopCover()
        {
            UpgradeInfo = new UpgradeCardInfo(
                name: "Top Cover",
                type: UpgradeType.Talent,
                cost: 0,
                abilityType: typeof(TopCoverAbility)
            );

            IsHidden = true;

            ImageUrl = "Pilots/captainjonus-swz98";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TopCoverAbility : GenericAbility
    {
        private bool deviceWasDroppedOrLaunched = false;

        public override void ActivateAbility()
        {
            GenericShip.OnAttackFinishGlobal += CheckAbility;
            HostShip.OnBombWasDropped += SetBombFlag;
            HostShip.OnBombWasLaunched += SetBombFlag;
            HostShip.OnRoundEnd += ResetBombFlag;

        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackFinishGlobal -= CheckAbility;
            HostShip.OnBombWasDropped -= SetBombFlag;
            HostShip.OnBombWasLaunched -= SetBombFlag;
            HostShip.OnRoundEnd -= ResetBombFlag;
        }

        private void SetBombFlag()
        {
            deviceWasDroppedOrLaunched = true;
        }

        private void ResetBombFlag(GenericShip ship)
        {
            deviceWasDroppedOrLaunched = false;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (Tools.IsFriendly(ship, HostShip)
                && Board.IsShipBetweenRange(HostShip, ship, 0, 3)
                && (Combat.Defender != null)
               && !deviceWasDroppedOrLaunched
               )
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility
            (
                descriptionShort: "Top Cover",
                descriptionLong: "Do you want to launch a bomb using the [3] straight or bank template?",
                useByDefault: NeverUseByDefault,
                useAbility: DropBomb,
                imageHolder: HostUpgrade
            );
        }

        private void DropBomb(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.OnGetAvailableBombLaunchTemplates += AddLaunchTemplates;

            BombsManager.RegisterBombDropTriggerIfAvailable(
                HostShip,
                TriggerTypes.OnAbilityDirect,
                subType: UpgradeSubType.Bomb,
                isRealDrop: false
            );

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Triggers.FinishTrigger);
        }

        protected virtual void AddLaunchTemplates(List<ManeuverTemplate> availableTemplates, GenericUpgrade upgrade)
        {
            if (upgrade.UpgradeInfo.SubType != UpgradeSubType.Bomb) return;
            availableTemplates.Clear();
            availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed3));
            availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, ManeuverSpeed.Speed3));
            availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, ManeuverSpeed.Speed3));
        }

    }
}
