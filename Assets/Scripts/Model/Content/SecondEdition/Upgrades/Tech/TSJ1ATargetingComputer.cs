using Abilities.SecondEdition;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class TSJ1ATargetingComputer : GenericUpgrade
    {
        public TSJ1ATargetingComputer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "T-SJ1A Targeting Computer",
                UpgradeType.Tech,
                cost: 4,
                charges: 1,
                abilityType: typeof(TSJ1ATargetingComputerAbility),
                restriction: new FactionRestriction(Faction.FirstOrder),
                legalityInfo: new List<Legality>() { Legality.XWA }
            );

            NameCanonical = "tsj1atargetingcomputer-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TSJ1ATargetingComputerAbility : GenericAbility
    {
        // While you perform a primary attack, if the defender does not have any green tokens,
        // you may spend 1 charge and 1 crit result. If you do, add 2 hit results.
        public override void ActivateAbility()
        {
            AddDiceModification(
                name: HostUpgrade.UpgradeInfo.Name,
                isAvailable: IsDiceModificationAvailable,
                aiPriority: GetAiPriority,
                modificationType: DiceModificationType.Add,
                count: 2,
                payAbilityCost: PayCost,
                sideCanBeChangedTo: DieSide.Success
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsDiceModificationAvailable()
        {
            return Combat.Attacker == HostShip &&
                HostUpgrade.State.Charges > 0 &&
                Combat.ChosenWeapon is PrimaryWeaponClass &&
                !Combat.Defender.Tokens.HasGreenTokens &&
                Combat.DiceRollAttack.CriticalSuccesses > 0;
        }

        private int GetAiPriority()
        {
            if (Combat.DiceRollAttack.Successes < Combat.Defender.State.ShieldsCurrent) return 100; // crit would be eaten by shield anyway

            if (Combat.DiceRollAttack.Successes - Combat.Defender.GetNumberOfDefenceDice() == Combat.Defender.State.HullCurrent - 1) return 100; // The extra success would ensure a kill

            return 0;
        }

        private void PayCost(Action<bool> callback)
        {
            HostUpgrade.State.SpendCharge();
            Combat.DiceRollAttack.RemoveType(DieSide.Crit);

            callback(true);
        }
    }
}