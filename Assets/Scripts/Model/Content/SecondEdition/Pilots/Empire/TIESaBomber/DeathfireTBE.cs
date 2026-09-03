using BoardTools;
using Bombs;
using Content;
using Movement;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.TIESaBomber
    {
        public class DeathfireTBE : TIESaBomber
        {
            public DeathfireTBE()
            {
                PilotInfo = new PilotCardInfo25(
                    "\"Deathfire\"",
                    "Obstinate Bombardier",
                    Faction.Imperial,
                    2,
                    3,
                    loadoutValue: 0,
                    isStandardLayout: true,
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    charges: 2,
                    regensCharges: 1,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DeathfireTBEAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Device,
                        UpgradeType.Device
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal },
                    abilityText: "After you fully execute a speed 3-5 maneuver, if you have not dropped or launched a device this round, you may spend 2 charges to drop or launch a bomb using the 3-speed forward template."
                );
                PilotNameCanonical = "deathfire-swz98";

                MustHaveUpgrades.Add(typeof(ProtonBombs));
                MustHaveUpgrades.Add(typeof(ConnerNets));
                MustHaveUpgrades.Add(typeof(SwiftApproach));

                ImageUrl = "https://infinitearenas.com/xw2/images/quickbuilds/deathfire-swz98.png";
            }
        }

        public class DeathfireTBEXWA : DeathfireTBE
        {
            public DeathfireTBEXWA() : base()
            {
                var pilot = (PilotCardInfo25)PilotInfo;
                pilot.Cost = 10;
                pilot.LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a speed 3-5 maneuver, if you have not dropped or launched a device this round,
    //you may spend 2 charges to drop or launch a bomb using the 3 forward template.
    public class DeathfireTBEAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (HostShip.AssignedManeuver.Speed is >= 3 and <= 5
                && HostShip.State.Charges > 1
                && !HostShip.IsBombAlreadyDropped)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility
            (
                descriptionShort: HostShip.PilotInfo.PilotName,
                descriptionLong: "Do you want to spend 2 charges to drop or launch a bomb using the [3] template?",
                useByDefault: NeverUseByDefault,
                useAbility: DropBomb,
                imageHolder: HostUpgrade
            );
        }

        private void DropBomb(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.SpendCharges(2);

            HostShip.OnGetAvailableBombDropTemplatesNoConditions += AddDropTemplate;
            HostShip.OnGetAvailableBombLaunchTemplates += AddLaunchTemplate;

            BombsManager.RegisterBombDropTriggerIfAvailable(
                HostShip,
                TriggerTypes.OnAbilityDirect,
                subType: UpgradeSubType.Bomb,
                isRealDrop: false
            );

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Triggers.FinishTrigger);
        }

        private void AddDropTemplate(List<ManeuverTemplate> availableTemplates, GenericUpgrade upgrade)
        {
            if (upgrade.UpgradeInfo.SubType != UpgradeSubType.Bomb) return;
            availableTemplates.Clear();
            availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed3, isBombTemplate: true));

            ResetDelegates();
        }

        protected virtual void AddLaunchTemplate(List<ManeuverTemplate> availableTemplates, GenericUpgrade upgrade)
        {
            if (upgrade.UpgradeInfo.SubType != UpgradeSubType.Bomb) return;
            availableTemplates.Clear();
            availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed3));

            ResetDelegates();
        }

        private void ResetDelegates()
        {
            HostShip.OnGetAvailableBombDropTemplatesNoConditions -= AddDropTemplate;
            HostShip.OnGetAvailableBombLaunchTemplates -= AddLaunchTemplate;
        }
    }
}