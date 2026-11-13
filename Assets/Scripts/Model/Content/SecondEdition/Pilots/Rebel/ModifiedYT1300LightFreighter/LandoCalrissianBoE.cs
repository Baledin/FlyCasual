using Abilities.SecondEdition;
using Actions;
using ActionsList;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.ModifiedYT1300LightFreighter
{
    public class LandoCalrissianBoE : ModifiedYT1300LightFreighter
    {
        public LandoCalrissianBoE() : base()
        {
            PilotInfo = new PilotCardInfo25(
                pilotName: "Lando Calrissian",
                pilotTitle: "Battle Over Endor",
                faction: Faction.Rebel,
                initiative: 5,
                cost: 7,
                loadoutValue: 0,
                isLimited: true,
                abilityType: typeof(LandoCalrissianBattleOverEndorAbility),
                charges: 2,
                isStandardLayout: true,
                tags: new() {
                        Tags.Freighter,
                        Tags.YT1300
                },
                extraUpgradeIcons: new()
                {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Title
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            ShipAbilities.Add(new HighStakesAbility());

            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(EvadeAction)));
            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(CoordinateAction), ActionColor.Red));
            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(CoordinateAction), typeof(FocusAction)));

            MustHaveUpgrades.Add(typeof(AceInTheHole));
            MustHaveUpgrades.Add(typeof(ItsATrap));
            MustHaveUpgrades.Add(typeof(NienNunb));
            MustHaveUpgrades.Add(typeof(AirenCracken));
            MustHaveUpgrades.Add(typeof(MillenniumFalconBoE));

            PilotNameCanonical = "landocalrissian-battleoverendor";
        }
    }

    public class LandoCalrissianBoEXWA : LandoCalrissianBoE
    {
        public LandoCalrissianBoEXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 18;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new() { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    //At the start of the Activation Phase, you may spend 1 Charge. If you do, choose an initiative from 1 to 6. You activate at that initiative this phase.
    public class LandoCalrissianBattleOverEndorAbility : GenericAbility, IModifyPilotSkill
    {
        int initiative = 0;
        public override void ActivateAbility()
        {
            HostShip.OnActivationPhaseStart += RegisterTrigger;
        }
        public override void DeactivateAbility()
        {
            HostShip.OnActivationPhaseStart -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            if (HostShip.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActivationPhaseStart, AskToUseAbility);
            }
        }

        private void AskToUseAbility(object sender, EventArgs e)
        {
            if (HostShip.State.Charges > 0)
            {
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    NeverUseByDefault,
                    UseAbility,
                    descriptionLong: "Do you want to spend a charge to choose an initiative to activate at?",
                    imageHolder: HostShip
                );
            }
        }

        private void UseAbility(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            HostShip.State.Charges--;
            InitiativeSelectionsSubPhase selectionSubPhase = (InitiativeSelectionsSubPhase)Phases.StartTemporarySubPhaseNew(
                "Choose an initiative from 1 to 6. You activate at that initiative this phase.",
                typeof(InitiativeSelectionsSubPhase),
                Triggers.FinishTrigger
            );

            selectionSubPhase.DescriptionShort = "Lando";
            selectionSubPhase.DescriptionLong = String.Format("Choose an initiative from 1 to 6. You activate at that initiative this phase.");
            selectionSubPhase.ImageSource = HostUpgrade;

            for (int i = 1; i <= 6; i++)
            {
                int option = i;
                selectionSubPhase.AddDecision(option.ToString(),
                    delegate
                    {
                        this.initiative = option;
                        UpdateInitiative();
                    }
                );
            }

            selectionSubPhase.DefaultDecisionName = "6";
            selectionSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;
            selectionSubPhase.Start();
        }

        private void UpdateInitiative()
        {
            HostShip.State.AddPilotSkillModifier(this);
            Phases.Events.OnActivationPhaseEnd_NoTriggers += RemovePilotSkillModifieer;
            DecisionSubPhase.ConfirmDecision();
        }

        private void RemovePilotSkillModifieer()
        {
            Phases.Events.OnActivationPhaseEnd_NoTriggers -= RemovePilotSkillModifieer;
            HostShip.State.RemovePilotSkillModifier(this);
        }

        public void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill = initiative;
        }

        private class InitiativeSelectionsSubPhase : DecisionSubPhase { }
    }
}