using Abilities.SecondEdition;
using Ship;
using System;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Determination : GenericUpgrade
    {
        public Determination() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                name: "Determination",
                type: UpgradeType.Talent,
                cost: 0,
                abilityType: typeof(DeterminationAbility)
            );

            IsHidden = true;

            ImageUrl = "https://infinitearenas.com/xw2xwa/images/pilots/kyloren-evacuationofdqar.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    // While you perform a primary attack, if the defender is in your bullseye, you may suffer 1 damage to add 1 focus result.

    public class DeterminationAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                name: HostUpgrade.UpgradeInfo.Name,
                isAvailable: IsAvailable,
                aiPriority: AiPriority,
                modificationType: DiceModificationType.Add,
                count: 1,
                payAbilityCost: PayCost,
                sideCanBeChangedTo: DieSide.Focus
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private void PayCost(Action<bool> callback)
        {
            Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name}: {HostShip.PilotInfo.PilotName} ship suffered 1 damage.");

            DamageSourceEventArgs determinationDamage = new()
            {
                Source = "Determination Upgrade Ability",
                DamageType = DamageTypes.CardAbility
            };

            HostShip.Damage.TryResolveDamage(1, determinationDamage, delegate { callback(true); });
        }

        private int AiPriority()
        {
            if (HostShip.State.HullCurrent + HostShip.State.ShieldsCurrent < 2  // Don't kill self to use ability
                || Combat.CurrentDiceRoll.Successes > Combat.Defender.State.Agility + Combat.Defender.State.HullCurrent)  // Overkill, don't use ability
            {
                return 0;
            }

            double result = 0;

            result += HostShip.State.Force > Combat.CurrentDiceRoll.Focuses ? 50 : 0;

            result += HostShip.Tokens.HasToken(typeof(FocusToken)) ? 100 : 0;

            result += result > 0 && Combat.CurrentDiceRoll.Successes + 1 >= Combat.Defender.State.Agility + Combat.Defender.State.HullCurrent ? 100 : 0; // Has force/focus and an extra eyeball will ensure kill

            result -= HostShip.State.ShieldsCurrent == 0 ? 100 : 0; // Reduce likelihood of use if no shields

            result *= HostShip.PilotInfo.Cost < Combat.Defender.PilotInfo.Cost ? 2 : 1; // David and Goliath, taking a hit may be worth it for points

            result *= HostShip.State.HullCurrent + HostShip.State.ShieldsCurrent < (HostShip.State.HullMax + HostShip.State.ShieldsMax) / 2 ? 0.5 : 1; // Limit use if below half health

            return (int)Math.Round(result);
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.ChosenWeapon is PrimaryWeaponClass
                && HostShip.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Bullseye);
        }
    }
}