using BoardTools;
using Bombs;
using Content;
using Movement;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Drk1ProbeDroids : GenericUpgrade
    {
        public Drk1ProbeDroids() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "DRK-1 Probe Droids",
                UpgradeType.Device,
                subType: UpgradeSubType.Remote,
                charges: 2,
                cannotBeRecharged: true,
                cost: 5,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Separatists),
                abilityType: typeof(Abilities.SecondEdition.Drk1ProbeDroidsAbility),
                remoteType: typeof(Remote.Drk1ProbeDroid),
                legalityInfo: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }

        public override List<ManeuverTemplate> GetDefaultDropTemplates()
        {
            return new List<ManeuverTemplate>()
            {
                new ManeuverTemplate(ManeuverBearing.Turn, ManeuverDirection.Right, ManeuverSpeed.Speed3, isBombTemplate: true),
                new ManeuverTemplate(ManeuverBearing.Turn, ManeuverDirection.Left, ManeuverSpeed.Speed3, isBombTemplate: true),
                new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, ManeuverSpeed.Speed3, isBombTemplate: true),
                new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, ManeuverSpeed.Speed3, isBombTemplate: true),
                new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed3, isBombTemplate: true)
            };
        }

        public override List<ManeuverTemplate> GetDefaultLaunchTemplates()
        {
            return new List<ManeuverTemplate>()
            {
                new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed3),
                new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, ManeuverSpeed.Speed3),
                new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, ManeuverSpeed.Speed3),
                new ManeuverTemplate(ManeuverBearing.Turn, ManeuverDirection.Left, ManeuverSpeed.Speed3),
                new ManeuverTemplate(ManeuverBearing.Turn, ManeuverDirection.Right, ManeuverSpeed.Speed3)
            };

        }
    }

    public class Drk1ProbeDroidsXWA : Drk1ProbeDroids
    {
        public Drk1ProbeDroidsXWA() : base()
        {
            UpgradeInfo.Cost = 2;
            UpgradeInfo.LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Drk1ProbeDroidsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_NoTriggers += CheckAbility;
            HostShip.OnRemoteWasDroppedUpgrade += SpendCharge;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_NoTriggers -= CheckAbility;
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterOwnAbilityTrigger;
            HostShip.OnRemoteWasDroppedUpgrade -= SpendCharge;
        }

        private void CheckAbility()
        {
            if (HostUpgrade.State.Charges > 0)
            {
                Phases.Events.OnEndPhaseStart_Triggers += RegisterOwnAbilityTrigger;
            }
        }

        private void RegisterOwnAbilityTrigger()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterOwnAbilityTrigger;

            RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, DeployRemote);
        }

        private void DeployRemote(object sender, EventArgs e)
        {
            BombsManager.RegisterBombDropTriggerIfAvailable(
                HostShip,
                TriggerTypes.OnAbilityDirect,
                subType: UpgradeSubType.Remote,
                type: HostUpgrade.UpgradeInfo.RemoteType
            );

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Triggers.FinishTrigger);
        }

        private void SpendCharge(GenericUpgrade upgrade)
        {
            if (upgrade == HostUpgrade)
                upgrade.State.SpendCharge();
        }
    }
}