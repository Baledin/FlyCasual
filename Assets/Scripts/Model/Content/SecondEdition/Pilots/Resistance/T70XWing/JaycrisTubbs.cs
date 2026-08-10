using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.T70XWing
{
    public class JaycrisTubbs : T70XWing
    {
        public JaycrisTubbs() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Jaycris Tubbs",
                "Loving Father",
                Faction.Resistance,
                1,
                4,
                8,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.JaycrisTubbsAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Tech,
                    UpgradeType.Astromech,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.XWing
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class JaycrisTubbsXWA : JaycrisTubbs
    {
        public JaycrisTubbsXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 10;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 7;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Astromech,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JaycrisTubbsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += JaycrisTubbsPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= JaycrisTubbsPilotAbility;
        }

        protected void JaycrisTubbsPilotAbility(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;
            if (ship.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Easy)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, SelectTargetForJaycrisTubbsAbility);
            }
        }

        private void SelectTargetForJaycrisTubbsAbility(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                RemoveStressToken,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Choose a friendly ship, it removes a stress token",
                HostShip
            );
        }

        private void RemoveStressToken()
        {
            TargetShip.Tokens.RemoveToken(typeof(StressToken), SelectShipSubPhase.FinishSelection);
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.This, TargetTypes.OtherFriendly) && FilterTargetsByRange(ship, 0, 1) && ship.Tokens.HasToken(typeof(StressToken));
        }

        private int GetAiPriority(GenericShip ship)
        {
            int priority = 0;

            if (ship.Tokens.HasToken(typeof(StressToken))) priority += 100;

            priority += ship.PilotInfo.Cost;

            return priority;
        }
    }
}
