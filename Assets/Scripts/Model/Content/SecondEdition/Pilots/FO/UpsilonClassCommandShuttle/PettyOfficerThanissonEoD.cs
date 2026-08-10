using Abilities.SecondEdition;
using Arcs;
using Content;
using MainPhases;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Tokens;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.UpsilonClassCommandShuttle
{
    public class PettyOfficerThanissonEoD : UpsilonClassCommandShuttle
    {
        public PettyOfficerThanissonEoD() : base()
        {
            PilotInfo = new PilotCardInfo25(
                pilotName: "Petty Officer Thanisson",
                pilotTitle: "Evacuation of D'Qar",
                faction: Faction.FirstOrder,
                initiative: 1,
                cost: 16,
                loadoutValue: 0,
                isLimited: true,
                charges: 1,
                regensCharges: 1,
                isStandardLayout: true,
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Crew,
                    UpgradeType.Sensor,
                    UpgradeType.Tech
                },
                abilityType: typeof(PettyOfficerThanissonEoDAbility),
                legality: new() { Legality.XWA }
            );

            PilotNameCanonical = "pettyofficerthanisson-evacuationofdqar";

            MustHaveUpgrades.Add(typeof(GeneralHuxEoD));
            MustHaveUpgrades.Add(typeof(AdvancedSensors));
            MustHaveUpgrades.Add(typeof(InterferenceArray));
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PettyOfficerThanissonEoDAbility : GenericAbility
    {
        private GenericShip targetShip;

        public override void ActivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, GenericToken token)
        {
            if (Phases.CurrentPhase is not ActivationPhase && Phases.CurrentPhase is not CombatPhase) return;

            if (token is not DepleteToken && token is not StrainToken) return;

            if (HostShip.State.Charges == 0) return;

            if (HostShip != ship
                && HostShip.SectorsInfo.IsShipInSector(ship, ArcType.Front)
                && HostShip.SectorsInfo.RangeToShipBySector(ship, ArcType.Front) <= 2)
            {
                targetShip = ship;

                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, AskAssignStress);
            }
        }

        private void AskAssignStress(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                ShouldUseAbility,
                AssignStress,
                descriptionLong: $"Do you want to assign a Stress token to {targetShip.PilotInfo.PilotName}?",
                imageHolder: HostShip
            );
        }

        private bool ShouldUseAbility()
        {
            return Tools.IsAnotherTeam(HostShip, targetShip);
        }

        private void AssignStress(object sender, System.EventArgs e)
        {
            HostShip.SpendCharge();

            targetShip.Tokens.AssignToken(new StressToken(targetShip), DecisionSubPhase.ConfirmDecision);
        }
    }
}