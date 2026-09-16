using Abilities.SecondEdition;
using Actions;
using ActionsList;
using BoardTools;
using Content;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.Mg100StarFortress
{
    public class FinchDallowEoD : Mg100StarFortress
    {
        public FinchDallowEoD() : base()
        {
            PilotInfo = new PilotCardInfo25
                (
                    pilotName: "Finch Dallow",
                    pilotTitle: "Evacuation of D'Qar",
                    faction: Faction.Resistance,
                    initiative: 4,
                    cost: 16,
                    loadoutValue: 0,
                    isLimited: true,
                    abilityType: typeof(FinchDallowEoDAbility),
                    extraUpgradeIcons: new()
                    {
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Device
                    },
                    isStandardLayout: true,
                    legality: new() { Legality.XWA }
                );

            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(ReloadAction), typeof(ReinforceAction)));

            ShipAbilities.Add(new ModularBombingMagazine());

            MustHaveUpgrades.Add(typeof(NixJerd));
            MustHaveUpgrades.Add(typeof(PaigeTicoEoD));
            MustHaveUpgrades.Add(typeof(ProtonBombs));
            MustHaveUpgrades.Add(typeof(ThermalDetonatorsEoD));

            ModelInfo.SkinName = "Cobalt";
            PilotNameCanonical = "finchdallow-evacuationofdqar";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class FinchDallowEoDAbility : GenericAbility
    {
        // If you would drop a device using a straight template and you are not stressed,
        // you may gain 1 stress token to use a left turn or right turn template of the same speed instead
        public override void ActivateAbility()
        {
            HostShip.OnBombWillBeDropped += CheckPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnBombWillBeDropped -= CheckPilotAbility;
        }

        private void CheckPilotAbility()
        {
            if (HostShip.IsStressed) return;

            RegisterAbilityTrigger(TriggerTypes.OnBombWillBeDropped, AskUseTurnTemplates);
        }

        private void AskUseTurnTemplates(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                NeverUseByDefault,
                UseTurnTemplates,
                dontUseAbility: CleanUp,
                requiredPlayer: HostShip.Owner.PlayerNo,
                descriptionLong: "You may gain 1 stress token to use a left bank or right bank template of the same speed instead as straight."
            );
        }

        private void UseTurnTemplates(object sender, EventArgs e)
        {
            HostShip.OnGetAvailableBombDropTemplatesOneCondition += GetDropTemplates;
            HostShip.Tokens.AssignToken(new StressToken(HostShip), DecisionSubPhase.ConfirmDecision);
        }

        private void GetDropTemplates(List<ManeuverTemplate> availableTemplates, GenericUpgrade upgrade)
        {
            HostShip.OnGetAvailableBombDropTemplatesOneCondition -= GetDropTemplates;

            List<ManeuverTemplate> newTemplates = new();

            foreach (ManeuverTemplate template in availableTemplates)
            {
                if (template.Bearing == Movement.ManeuverBearing.Straight)
                {
                    newTemplates.Add(new ManeuverTemplate(Movement.ManeuverBearing.Bank, Movement.ManeuverDirection.Left, template.Speed, true));
                    newTemplates.Add(new ManeuverTemplate(Movement.ManeuverBearing.Bank, Movement.ManeuverDirection.Right, template.Speed, true));
                }
            }

            availableTemplates.AddRange(newTemplates);
        }

        private void CleanUp(object sender, EventArgs e)
        {
            Triggers.FinishTrigger();
        }
    }
}