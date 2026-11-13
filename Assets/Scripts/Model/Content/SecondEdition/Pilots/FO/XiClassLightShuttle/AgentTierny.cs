using BoardTools;
using Conditions;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.XiClassLightShuttle
{
    public class AgentTierny : XiClassLightShuttle
    {
        public AgentTierny() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Agent Tierny",
                "Persuasive Recruiter",
                Faction.FirstOrder,
                3,
                5,
                15,
                isLimited: true,
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Tech,
                    UpgradeType.Crew,
                    UpgradeType.Crew,
                    UpgradeType.Modification,
                    UpgradeType.Modification
                },
                abilityType: typeof(Abilities.SecondEdition.AgentTiernyAbility),
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            PilotNameCanonical = "agenttierny";
        }
    }

    public class AgentTiernyXWA : AgentTierny
    {
        public AgentTiernyXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 12;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 18;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Crew,
                UpgradeType.Crew,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Tech
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AgentTiernyAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnSetupEnd += RegisterAgentTiernyAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupEnd -= RegisterAgentTiernyAbility;
        }

        private void RegisterAgentTiernyAbility()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = HostShip.ShipId + ": Assign \"Broken Trust\" condition",
                TriggerType = TriggerTypes.OnSetupEnd,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = SelectAgentTiernyTarget,
            });
        }

        private void SelectAgentTiernyTarget(object Sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                  AssignBrokenTrust,
                  CheckRequirements,
                  GetAiPriority,
                  HostShip.Owner.PlayerNo,
                  "Broken Trust",
                  "Assign the Broken Trust condition to 1 enemy ship.",
                  HostUpgrade,
                  showSkipButton: false
            );
        }

        protected virtual void AssignBrokenTrust()
        {
            // Remove decoyed from all enemy ships
            foreach (KeyValuePair<string, GenericShip> kvp in Roster.AllShips)
            {
                GenericShip ship = kvp.Value;
                ship.Tokens.RemoveCondition(typeof(BrokenTrust));
            }

            TargetShip.Tokens.AssignCondition(new BrokenTrust(TargetShip) { SourceUpgrade = HostUpgrade });
            SelectShipSubPhase.FinishSelection();
        }

        protected virtual bool CheckRequirements(GenericShip ship)
        {
            return !Tools.IsSameTeam(ship, HostShip);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }
    }
}

namespace Conditions
{
    public class BrokenTrust : GenericToken
    {
        public GenericUpgrade SourceUpgrade;

        public BrokenTrust(GenericShip host) : base(host)
        {
            Name = ImageName = "Broken Trust Condition";
            Temporary = false;
            Tooltip = "Conditions/brokentrust";
        }

        public override void WhenAssigned()
        {
            Host.OnCheckIsFriendly += TreatAsAllied;
            Host.OnAttackStartAsAttacker += CheckAlliedStress;
            GenericShip.OnFaceupCritCardReadyToBeDealtGlobal += CheckRemoveBrokenTrust;
            Host.OnAttackFinish += CheckRemoveBrokenTrust;
        }

        public override void WhenRemoved()
        {
            Host.OnCheckIsFriendly -= TreatAsAllied;
            Host.OnAttackStartAsAttacker -= CheckAlliedStress;
            GenericShip.OnFaceupCritCardReadyToBeDealtGlobal -= CheckRemoveBrokenTrust;
            Host.OnAttackFinish -= CheckRemoveBrokenTrust;
        }

        public void CheckRemoveBrokenTrust(GenericShip ship)
        {
            if (Combat.Defender != null && Combat.Defender.IsDestroyed)
            {
                RemoveBrokenTrust(Host);
            }
        }

        private void CheckRemoveBrokenTrust(GenericShip ship, GenericDamageCard crit, EventArgs e)
        {
            if (Combat.Defender != null && (Tools.IsSameShip(Combat.Defender, Host) || Tools.IsSameShip(Combat.Attacker, Host)))
            {
                RemoveBrokenTrust(Host);
            }
        }

        private void RemoveBrokenTrust(GenericShip host)
        {
            Messages.ShowInfo(Host.PilotInfo.PilotName + " removed Broken Trust condition.");
            if (Host.Tokens.HasToken(typeof(BrokenTrust)))
            {
                Host.Tokens.RemoveCondition(typeof(BrokenTrust));
            }
        }

        private void CheckAlliedStress()
        {
            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                if (Tools.IsSameTeam(ship, Host) && !ship.IsStressed)
                {
                    ShotInfo shotInfo = new ShotInfo(Host, ship, Combat.ChosenWeapon);
                    if (shotInfo.InArc)
                    {
                        Triggers.RegisterTrigger
                        (
                            new Trigger()
                            {
                                Name = "Broken Trust",
                                TriggerType = TriggerTypes.OnAttackStart,
                                TriggerOwner = Host.Owner.PlayerNo,
                                EventHandler = (object sender, EventArgs e) => AssignStress(ship)
                            }
                        );

                    }
                }
            }
        }

        private void AssignStress(GenericShip ship)
        {
            Messages.ShowInfo(ship.PilotInfo.PilotName + " gains one stress token from Broken Trust condition.");
            ship.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
        }

        private void TreatAsAllied(GenericShip ship, ref bool friendly)
        {
            friendly = false;
        }
    }
}