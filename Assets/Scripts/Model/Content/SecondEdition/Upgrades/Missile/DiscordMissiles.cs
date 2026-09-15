using BoardTools;
using Bombs;
using Content;
using Movement;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class DiscordMissiles : GenericUpgrade, IDroppable
    {
        public DiscordMissiles() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Discord Missiles",
                UpgradeType.Missile,
                subType: UpgradeSubType.Remote,
                cost: 2,
                limited: 3,
                charges: 1,
                cannotBeRecharged: true,
                restriction: new FactionRestriction(Faction.Separatists),
                abilityType: typeof(Abilities.SecondEdition.DiscordMissilesAbility),
                remoteType: typeof(Remote.BuzzDroidSwarm),
                legalityInfo: new() { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }

        public List<ManeuverTemplate> GetDefaultDropTemplates()
        {
            return new List<ManeuverTemplate>();
        }

        public List<ManeuverTemplate> GetDefaultLaunchTemplates()
        {
            return new List<ManeuverTemplate>()
            {
                new (ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed3),
                new (ManeuverBearing.Bank, ManeuverDirection.Left, ManeuverSpeed.Speed3),
                new (ManeuverBearing.Bank, ManeuverDirection.Right, ManeuverSpeed.Speed3)
            };
        }
    }

    public class DiscordMissilesXWA : DiscordMissiles
    {
        public DiscordMissilesXWA() : base()
        {
            UpgradeInfo.Cost = 2;
            UpgradeInfo.LegalityInfo = new() { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DiscordMissilesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_NoTriggers += CheckAbility;
            HostShip.OnRemoteWasDroppedUpgrade += SpendCosts;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_NoTriggers -= CheckAbility;
            HostShip.OnRemoteWasDroppedUpgrade -= SpendCosts;
        }

        private void CheckAbility()
        {
            if (HostUpgrade.State.Charges > 0 && HostShip.Tokens.HasToken(typeof(CalculateToken)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, StartRemoteDeployment);
            }
        }

        private void StartRemoteDeployment(object sender, EventArgs e)
        {
            if (HostUpgrade.State.Charges > 0 && HostShip.Tokens.HasToken(typeof(CalculateToken)))
            {
                Selection.ChangeActiveShip(HostShip);

                BombsManager.RegisterBombDropTriggerIfAvailable(
                    HostShip,
                    TriggerTypes.OnAbilityDirect,
                    subType: UpgradeSubType.Remote,
                    type: HostUpgrade.UpgradeInfo.RemoteType
                );

                Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, FinishAbility);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void SpendCosts(GenericUpgrade upgrade)
        {
            if (upgrade == HostUpgrade)
                HostShip.Tokens.SpendToken(typeof(CalculateToken), upgrade.State.SpendCharge);
        }

        private void FinishAbility()
        {
            Selection.DeselectThisShip();
            Triggers.FinishTrigger();
        }
    }
}