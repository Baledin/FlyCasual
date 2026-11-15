using Conditions;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.DroidTriFighter
{
    public class FearsomePredator : DroidTriFighter
    {
        public FearsomePredator()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Fearsome Predator",
                "Fixated Pursuit",
                Faction.Separatists,
                3,
                3,
                3,
                limited: 3,
                abilityType: typeof(Abilities.SecondEdition.FearsomePredatorAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Missile,
                    UpgradeType.Modification,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class FearsomePredatorXWA : FearsomePredator
    {
        public FearsomePredatorXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 9;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 6;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Sensor,
                UpgradeType.Modification,
                UpgradeType.Modification,
                UpgradeType.Cannon,
                UpgradeType.Missile,
                UpgradeType.Configuration
            };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class FearsomePredatorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnSetupEnd += RegisterFearsomePredatorAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupEnd -= RegisterFearsomePredatorAbility;
        }

        private void RegisterFearsomePredatorAbility()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = HostShip.ShipId + ": Assign \"Fearful Prey\" condition",
                TriggerType = TriggerTypes.OnSetupEnd,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = SelectFearsomePredatorTarget,
            });
        }

        private void SelectFearsomePredatorTarget(object Sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                AssignFearfulPrey,
                CheckRequirements,
                GetAiFearfulPreyPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Assign the Fearful Prey condition to 1 enemy ship",
                HostShip
            );
        }

        protected virtual void AssignFearfulPrey()
        {
            foreach (GenericShip enemyShip in HostShip.Owner.EnemyShips.Values)
            {
                if (enemyShip.Tokens.HasToken(typeof(Conditions.FearfulPrey))) enemyShip.Tokens.RemoveCondition(typeof(Conditions.FearfulPrey));
            }

            TargetShip.Tokens.AssignCondition(new FearfulPrey(TargetShip));
            SelectShipSubPhase.FinishSelection();
        }

        private bool CheckRequirements(GenericShip ship)
        {
            return Tools.IsAnotherTeam(HostShip, ship);
        }

        private int GetAiFearfulPreyPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }
    }
}

namespace Conditions
{
    public class FearfulPrey : GenericToken
    {
        public FearfulPrey(GenericShip host) : base(host)
        {
            Name = ImageName = "Fearful Prey";
            Temporary = false;
            Tooltip = "Conditions/fearfulprey";
        }

        public override void WhenAssigned()
        {
            Host.OnAttackFinishAsDefender += CheckAbility;
        }

        public override void WhenRemoved()
        {
            Host.OnAttackFinishAsDefender -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (Combat.Attacker is Ship.SecondEdition.DroidTriFighter.FearsomePredator)
            {
                if (!Combat.SpentTokens.ContainsKey(Host)
                    || !Combat.SpentTokens[Host].Any(n =>
                        n == typeof(CalculateToken)
                        || n == typeof(FocusToken)
                        || n == typeof(EvadeToken)
                    )
                )
                {
                    Triggers.RegisterTrigger(
                        new Trigger()
                        {
                            Name = "Assign Strain token",
                            TriggerType = TriggerTypes.OnAttackFinish,
                            TriggerOwner = Host.Owner.AnotherPlayer.PlayerNo,
                            EventHandler = AssignStrainToken
                        }
                    );
                }
            }
        }

        private void AssignStrainToken(object sender, EventArgs e)
        {
            Messages.ShowInfo("Fearsme Predator: Defender didn't spend at least 1 green token - Strain token is assigned");
            Host.Tokens.AssignToken(typeof(Tokens.StrainToken), Triggers.FinishTrigger);
        }
    }
}
