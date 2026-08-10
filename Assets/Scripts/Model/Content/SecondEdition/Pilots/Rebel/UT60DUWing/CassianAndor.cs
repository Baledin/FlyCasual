using Abilities.SecondEdition;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.UT60DUWing
{
    public class CassianAndor : UT60DUWing
    {
        public CassianAndor() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Cassian Andor",
                "Raised by the Rebellion",
                Faction.Rebel,
                3,
                5,
                12,
                isLimited: true,
                abilityType: typeof(CassianAndorAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Crew,
                    UpgradeType.Sensor,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                seImageNumber: 56,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class CassianAndorXWA : CassianAndor
    {
        public CassianAndorXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 12;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 14;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Talent,
                UpgradeType.Crew,
                UpgradeType.Crew,
                UpgradeType.Sensor,
                UpgradeType.Modification,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CassianAndorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnActivationPhaseStart += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnActivationPhaseStart -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnActivationPhaseStart, Ability);
        }

        protected virtual string GenerateAbilityMessage()
        {
            return "Choose a ship to remove a stress token from.";
        }

        private void Ability(object sender, EventArgs e)
        {
            bool friendlyShipIsStressed = false;

            foreach (var kv in HostShip.Owner.Ships)
            {
                GenericShip ship = kv.Value;
                if (ship.Tokens.HasToken(typeof(StressToken)))
                {
                    friendlyShipIsStressed = true;
                }
            }

            if (friendlyShipIsStressed)
            {
                SelectTargetForAbility(
                    SelectAbilityTarget,
                    FilterAbilityTarget,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotInfo.PilotName,
                    GenerateAbilityMessage(),
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        protected virtual bool FilterAbilityTarget(GenericShip ship)
        {
            return
                FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly, TargetTypes.This }) &&
                FilterTargetsByRange(ship, 1, 3) &&
                ship.Tokens.HasToken(typeof(StressToken));
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;
            if (ActionsHolder.HasTarget(ship)) result += 100;
            return result;
        }

        private void SelectAbilityTarget()
        {
            TargetShip.Tokens.RemoveToken(typeof(StressToken), SelectShipSubPhase.FinishSelection);
        }
    }
}