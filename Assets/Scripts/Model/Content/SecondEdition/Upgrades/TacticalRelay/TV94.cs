using BoardTools;
using Ship;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class TV94 : GenericUpgrade
    {
        public TV94() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "TV-94",
                UpgradeType.TacticalRelay,
                cost: 3,
                isLimited: true,
                isSolitary: true,
                restriction: new FactionRestriction(Faction.Separatists),
                abilityType: typeof(Abilities.SecondEdition.TV94Ability)
            );

            Avatar = new AvatarInfo(
                Faction.Separatists,
                new Vector2(401, 50)
            );


        }
    }
}

namespace Abilities.SecondEdition
{
    public class TV94Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostUpgrade.UpgradeInfo.Name,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Add,
                1,
                sideCanBeChangedTo: DieSide.Success,
                isGlobal: true,
                payAbilityCost: PayCalculateToken
            );
        }

        private void PayCalculateToken(Action<bool> callback)
        {
            if (Combat.Attacker.Tokens.HasToken<Tokens.CalculateToken>())
            {
                Combat.Attacker.Tokens.SpendToken(
                    typeof(Tokens.CalculateToken),
                    () => callback(true)
                );
            }
            else
            {
                callback(false);
            }
        }

        private int GetDiceModificationAiPriority()
        {
            return (Combat.Attacker.Tokens.HasToken<Tokens.CalculateToken>()) ? 110 : 0;
        }

        private bool IsDiceModificationAvailable()
        {
            if (Combat.AttackStep != CombatStep.Attack) return false;

            if (Combat.Attacker.Owner.PlayerNo != HostShip.Owner.PlayerNo) return false;

            if (Combat.ChosenWeapon.WeaponType != WeaponTypes.PrimaryWeapon) return false;

            DistanceInfo distInfo = new(HostShip, Combat.Attacker);
            if (distInfo.Range > 4) return false;

            if (!Combat.Attacker.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Bullseye)) return false;

            if (DiceRoll.CurrentDiceRoll.Count > 2) return false;

            if (!Combat.Attacker.Tokens.HasToken<Tokens.CalculateToken>()) return false;

            return true;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}