using ActionsList;
using Arcs;
using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class VeteranTurretGunner : GenericUpgrade
    {
        public VeteranTurretGunner() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Veteran Turret Gunner",
                UpgradeType.Gunner,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.VeteranTurretGunnerAbility),
                restriction: new ActionBarRestriction(typeof(RotateArcAction)),
                seImageNumber: 52,
                legalityInfo: new() { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            Avatar = new AvatarInfo(
                Faction.Scum,
                new Vector2(423, 17),
                new Vector2(150, 150)
            );
        }
    }

    public class VeteranTurretGunnerXWA : VeteranTurretGunner
    {
        public VeteranTurretGunnerXWA() : base()
        {
            UpgradeInfo.Cost = 6;
            UpgradeInfo.LegalityInfo = new() { Legality.XWA };
            UpgradeInfo.Limited = 3;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class VeteranTurretGunnerAbility : GenericAbility
    {
        // After you perform a primary attack, you may perform a bonus turret arc
        // attack using a turret arc you did not already attack from this round.

        ArcFacing prevFacing = ArcFacing.None;

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += CheckAbility;
            HostShip.Ai.OnGetWeaponPriority += ModifyWeaponPriority;
            HostShip.Ai.OnGetActionPriority += ModifyRotateArcActionPriority;
            HostShip.Ai.OnGetRotateArcFacingPriority += ModifyRotateArcFacingPriority;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= CheckAbility;
            HostShip.Ai.OnGetWeaponPriority -= ModifyWeaponPriority;
            HostShip.Ai.OnGetActionPriority -= ModifyRotateArcActionPriority;
            HostShip.Ai.OnGetRotateArcFacingPriority -= ModifyRotateArcFacingPriority;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (Combat.ShotInfo.Weapon.WeaponType != WeaponTypes.PrimaryWeapon) return;

            if (HostShip.IsCannotAttackSecondTime) return;

            prevFacing = Combat.ArcForShot.Facing;

            HostShip.OnCombatCheckExtraAttack += RegisterExtraAttack;
        }

        private void RegisterExtraAttack(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterExtraAttack;
            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, UseGunnerAbility);
        }

        private void UseGunnerAbility(object sender, System.EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime && HostShip.ArcsInfo.Arcs.Any(a => a.IsTurretArc && a.Facing != prevFacing))
            {
                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartSelectAttackTarget(
                    ship: HostShip,
                    callback: FinishAdditionalAttack,
                    extraAttackFilter: IsUnusedTurretArcShot,
                    abilityName: HostUpgrade.UpgradeInfo.Name,
                    description: "You may perform a bonus turret arc attack using another turret arc",
                    imageSource: HostUpgrade
                );
            }
            else
            {
                Messages.ShowErrorToHuman($"{HostShip.PilotInfo.PilotName} cannot attack an additional time");
                Triggers.FinishTrigger();
            }
        }

        private void FinishAdditionalAttack()
        {
            // If attack is skipped, set this flag, otherwise regular attack can be performed second time
            HostShip.IsAttackPerformed = true;

            //if bonus attack was skipped, allow bonus attacks again
            if (HostShip.IsAttackSkipped) HostShip.IsCannotAttackSecondTime = false;

            Triggers.FinishTrigger();
        }

        private bool IsUnusedTurretArcShot(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            ShotInfo shotInfo = new(HostShip, defender, weapon);

            if (!shotInfo.ShotAvailableFromArcs.Any(a => a.IsTurretArc && a.Facing != prevFacing))
            {
                if (!isSilent) Messages.ShowError("Your attack must use a turret arc you have not already attacked from this round");
                return false;
            }

            if (!weapon.WeaponInfo.ArcRestrictions.Contains(ArcType.SingleTurret))
            {
                if (!isSilent) Messages.ShowError("Your attack must use a turret arc");
                return false;
            }

            return true;
        }

        private void ModifyWeaponPriority(GenericShip targetShip, IShipWeapon weapon, ref int priority)
        {
            //If this is first attack, and ship can trigger VTG - priorize primary non-turret weapon
            if
            (
                !HostShip.IsAttackPerformed
                && weapon.WeaponType == WeaponTypes.PrimaryWeapon
                && !weapon.WeaponInfo.ArcRestrictions.Contains(ArcType.SingleTurret)
                && CanAttackTargetWithPrimaryWeapon(targetShip)
                && CanAttackTargetWithTurret(targetShip)
            )
            {
                priority += 2000;
            }
        }

        private bool CanAttackTargetWithTurret(GenericShip targetShip)
        {
            foreach (GenericUpgrade turretUpgrade in Selection.ThisShip.UpgradeBar.GetSpecialWeaponsAll())
            {
                IShipWeapon turretWeapon = turretUpgrade as IShipWeapon;
                if (turretWeapon.WeaponType == WeaponTypes.Turret)
                {
                    if (new ShotInfo(HostShip, targetShip, turretWeapon).IsShotAvailable)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CanAttackTargetWithPrimaryWeapon(GenericShip targetShip)
        {
            //AI tries to check non-turret weapon first
            IShipWeapon weapon = HostShip.PrimaryWeapons.FirstOrDefault(w => !w.WeaponInfo.ArcRestrictions.Contains(ArcType.SingleTurret));

            weapon ??= HostShip.PrimaryWeapons.First();

            return new ShotInfo(HostShip, targetShip, weapon).IsShotAvailable;
        }

        private void ModifyRotateArcActionPriority(GenericAction action, ref int priority)
        {
            if (action is RotateArcAction)
            {
                // Rotate arc if ship has fixed arcs, has a target in it, but doesn't have a turret pointer in that sector
                List<GenericArc> fixedArcs = HostShip.ArcsInfo.Arcs
                                                            .Where(a => (!(a is ArcSingleTurret || a is OutOfArc)))
                                                            .ToList();

                if (fixedArcs.Count > 0 && HasTargetForWeapons(fixedArcs))
                {
                    List<ArcSingleTurret> singleTurretArcs = HostShip.ArcsInfo.Arcs
                                                            .Where(a => a is ArcSingleTurret)
                                                            .Select(a => a as ArcSingleTurret)
                                                            .ToList();

                    if (!singleTurretArcs.Any(sta => fixedArcs.Any(fa => fa.Facing == sta.Facing)))
                    {
                        priority += 100;
                    }
                }
            }
        }

        private bool HasTargetForWeapons(List<GenericArc> arcs)
        {
            foreach (GenericArc arc in arcs)
            {
                foreach (GenericShip enemyShip in HostShip.Owner.EnemyShips.Values)
                {
                    if (new ShotInfoArc(HostShip, enemyShip, arc).IsShotAvailable) return true;
                }
            }

            return false;
        }

        private void ModifyRotateArcFacingPriority(ArcFacing facing, ref int priority)
        {
            if (facing == ArcFacing.Front && NoArcInFrontSector())
            {
                priority += (IsEnemyInFrontSector()) ? 100 : 5;
            }
        }

        private bool NoArcInFrontSector()
        {
            return !HostShip.ArcsInfo.Arcs.Any(a => a.ArcType == ArcType.SingleTurret && a.Facing == ArcFacing.Front);
        }

        private bool IsEnemyInFrontSector()
        {
            return HostShip.Owner.EnemyShips.Any(e => HostShip.SectorsInfo.IsShipInSector(e.Value, ArcType.Front));
        }
    }
}