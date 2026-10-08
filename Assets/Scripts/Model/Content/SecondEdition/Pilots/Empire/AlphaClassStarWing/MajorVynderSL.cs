using Abilities.SecondEdition;
using Actions;
using ActionsList;
using Arcs;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AlphaClassStarWing
    {
        public class MajorVynderSL : AlphaClassStarWing
        {
            public MajorVynderSL() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Major Vynder",
                    "Helping Hand",
                    Faction.Imperial,
                    4,
                    5,
                    0,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.MajorVynderSLAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Cannon,
                        UpgradeType.Missile
                    },
                    isStandardLayout: true,
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );

                PilotNameCanonical = "majorvynder-ssl";

                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.LongRangeScanners));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.IonCannon));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.HeavyPlasmaMissiles));

                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(SlamAction), typeof(TargetLockAction), ActionColor.Red));
                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(SlamAction), typeof(ReloadAction), ActionColor.Red));

                ShipAbilities.Add(new AlphaClassStarWingSLAbility());
            }
        }

        public class MajorVynderSLXWA : MajorVynderSL
        {
            public MajorVynderSLXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 13;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace UpgradesList.SecondEdition
{
    public class LongRangeScanners : GenericUpgrade
    {
        public LongRangeScanners() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                    "Long-Range Scanners",
                    UpgradeType.Sensor,
                    cost: 0,
                    charges: 2,
                    abilityType: typeof(Abilities.SecondEdition.LongRangeScannersAbility),
                    restriction: new AbilityPresenceRestriction(typeof(MajorVynderSLAbility))
                );

            ImageUrl = "https://infinitearenas.com/xw2/images/pilots/majorvynder-ssl.png";
        }
    }

    public class HeavyPlasmaMissiles : GenericSpecialWeapon
    {
        public HeavyPlasmaMissiles() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                    "Heavy Plasma Missiles",
                    UpgradeType.Missile,
                    cost: 0,
                    weaponInfo: new SpecialWeaponInfo(
                        attackValue: 3,
                        minRange: 1,
                        maxRange: 3,
                        charges: 2,
                        noRangeBonus: true,
                        arc: ArcType.Front,
                        requiresToken: typeof(BlueTargetLockToken)
                    ),
                    abilityType: typeof(Abilities.SecondEdition.HeavyPlasmaMissilesAbility),
                    restriction: new AbilityPresenceRestriction(typeof(MajorVynderAbility))
                );

            ImageUrl = "https://infinitearenas.com/xw2/images/pilots/majorvynder-ssl.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MajorVynderSLAbility : GenericAbility
    {
        // After you perform a Missile attack, you may performa a bonus cannon attack. While you perform this bonus attack, you may change 1 focus to a hit result.
        bool usedBonusAttack;
        IShipWeapon prevWeapon;

        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterAbility;
            HostShip.OnRoundEnd += ClearFlags;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterAbility;
            HostShip.OnRoundEnd -= ClearFlags;
        }

        public void RegisterAbility()
        {
            if (!usedBonusAttack)
            {
                prevWeapon = Combat.ChosenWeapon;
                HostShip.OnCombatCheckExtraAttack += RegisterBonusAttack;
            }
        }

        public void RegisterBonusAttack(GenericShip ship)
        {
            usedBonusAttack = true;

            if (prevWeapon.WeaponType == WeaponTypes.Missile)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, PerformBonusAttack);
            }

            HostShip.OnCombatCheckExtraAttack -= RegisterBonusAttack;
        }

        public void PerformBonusAttack(object sender, EventArgs e)
        {
            HostShip.OnCheckIsForbiddenWeapon += AllowCannonOnly;

            AddDiceModification(
                "Major Vynder Ability",
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Change,
                1,
                new List<DieSide> { DieSide.Focus },
                DieSide.Success
            );

            Messages.ShowInfo($"Select a target for bonus attack.");

            Combat.StartSelectAttackTarget(
                HostShip,
                FinishBonusAttack,
                abilityName: HostShip.PilotName,
                description: $"You may perform a bonus cannon attack, if you do, you may change 1 focus result to a hit result.",
                showSkipButton: true
            );
        }

        public void AllowCannonOnly(GenericShip ship, IShipWeapon weapon, ref bool isForbidden)
        {
            isForbidden = weapon.WeaponType != WeaponTypes.Cannon;
        }

        public void FinishBonusAttack()
        {
            HostShip.OnCheckIsForbiddenWeapon -= AllowCannonOnly;

            Triggers.FinishTrigger();

            RemoveDiceModification();
        }

        public bool IsDiceModificationAvailable()
        {
            return Combat.CurrentDiceRoll.Focuses > 0;
        }

        private int GetDiceModificationAiPriority()
        {
            return 100;
        }

        private void ClearFlags(GenericShip ship)
        {
            usedBonusAttack = false;
            prevWeapon = null;
        }
    }

    public class LongRangeScannersAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.BeforeActionIsPerformed += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeActionIsPerformed -= RegisterAbility;
        }

        private void RegisterAbility(GenericAction action, ref bool isValid)
        {
            if (HostUpgrade.State.Charges > 0 && action.GetType() == typeof(TargetLockAction))
            {
                RegisterAbilityTrigger(TriggerTypes.BeforeActionIsPerformed, AskUseMajorVynderAbility);
            }

            isValid = true;
        }

        private void AskUseMajorVynderAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                NeverUseByDefault,
                UseMajorVynderAbility,
                descriptionLong: "Do you want to spend 1 Charge to acquire locks at any range?",
                imageHolder: HostShip
            );
        }

        private void UseMajorVynderAbility(object sender, EventArgs e)
        {
            HostUpgrade.State.SpendCharge();
            HostShip.SetTargetLockRange(0, int.MaxValue);

            DecisionSubPhase.ConfirmDecision();
        }
    }

    public class HeavyPlasmaMissilesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackHitAsAttacker += RegisterHeavyPlasmaMissileAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackHitAsAttacker -= RegisterHeavyPlasmaMissileAbility;
        }

        public void RegisterHeavyPlasmaMissileAbility()
        {
            if (Combat.ChosenWeapon == HostUpgrade)
            {
                RegisterAbilityTrigger(
                    TriggerTypes.OnAttackHit,
                    RemoveTargetShield
                );
            }
        }

        public void RemoveTargetShield(object sender, EventArgs e)
        {
            if (Combat.Defender != null && Combat.ChosenWeapon == HostUpgrade && Combat.Defender.State.ShieldsCurrent > 0)
            {
                DamageSourceEventArgs shieldDamage = new DamageSourceEventArgs()
                {
                    Source = HostUpgrade.UpgradeInfo.Name,
                    DamageType = DamageTypes.CardAbility
                };

                Combat.Defender.Damage.SufferRegularDamage(shieldDamage, Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}