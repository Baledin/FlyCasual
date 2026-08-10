using Abilities.SecondEdition;
using Content;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.TIESfFighter
{
    public class Theta4EoD : TIESfFighter
    {
        public Theta4EoD() : base()
        {
            PilotInfo = new PilotCardInfo25(
                pilotName: "Theta 4",
                pilotTitle: "Evacuation of D'Qar",
                faction: Faction.FirstOrder,
                initiative: 4,
                cost: 11,
                loadoutValue: 0,
                isLimited: true,
                limited: 1,
                charges: 1,
                regensCharges: 1,
                abilityType: typeof(Theta4EoDAbility),
                extraUpgradeIcons: new()
                {
                    UpgradeType.Talent,
                    UpgradeType.Missile,
                    UpgradeType.Missile,
                    UpgradeType.Tech
                },
                tags: new()
                {
                    Tags.Tie
                },
                isStandardLayout: true,
                legality: new List<Legality>() { Legality.XWA }
            );

            PilotNameCanonical = "theta4-evacuationofdqar";

            MustHaveUpgrades.Add(typeof(Determination));
            MustHaveUpgrades.Add(typeof(BarrageRockets));
            MustHaveUpgrades.Add(typeof(PatternAnalyzer));

            ShipAbilities.Add(new HeavyWeaponTurretEoD());

            ShipInfo.ActionIcons.LinkedActions.Clear();
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Theta4EoDAbility : GenericAbility
    {
        // While an enemy ship in your turret arc performs an attack, you may spend 1 charge. If you do, spend 1 charge from your missile upgrade to choose two attack dice.
        // The attacker must reroll those dice.

        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsDiceModificationAvailable,
                GetDiceModificationPriority,
                DiceModificationType.Reroll,
                2,
                timing: DiceModificationTimingType.Opposite,
                payAbilityCost: PayCost
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsDiceModificationAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Tools.IsAnotherTeam(HostShip, Combat.Attacker)
                && HostShip.State.Charges > 0
                && HostShip.UpgradeBar.GetInstalledUpgrade(UpgradeType.Missile).State.Charges > 0
                && HostShip.SectorsInfo.HasShipInTurretArc(Combat.Attacker);
        }

        private int GetDiceModificationPriority()
        {
            return 50; // We may want to customize this
        }

        private void PayCost(Action<bool> callback)
        {
            HostShip.SpendCharge();
            HostShip.UpgradeBar.GetInstalledUpgrade(UpgradeType.Missile).State.SpendCharge();
            callback(true);
        }
    }
}