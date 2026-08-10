using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.YT2400LightFreighter
{
    public class DashRendar : YT2400LightFreighter
    {
        public DashRendar() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Dash Rendar",
                "Freighter for Hire",
                Faction.Rebel,
                5,
                7,
                20,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.DashRendarAbility),
                tags: new List<Tags>
                {
                    Tags.Freighter
                },
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Crew,
                    UpgradeType.Illicit,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Missile,
                    UpgradeType.Title
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            ImageUrl = "https://infinitearenas.com/xw2/images/pilots/dashrendar-freighterforhire.png";
            PilotNameCanonical = "dashrendar-swz103-rebelalliance";
        }
    }

    public class DashRendarXWA : DashRendar
    {
        public DashRendarXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 18;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 24;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Talent,
                UpgradeType.Crew,
                UpgradeType.Gunner,
                UpgradeType.Illicit,
                UpgradeType.Illicit,
                UpgradeType.Modification,
                UpgradeType.Missile,
                UpgradeType.Title
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DashRendarAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCombatActivation += RegisterTriggerCombatPhase;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatActivation -= RegisterTriggerCombatPhase;
        }

        public void RegisterTriggerCombatPhase(GenericShip host)
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, UseAbilityCombatPhase);
        }

        private void UseAbilityCombatPhase(object sender, EventArgs e)
        {
            HostShip.IgnoreObstaclesList.AddRange(HostShip.ObstaclesLanded);
            HostShip.OnCanAttackWhileLandedOnObstacle += CanAttack;
            Phases.Events.OnCombatPhaseEnd_NoTriggers += TurnOffIgnoreObstaclesCombatPhase;
            Triggers.FinishTrigger();
        }

        private void TurnOffIgnoreObstaclesCombatPhase()
        {
            GenericShip.OnCanAttackWhileLandedOnObstacleGlobal -= CanAttack;
            HostShip.IgnoreObstaclesList.Clear();
            Phases.Events.OnCombatPhaseEnd_NoTriggers -= TurnOffIgnoreObstaclesCombatPhase;
        }

        private void CanAttack(GenericShip ship, ref bool canAttack)
        {
            canAttack = true;
        }
    }
}