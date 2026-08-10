using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.RZ2AWing
{
    public class ZiziTlo : RZ2AWing
    {
        public ZiziTlo() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Zizi Tlo",
                "Committed to the Cause",
                Faction.Resistance,
                5,
                4,
                7,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.ZiziTloAbility),
                charges: 1,
                regensCharges: 1,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Missile
                },
                tags: new List<Tags>
                {
                    Tags.AWing
                },
                skinName: "Red",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class ZiziTloXWA : ZiziTlo
    {
        public ZiziTloXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 10;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 8;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Talent,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Tech,
                UpgradeType.Missile
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you defend or perform an attack, you may spend 1 charge to gain 1 focus or evade token.
    public class ZiziTloAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinish += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinish -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (HostShip.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, ChooseToken);
            }
        }

        private void ChooseToken(object sender, EventArgs e)
        {
            if (HostShip.State.Charges > 0)
            {
                DecisionSubPhase decisionSubPhase = Phases.StartTemporarySubPhaseNew<DecisionSubPhase>(
                    Name,
                    Triggers.FinishTrigger
                );

                decisionSubPhase.DescriptionShort = HostName + ": Spend 1 charge to gain 1 focus or evade token?";

                decisionSubPhase.AddDecision(
                    "Focus",
                    delegate
                    {
                        GainToken("Focus");
                    }
                );
                decisionSubPhase.AddDecision(
                     "Evade",
                     delegate
                     {
                         GainToken("Evade");
                     }
                 );

                decisionSubPhase.DefaultDecisionName = HostShip.Tokens.HasToken<FocusToken>() ? "Evade" : "Focus";
                decisionSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;
                decisionSubPhase.ShowSkipButton = true;

                decisionSubPhase.Start();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void GainToken(string tokenName)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            if (HostShip.State.Charges > 0)
            {
                Type tokenType = tokenName == "Focus" ? typeof(FocusToken) : typeof(EvadeToken);

                Messages.ShowInfo(HostName + " gains 1 " + tokenName + " token");
                HostShip.State.Charges--;
                HostShip.Tokens.AssignToken(tokenType, Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}