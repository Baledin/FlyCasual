using Content;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class SeekerMissiles : GenericSpecialWeapon
    {
        public SeekerMissiles() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Seeker Missiles",
                UpgradeType.Missile,
                cost: 0,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 2,
                    maxRange: 3,
                    charges: 4,
                    chargesCost: 1,
                    arc: Arcs.ArcType.Front,
                    requiresToken: typeof(BlueTargetLockToken)
                ),
                abilityType: typeof(Abilities.SecondEdition.SeekerMissilesAbility),
                legalityInfo: new() { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            IsHidden = true;

            NameCanonical = "seekermissiles-legendsandrelics";
        }
    }

    public class SeekerMissilesXWA : SeekerMissiles
    {
        public SeekerMissilesXWA() : base()
        {
            UpgradeInfo.Cost = 12;
            UpgradeInfo.LegalityInfo = new() { Legality.XWA };
            IsHidden = false;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SeekerMissilesAbility : GenericAbility
    {
        int currentUseCount = 0;

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += ResetCurrentUseCount;

            AddDiceModification(name: "Seeker Missiles",
                isAvailable: IsAvailable,
                aiPriority: GetAIPriority,
                modificationType: DiceModificationType.Change,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Focus },
                sideCanBeChangedTo: DieSide.Success,
                payAbilityCost: PayAbilityCost,
                count: 1,
                canBeUsedFewTimes: true
            );
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= ResetCurrentUseCount;

            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            return currentUseCount < 2 &&
                Combat.Attacker == HostShip &&
                Combat.AttackStep == CombatStep.Attack &&
                Combat.ChosenWeapon == HostUpgrade &&
                Combat.DiceRollAttack.Focuses > 0 &&
                HostUpgrade.State.Charges > 0;
        }

        private int GetAIPriority()
        {
            return 80;
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            currentUseCount++;
            HostUpgrade.State.SpendCharge();
            callback(true);
        }

        private void ResetCurrentUseCount(GenericShip ship)
        {
            currentUseCount = 0;
        }
    }
}