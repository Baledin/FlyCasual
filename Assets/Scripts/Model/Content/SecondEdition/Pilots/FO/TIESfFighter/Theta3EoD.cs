using Abilities.SecondEdition;
using Content;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.TIESfFighter
{
    public class Theta3EoD : TIESfFighter
    {
        public Theta3EoD() : base()
        {
            PilotInfo = new PilotCardInfo25(
                pilotName: "Theta 3",
                pilotTitle: "Evacuation of D'Qar",
                faction: Faction.FirstOrder,
                initiative: 3,
                cost: 10,
                loadoutValue: 0,
                isLimited: true,
                limited: 1,
                charges: 2,
                abilityType: typeof(Theta3EoDAbility),
                extraUpgradeIcons: new()
                {
                    UpgradeType.Sensor,
                    UpgradeType.Missile,
                    UpgradeType.Gunner
                },
                tags: new()
                {
                    Tags.Tie
                },
                isStandardLayout: true,
                legality: new List<Legality>() { Legality.XWA }
            );

            PilotNameCanonical = "theta3-evacuationofdqar";

            MustHaveUpgrades.Add(typeof(PassiveSensors));
            MustHaveUpgrades.Add(typeof(SeekerMissiles));
            MustHaveUpgrades.Add(typeof(FirstOrderOrdnanceTechEoD));

            ShipAbilities.Add(new HeavyWeaponTurretEoD());

            ShipInfo.ActionIcons.LinkedActions.Clear();
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Theta3EoDAbility : GenericAbility
    {
        // At the start of the Engagement Phase, if you have no green tokens, you may spend 1 charge to gain an evade token.

        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbility;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskGainEvadeToken);
        }

        private void AskGainEvadeToken(object sender, System.EventArgs e)
        {
            if (!HostShip.Tokens.HasGreenTokens && HostShip.State.Charges > 0)
            {
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AiCanUse,
                    GainEvadeToken,
                    descriptionLong: "Spend 1 charge to gain an evade token?"
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void GainEvadeToken(object sender, System.EventArgs e)
        {
            HostShip.SpendCharge();
            HostShip.Tokens.AssignToken(new EvadeToken(HostShip), DecisionSubPhase.ConfirmDecision);
        }

        private bool AiCanUse()
        {
            return HostShip.Owner.AnotherPlayer.Ships.Values.Any(s => s.GetAllWeapons().Any(w => w.IsShotAvailable(HostShip))); // Does any enemy ship have a shot on me?
        }
    }
}