using ActionsList;
using Content;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.TIESeBomber
{
    public class JulJerjerrod : TIESeBomber
    {
        public JulJerjerrod() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Jul Jerjerrod",
                "Security Commander",
                Faction.FirstOrder,
                4,
                4,
                13,
                isLimited: true,
                charges: 3,
                abilityType: typeof(Abilities.SecondEdition.JulJerjerrodPilotAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Tech,
                    UpgradeType.Torpedo,
                    UpgradeType.Missile,
                    UpgradeType.Gunner,
                    UpgradeType.Device,
                    UpgradeType.Device,
                    UpgradeType.Modification,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Tie
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class JulJerjerrodXWA : JulJerjerrod
    {
        public JulJerjerrodXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 10;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 14;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Gunner,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Device,
                UpgradeType.Device,
                UpgradeType.Missile,
                UpgradeType.Torpedo
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JulJerjerrodPilotAbility : GenericAbility
    {
        // After you perform a Boost Action, you may spend 1 Charge to remove 1 non-lock red or orange token.

        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbilityAfterBoost;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbilityAfterBoost;
        }

        private void CheckAbilityAfterBoost(GenericAction action)
        {
            if (action is BoostAction && HostShip.State.Charges > 0 && HostShip.Tokens.GetNonLockRedOrangeTokens().Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskUseAbility);
            }
        }
        private void AskUseAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                NeverUseByDefault,
                UseAbility,
                descriptionLong: "Do you want to spend 1 Charge to remove 1 non-lock red or orange token?",
                imageHolder: HostShip
            );
        }
        private void UseAbility(object sender, EventArgs e)
        {
            JulJerjerrodAbilitySubphase subphase = Phases.StartTemporarySubPhaseNew<JulJerjerrodAbilitySubphase>(
                "Remove Token",
                DecisionSubPhase.ConfirmDecision
            );

            subphase.Name = HostShip.PilotInfo.PilotName;
            subphase.DescriptionShort = "Select a non-lock red or orange token to remove";
            subphase.ImageSource = HostShip;

            subphase.DecisionOwner = HostShip.Owner;
            subphase.ShowSkipButton = true;

            HostShip.SpendCharge();

            List<GenericToken> tokensToRemove = HostShip.Tokens.GetNonLockRedOrangeTokens();

            foreach (GenericToken token in tokensToRemove)
            {
                subphase.AddDecision(
                    token.Name,
                    delegate
                    {
                        HostShip.Tokens.RemoveToken(token, DecisionSubPhase.ConfirmDecision);
                    }
                );
            }


            if (subphase.GetDecisions().Count > 0)
            {
                subphase.Start();
            }
            else
            {
                Phases.GoBack();
                Messages.ShowInfoToHuman("Jul Jerjerrod: No tokens to remove");
                Triggers.FinishTrigger();
            }
        }

        private class JulJerjerrodAbilitySubphase : DecisionSubPhase { }
    }
}