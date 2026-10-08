using Content;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class TargetingMatrix : GenericUpgrade
    {
        public TargetingMatrix() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "Targeting Matrix",
                UpgradeType.Modification,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.TargetingMatrixAbility),
                legalityInfo: new List<Legality>() { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            NameCanonical = "targetingmatrix-legendsandrelics";
        }
    }

    public class TargetingMatrixXWA : TargetingMatrix
    {
        public TargetingMatrixXWA() : base()
        {
            IsHidden = false;

            UpgradeInfo.Cost = 6;
            UpgradeInfo.LegalityInfo = new List<Legality>() { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TargetingMatrixAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAfterNeutralizeResultsAttacker += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAfterNeutralizeResultsAttacker -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (HostShip == Combat.Attacker && Combat.DiceRollAttack.Focuses > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAfterNeutralizeResultsAttacker, AskToStrain);
            }
        }

        private void AskToStrain(object sender, EventArgs e)
        {
            if (alwaysUseAbility)
            {
                UseAbility();
            }
            else
            {
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    AlwaysUseByDefault,
                    UseAbility,
                    descriptionLong: "Do you want to spend one focus result to assign a strain token to the defender?",
                    imageHolder: HostUpgrade,
                    showAlwaysUseOption: true,
                    callback: Triggers.FinishTrigger
                );
            }
        }

        private void UseAbility()
        {
            Combat.DiceRollAttack.RemoveType(DieSide.Focus);
            Combat.Defender.Tokens.AssignToken(new StrainToken(Combat.Defender), Triggers.FinishTrigger);
        }

        private void UseAbility(object sender, EventArgs e)
        {
            Combat.DiceRollAttack.RemoveType(DieSide.Focus);
            Combat.Defender.Tokens.AssignToken(new StrainToken(Combat.Defender), DecisionSubPhase.ConfirmDecision);
        }
    }
}