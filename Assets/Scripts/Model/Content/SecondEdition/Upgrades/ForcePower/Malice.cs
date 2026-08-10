using Abilities.SecondEdition;
using ActionsList.SecondEdition;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Malice : GenericUpgrade
    {
        public Malice() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "Malice",
                UpgradeType.ForcePower,
                cost: 4,
                restriction: new TagRestriction(Tags.DarkSide),
                abilityType: typeof(MaliceAbility),
                legalityInfo: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class MaliceXWA : Malice
    {
        public MaliceXWA() : base()
        {
            UpgradeInfo.Cost = 5;
            UpgradeInfo.LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MaliceAbility : GenericAbility
    {
        public bool abilityUsed = false;

        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddMaliceAction;

            GenericShip.OnFaceupCritCardReadyToBeDealtGlobal += CheckRecoverForce;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddMaliceAction;

            GenericShip.OnFaceupCritCardReadyToBeDealtGlobal -= CheckRecoverForce;
        }

        private void AddMaliceAction(GenericShip ship)
        {
            ship.AddAvailableDiceModificationOwn(new MaliceDiceModification(HostUpgrade, this));
        }

        private void CheckRecoverForce(GenericShip ship, GenericDamageCard crit, EventArgs e)
        {
            if (abilityUsed
                && Combat.Defender != null
                && (Tools.IsSameShip(Combat.Attacker, HostShip))
                && Combat.CurrentCriticalHitCard.IsFaceup && (Combat.CurrentCriticalHitCard.Type == CriticalCardType.Pilot || Combat.CurrentCriticalHitCard.Type == CriticalCardType.Crew))
            {
                abilityUsed = false;
                HostShip.State.RestoreForce(2);
            }
        }
    }
}

namespace ActionsList.SecondEdition
{
    public class MaliceDiceModification : GenericAction
    {
        readonly GenericUpgrade HostUpgrade;
        readonly MaliceAbility HostAbility;

        public MaliceDiceModification(GenericUpgrade upgrade, MaliceAbility maliceAbility)
        {
            HostUpgrade = upgrade;
            Name = HostUpgrade.UpgradeInfo.Name;
            DiceModificationName = HostUpgrade.UpgradeInfo.Name;
            ImageUrl = HostUpgrade.ImageUrl;
            HostAbility = maliceAbility;
        }

        public override int GetActionPriority()
        {
            int result = 0;
            if (Combat.DiceRollAttack.RegularSuccesses + Combat.DiceRollAttack.Focuses > 0) result = 100;
            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            return (Combat.AttackStep == CombatStep.Attack) && (HostShip.State.Force >= 1) && (Combat.Attacker == HostShip);
        }

        public override void ActionEffect(Action callback)
        {
            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = DiceModificationName,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnAbilityDirect,
                    EventHandler = StartSubphase
                }
            );

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, callback);
        }

        private void StartSubphase(object sender, EventArgs e)
        {
            MaliceDecisionSubPhase spendDiceSubPhase = Phases.StartTemporarySubPhaseNew<MaliceDecisionSubPhase>(Name, Triggers.FinishTrigger);
            spendDiceSubPhase.HostUpgrade = HostUpgrade;
            spendDiceSubPhase.HostAbility = HostAbility;
            spendDiceSubPhase.ShowSkipButton = true;
            spendDiceSubPhase.OnSkipButtonIsPressed = DecisionSubPhase.ConfirmDecision;
            spendDiceSubPhase.DecisionOwner = HostShip.Owner;
            spendDiceSubPhase.Start();
        }
    }
}

namespace SubPhases
{
    public class MaliceDecisionSubPhase : SpendDiceResultDecisionSubPhase
    {
        public GenericUpgrade HostUpgrade;
        public MaliceAbility HostAbility;

        protected override void PrepareDiceResultEffects()
        {
            DescriptionShort = "Malice";
            DescriptionLong = "Select a die result to change to a Crit";
            ImageSource = HostUpgrade;

            if (HostUpgrade.HostShip.State.Force < 1) return;

            AddSpendDiceResultEffect(DieSide.Focus, "Focus result", delegate { SpendResultToChange(DieSide.Focus); });
            AddSpendDiceResultEffect(DieSide.Success, "Hit result", delegate { SpendResultToChange(DieSide.Success); });
        }

        private void SpendResultToChange(DieSide side)
        {
            HostUpgrade.HostShip.State.SpendForce
            (
                1,
                delegate
                {
                    HostAbility.abilityUsed = true;
                    Combat.DiceRollAttack.ChangeOne(side, DieSide.Crit);
                    DecisionSubPhase.ConfirmDecision();
                }
            );
        }
    }
}