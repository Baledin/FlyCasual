using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class DeuteriumPowerCells : GenericUpgrade
    {
        public DeuteriumPowerCells() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Deuterium Power Cells",
                types: new List<UpgradeType> { UpgradeType.Tech, UpgradeType.Modification },
                cost: 6,
                charges: 2,
                restriction: new FactionRestriction(Faction.FirstOrder),
                abilityType: typeof(Abilities.SecondEdition.DeuteriumPowerCellsAbility),
                legalityInfo: new() { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class DeuteriumPowerCellsXWA : DeuteriumPowerCells
    {
        public DeuteriumPowerCellsXWA() : base()
        {
            UpgradeInfo.Cost = 7;
            UpgradeInfo.LegalityInfo = new() { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    // During the System Phase, you may spend 1 charge and gain 1 disarm token to recover 1 shield.
    // Before you would gain 1 non-lock token, if you are not stressed, you may spend 1 charge to gain 1 stress token instead.

    public class DeuteriumPowerCellsAbility : GenericAbility
    {
        readonly List<GenericToken> ignoreTokens = new();

        public override void ActivateAbility()
        {
            ignoreTokens.Add(new GenericTargetLockToken(HostShip));
            ignoreTokens.Add(new BlueTargetLockToken(HostShip));
            ignoreTokens.Add(new RedTargetLockToken(HostShip));
            ignoreTokens.Add(new StressToken(HostShip));
            ignoreTokens.Add(new ChargeToken(HostShip));

            HostShip.OnSystemsAbilityActivation += CheckRegenerationAbility;
            HostShip.OnCheckSystemsAbilityActivation += CheckAbility;
            HostShip.BeforeTokenIsAssigned += CheckTokenProtection;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsAbilityActivation -= CheckRegenerationAbility;
            HostShip.OnCheckSystemsAbilityActivation -= CheckAbility;
            HostShip.BeforeTokenIsAssigned -= CheckTokenProtection;
        }

        private bool IsRechargeAvailable()
        {
            return HostShip.State.ShieldsCurrent < HostShip.State.ShieldsMax && HostUpgrade.State.Charges > 0;
        }

        private void CheckAbility(GenericShip ship, ref bool isAbilityActive)
        {
            isAbilityActive = IsRechargeAvailable();
        }

        private void CheckTokenProtection(GenericShip ship, GenericToken token)
        {
            if (!HostShip.IsStressed && HostUpgrade.State.Charges > 0
                && !ignoreTokens.Any(t => token.GetType() == t.GetType())
            )
            {
                RegisterAbilityTrigger(TriggerTypes.OnBeforeTokenIsAssigned, AskToReplaceToken);
            }
        }

        private void AskToReplaceToken(object sender, EventArgs e)
        {
            DeuteriumPowerCellsDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<DeuteriumPowerCellsDecisionSubphase>(
                $"{HostUpgrade.UpgradeInfo.Name} Decision SubPhase",
                delegate
                {
                    Phases.FinishSubPhase(typeof(DeuteriumPowerCellsDecisionSubphase));
                    Triggers.FinishTrigger();
                }
            );

            subphase.DescriptionShort = $"{HostUpgrade.UpgradeInfo.Name} Decision";
            subphase.DescriptionLong = $"Do you want to spend 1 charge to gain 1 stress token instead of {HostShip.Tokens.TokenToAssign.Name}?";
            subphase.ImageSource = HostUpgrade;

            subphase.AddDecision("Yes", DoReplaceToken);
            subphase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); });
            subphase.AddDecision($"Always ignore {HostShip.Tokens.TokenToAssign.Name}", AlwaysIgnoreToken);

            subphase.DefaultDecisionName = "No";
            subphase.ShowSkipButton = false;

            subphase.Start();
        }

        private void AlwaysIgnoreToken(object sender, EventArgs e)
        {
            GenericToken token = HostShip.Tokens.TokenToAssign switch
            {
                JamToken or TractorBeamToken => (GenericToken)Activator.CreateInstance(HostShip.Tokens.TokenToAssign.GetType(), HostShip, HostShip.Owner),
                _ => (GenericToken)Activator.CreateInstance(HostShip.Tokens.TokenToAssign.GetType(), HostShip),
            };

            ignoreTokens.Add(token);
            DecisionSubPhase.ConfirmDecision();
        }

        private void DoReplaceToken(object sender, EventArgs e)
        {
            HostUpgrade.State.SpendCharge();

            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": Stress token is assigned instead of planned token");

            HostShip.Tokens.TokenToAssign = null;

            HostShip.Tokens.AssignToken(typeof(StressToken), DecisionSubPhase.ConfirmDecision);
        }

        private void CheckRegenerationAbility(GenericShip ship)
        {
            if (IsRechargeAvailable())
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToRegen);
            }
        }

        private void AskToRegen(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                DoRegen,
                descriptionLong: "Do you want to spend 1 charge and gain 1 disarm token to recover 1 shield?",
                imageHolder: HostUpgrade,
                callback: Triggers.FinishTrigger
            );
        }

        private void DoRegen(object sender, EventArgs e)
        {
            if (HostShip.TryRegenShields())
            {
                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": " + HostShip.PilotInfo.PilotName + " recovered 1 shield");
                HostUpgrade.State.SpendCharge();
                HostShip.Tokens.AssignToken(typeof(WeaponsDisabledToken), DecisionSubPhase.ConfirmDecision);
            }
            else
            {
                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": " + HostShip.PilotInfo.PilotName + " failed to recover 1 shield");
            }
        }

        private class DeuteriumPowerCellsDecisionSubphase : DecisionSubPhase { }
    }
}