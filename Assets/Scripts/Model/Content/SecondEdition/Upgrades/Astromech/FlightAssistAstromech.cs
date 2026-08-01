using Abilities.SecondEdition;
using Actions;
using ActionsList;
using BoardTools;
using Content;
using Movement;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class FlightAssistAstromech : GenericUpgrade
    {
        public FlightAssistAstromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Flight-Assist Astromech",
                UpgradeType.Astromech,
                charges: 2,
                cost: 3,
                restriction: new BaseSizeRestriction(BaseSize.Small),
                abilityType: typeof(FlightAssistAstromechAbility),
                legalityInfo: new List<Legality> { Legality.XWA }
            );

            NameCanonical = "flightassistastromech-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    // While you perform a white barrel roll or white boost action, you may spend 1 charge.
    // If you do, treat the action as red, and use a speed 2 template while performing the action.
    public class FlightAssistAstromechAbility : GenericAbility
    {
        GenericAction savedAction;

        public override void ActivateAbility()
        {
            HostShip.BeforeActionIsPerformed += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeActionIsPerformed -= RegisterAbility;
        }

        private void RegisterAbility(GenericAction action, ref bool data)
        {
            if (HostUpgrade.State.Charges > 0 && action.Color == ActionColor.White && (action is BarrelRollAction || action is BoostAction))
            {
                savedAction = action;

                RegisterAbilityTrigger(TriggerTypes.BeforeActionIsPerformed, AskUseFlightAssistAstromech);
            }
        }

        private void AskUseFlightAssistAstromech(object sender, EventArgs e)
        {
            Tooltips.EndTooltip();

            AskToUseAbility(HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                UseFlightAssistAstromech,
                descriptionLong: $"Do you want to spend 1 charge to treat this action as red and use the speed 2 template?",
                callback: Triggers.FinishTrigger
            );
        }

        private void UseFlightAssistAstromech(object sender, EventArgs e)
        {
            if (HostUpgrade.State.Charges < 1)
            {
                Messages.ShowError("Not enough charges to use Flight-Assist Astromech");
            }
            else
            {
                savedAction.Color = ActionColor.Red;

                HostShip.OnActionIsSkipped += Cleanup;
                HostShip.OnActionIsPerformed += SpendCharge;

                HostShip.OnGetAvailableBarrelRollActionTemplates += ReplaceBarrelRollTemplates;
                HostShip.OnGetAvailableBoostTemplates += ReplaceBoostTemplates;
            }

            DecisionSubPhase.ConfirmDecision();
        }

        private void ReplaceBarrelRollTemplates(List<ManeuverTemplate> availableTemplates, GenericAction action)
        {
            List<ManeuverTemplate> newTemplates = new();

            foreach (ManeuverTemplate template in availableTemplates)
            {
                newTemplates.Add(new(
                    template.Bearing,
                    template.Direction,
                    ManeuverSpeed.Speed2,
                    false,
                    template.IsSideTemplate
                ));
            }

            availableTemplates.RemoveAll(n => n.Speed != ManeuverSpeed.Speed2);
            availableTemplates.AddRange(newTemplates);
        }

        private void ReplaceBoostTemplates(List<BoostMove> availableTemplates, GenericAction action)
        {
            List<BoostMove> newTemplates = new();

            foreach (BoostMove template in availableTemplates)
            {
                BoostMove newMove = new(
                        BoostMove.GetBoostTemplateFromName(template.Name.Replace('1', '2')),
                        template.IsRed,
                        template.IsPurple,
                        template.IsForced);

                newTemplates.Add(newMove);
            }

            availableTemplates.Clear();
            availableTemplates.AddRange(newTemplates);
        }

        private void SpendCharge(GenericAction action)
        {
            HostUpgrade.State.SpendCharge();

            Cleanup(HostShip);
        }

        private void Cleanup(GenericShip ship)
        {
            HostShip.OnActionIsSkipped -= Cleanup;
            HostShip.OnActionIsPerformed -= SpendCharge;

            HostShip.OnGetAvailableBarrelRollActionTemplates -= ReplaceBarrelRollTemplates;
            HostShip.OnGetAvailableBoostTemplates -= ReplaceBoostTemplates;

            savedAction.Color = ActionColor.White; // Reset color or it will persist
            savedAction = null;
        }
    }
}
