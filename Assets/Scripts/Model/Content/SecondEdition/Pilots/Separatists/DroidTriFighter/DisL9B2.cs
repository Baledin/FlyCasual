using Abilities.SecondEdition;
using ActionsList;
using Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.DroidTriFighter
{
    public class DisL9B2 : DroidTriFighter
    {
        public DisL9B2() : base()
        {
            PilotInfo = new PilotCardInfo25(
                pilotName: "DIS-L9B2",
                pilotTitle: "Calculated Risk",
                faction: Faction.Separatists,
                initiative: 6,
                cost: 11,
                loadoutValue: 11,
                isLimited: true,
                charges: 1,
                regensCharges: 1,
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Modification,
                    UpgradeType.Modification,
                    UpgradeType.Cannon,
                    UpgradeType.Missile,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>()
                {
                    Tags.Droid
                },
                abilityType: typeof(DisL9B2Ability),
                legality: new List<Legality>() { Legality.XWA }
            );

            PilotNameCanonical = "disl9b2-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    // At the start of the Engagement Phase, if there is an enemy ship in your bullseye, you MUST spend 1 charge.
    // During the Engagement Phase, if your charge is active, treat your initiative value as 1.

    public class DisL9B2Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbility;
            Phases.Events.OnCombatPhaseEnd_Triggers += DeRegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbility;
            Phases.Events.OnCombatPhaseEnd_Triggers -= DeRegisterAbility;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, CheckChargeState);
        }

        private void DeRegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseEnd, ResetInitiative);
        }

        private void CheckChargeState(object sender, EventArgs e)
        {
            if (HostShip.State.Charges > 0)
            {
                if (HasShipInBullseye())
                {
                    HostShip.SpendCharge();
                }
                else
                {
                    Messages.ShowInfoToHuman($"{HostShip.PilotInfo.PilotName} engages at initiative 1.");
                    HostShip.State.CombatActivationAtInitiative = 1;
                }
            }

            Triggers.FinishTrigger();
        }

        private bool HasShipInBullseye()
        {
            return Roster.AllShips.Values.Any(s => HostShip.SectorsInfo.IsShipInSector(s, Arcs.ArcType.Bullseye) && !Tools.IsFriendly(HostShip,s));
        }

        private void ResetInitiative(object sender, EventArgs e)
        {
            Messages.ShowInfoToHuman($"{HostShip.PilotInfo.PilotName} initiative reset to {HostShip.PilotInfo.Initiative}.");
            HostShip.State.CombatActivationAtInitiative = HostShip.PilotInfo.Initiative;

            Triggers.FinishTrigger();
        }
    }
}
