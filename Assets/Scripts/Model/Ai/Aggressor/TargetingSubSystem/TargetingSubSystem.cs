using BoardTools;
using Editions;
using Remote;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace AI.Aggressor
{
    public class AttackDecision
    {
        public GenericShip CurrentShip { get; private set; }
        public GenericShip TargetShip { get; private set; }
        public IShipWeapon Weapon { get; private set; }
        public int Priority { get; private set; }

        public AttackDecision(GenericShip currentShip, GenericShip targetShip, IShipWeapon weapon)
        {
            CurrentShip = currentShip;
            TargetShip = targetShip;
            Weapon = weapon;

            CalculatePriority();
        }

        private void CalculatePriority()
        {
            // Local constants

            const float attackDiceChanceUnmodified = 0.5f;
            const float attackDiceChanceSingleModification = 0.75f;
            const float attackDiceChanceFullModification = 0.938f;

            const float potentialCritsNoReroll = 0.125f;
            const float potentialCritsWithReroll = 0.1875f;

            const float defenceDiceChanceUnmodified = 0.375f;
            const float defenceDiceChanceFocusModification = 0.625f;

            ShotInfo shotInfo = new(CurrentShip, TargetShip, Weapon);

            // Attack dice

            float attackDiceThrown = Weapon.WeaponInfo.AttackValue;
            if (shotInfo.Range <= 1 && Edition.Current.IsWeaponHaveRangeBonus(Weapon)) attackDiceThrown++;

            float attackDiceModifier;
            float criticalHitsModifier = potentialCritsNoReroll;
            if (CurrentShip.Tokens.HasToken<FocusToken>() && ActionsHolder.HasTargetLockOn(CurrentShip, TargetShip))
            {
                attackDiceModifier = attackDiceChanceFullModification;
            }
            else if (CurrentShip.Tokens.HasToken<FocusToken>() || ActionsHolder.HasTargetLockOn(CurrentShip, TargetShip))
            {
                attackDiceModifier = attackDiceChanceSingleModification;
                if (ActionsHolder.HasTargetLockOn(CurrentShip, TargetShip)) criticalHitsModifier = potentialCritsWithReroll;
            }
            else
            {
                attackDiceModifier = attackDiceChanceUnmodified;
            }

            float potentialHits = attackDiceThrown * attackDiceModifier;

            // Defence dice

            float defenceDiceThrown = TargetShip.State.Agility;
            if (shotInfo.Range == 3 && !Edition.Current.IsWeaponHaveRangeBonus(Weapon)) defenceDiceThrown++;
            if (shotInfo.IsObstructedByObstacle) defenceDiceThrown++;

            float defenceDiceModifier;
            if (TargetShip.Tokens.HasToken<FocusToken>())
            {
                defenceDiceModifier = defenceDiceChanceFocusModification;
            }
            else
            {
                defenceDiceModifier = defenceDiceChanceUnmodified;
            }

            float potentialEvades = defenceDiceThrown * defenceDiceModifier;
            if (TargetShip.Tokens.HasToken<EvadeToken>() && defenceDiceThrown > 0)
            {
                potentialEvades += 1;
            }

            // Results

            float potentialDamage = potentialHits - potentialEvades;

            float targetHP = TargetShip.State.HullCurrent + TargetShip.State.ShieldsCurrent;
            float damageImpact = potentialDamage / targetHP;
            if (targetHP < potentialDamage) damageImpact *= 2;

            float potentialCrits = attackDiceThrown * criticalHitsModifier;
            float shipCost = TargetShip.PilotInfo.Cost;

            IShipWeapon currentWeapon;
            GenericUpgrade currentUpgrade = null;

            // Find the upgrade that matches our current weapon.
            foreach (GenericUpgrade upgrade in Selection.ThisShip.UpgradeBar.GetSpecialWeaponsActive())
            {
                if (upgrade is GenericSpecialWeapon)
                {
                    currentWeapon = (upgrade as IShipWeapon);
                    if (currentWeapon.Name == Weapon.Name)
                    {
                        currentUpgrade = upgrade;
                        break;
                    }
                }
            }

            // If our current weapon uses charges and has inadequate charges available, don't use it.
            if (currentUpgrade != null && Weapon.WeaponInfo.UsesCharges && currentUpgrade.UpgradeInfo.Charges > currentUpgrade.State.Charges)
            {
                Priority = 0;
            }
            else if (TargetShip is GenericRemote)
            {
                Priority = 1;
            }
            else
            {
                int priority = (int)(damageImpact * 1000f + potentialCrits * 100f + shipCost);
                CurrentShip.Ai.CallGetWeaponPriority(TargetShip, Weapon, ref priority);
                Priority = priority;
            }
        }
    }

    public static class TargetingSubSystem
    {
        public static GenericShip CurrentShip { get; private set; }

        public static List<AttackDecision> AttackDecisions;

        public static GenericShip SelectTargetAndWeapon(GenericShip ship)
        {
            CurrentShip = ship;
            AttackDecisions = new List<AttackDecision>();

            foreach (GenericShip enemyShip in GetEnemyShipsAndDistance(CurrentShip, inArcAndRange: true))
            {
                bool isAllowedByAbility = true;
                ship.CallTargetForAttackIsAllowed(enemyShip, ref isAllowedByAbility);

                if (!isAllowedByAbility) continue;

                Selection.AnotherShip = enemyShip;
                foreach (IShipWeapon weapon in CurrentShip.GetAllWeapons())
                {
                    if (Rules.TargetIsLegalForShot.IsLegal(CurrentShip, enemyShip, weapon, isSilent: true))
                    {
                        AttackDecisions.Add(new AttackDecision(CurrentShip, enemyShip, weapon));
                    }
                }
            }

            AttackDecision BestAttackDecision = AttackDecisions.OrderByDescending(n => n.Priority).FirstOrDefault();
            if (BestAttackDecision != null)
            {
                Combat.ChosenWeapon = BestAttackDecision.Weapon;
                Combat.ShotInfo = new ShotInfo(CurrentShip, BestAttackDecision.TargetShip, BestAttackDecision.Weapon);

                return BestAttackDecision.TargetShip;
            }
            else
            {
                return null;
            }
        }

        private static List<GenericShip> GetEnemyShipsAndDistance(GenericShip thisShip, bool ignoreCollided = false, bool inArcAndRange = false)
        {
            Dictionary<GenericShip, float> results = new();

            List<GenericShip> enemyTargets = Roster.GetPlayer(Roster.AnotherPlayer(thisShip.Owner.PlayerNo)).Ships.Values.ToList();
            enemyTargets.AddRange(Roster.GetPlayer(Roster.AnotherPlayer(thisShip.Owner.PlayerNo)).Units.Values.Where(n => n is GenericRemote).Cast<GenericShip>());

            foreach (GenericShip enemyShip in enemyTargets)
            {
                if (!enemyShip.IsDestroyed)
                {
                    if (ignoreCollided)
                    {
                        if (thisShip.LastShipCollision != null
                            && thisShip.LastShipCollision.ShipId == enemyShip.ShipId)
                        {
                            continue;
                        }


                        if (enemyShip.LastShipCollision != null
                            && enemyShip.LastShipCollision.ShipId == thisShip.ShipId)
                        {
                            continue;
                        }
                    }

                    if (inArcAndRange)
                    {
                        DistanceInfo distanceInfo = new(thisShip, enemyShip);

                        if ((distanceInfo.Range > 3))
                        {
                            continue;
                        }
                    }

                    float distance = Vector3.Distance(thisShip.GetCenter(), enemyShip.GetCenter());
                    results.Add(enemyShip, distance);
                }
            }

            results = results.OrderBy(n => n.Value).ToDictionary(n => n.Key, n => n.Value);

            return results.Select(n => n.Key).ToList();
        }
    }
}
