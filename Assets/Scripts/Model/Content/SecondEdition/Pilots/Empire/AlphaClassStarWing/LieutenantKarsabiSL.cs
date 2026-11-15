using Abilities.SecondEdition;
using Actions;
using ActionsList;
using Arcs;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AlphaClassStarWing
    {
        public class LieutenantKarsabiSL : AlphaClassStarWing
        {
            public LieutenantKarsabiSL() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Lieutenant Karsabi",
                    "Payload Courier",
                    Faction.Imperial,
                    3,
                    5,
                    0,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LieutenantKarsabiSLAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    isStandardLayout: true,
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );

                PilotNameCanonical = "lieutenantkarsabi-ssl";

                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.ProtonTorpedoes));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.SaturationRockets));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.ElectronicBaffle));

                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(SlamAction), typeof(TargetLockAction),
                    ActionColor.Red));
                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(SlamAction), typeof(ReloadAction),
                    ActionColor.Red));

                ShipAbilities.Add(new AlphaClassStarWingSLAbility());
            }
        }

        public class LieutenantKarsabiSLXWA : LieutenantKarsabiSL
        {
            public LieutenantKarsabiSLXWA() : base()
            {
                var pilotInfo = PilotInfo as PilotCardInfo25;
                pilotInfo.Cost = 13;
                pilotInfo.LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace UpgradesList.SecondEdition
{
    public class SaturationRockets : GenericSpecialWeapon
    {
        public SaturationRockets() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                    "Saturation Rockets",
                    UpgradeType.Missile,
                    cost: 0,
                    isLimited: true,
                    weaponInfo: new SpecialWeaponInfo(
                        attackValue: 3,
                        minRange: 1,
                        maxRange: 2,
                        charges: 4,
                        regensCharges: true,
                        arc: ArcType.FullFront,
                        chargesCost: 2,
                        noRangeBonus: true
                    ),
                    abilityType: typeof(Abilities.SecondEdition.SaturationRocketsAbility),
                    restriction: new AbilityPresenceRestriction(typeof(LieutenantKarsabiSLAbility))
            );

            IsHidden = true;

            ImageUrl = "Pilots/lieutenantkarsabi-ssl";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SaturationRocketsAbility : GenericAbility
    {
        bool IsBonusAttack = false;

        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterSaturationRocketAbility;
            Phases.Events.OnRoundEnd += ClearVariables;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterSaturationRocketAbility;
            Phases.Events.OnRoundEnd -= ClearVariables;
        }

        public void RegisterSaturationRocketAbility()
        {
            if (Combat.ChosenWeapon == HostUpgrade)
            {
                RegisterAbilityTrigger(
                    TriggerTypes.OnAttackStart,
                    CheckFiringArc
                );
            }
        }

        public void CheckFiringArc(object sender, System.EventArgs e)
        {
            if (Combat.Defender != null
                && HostUpgrade.State.Charges > 0)
            {
                if (!IsBonusAttack) HostShip.OnCombatCheckExtraAttack += RegisterBonusAttack;

                if (Combat.ShotInfo.InArcByType(ArcType.Front))
                {
                    AskToUseAbility(
                        HostUpgrade.UpgradeInfo.Name,
                        NeverUseByDefault,
                        UseAbility,
                        showSkipButton: false,
                        descriptionLong: "Do you want to spend an additional charge to add an attack die?",
                        imageHolder: HostShip
                        );
                }
                else
                {
                    Triggers.FinishTrigger();
                }
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        public void UseAbility(object sender, EventArgs e)
        {
            HostShip.AfterGotNumberOfAttackDice += SaturationRocketAddAttackDice;
            HostUpgrade.State.SpendCharge();
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void SaturationRocketAddAttackDice(ref int value)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": +1 attack die");
            value++;
            HostShip.AfterGotNumberOfAttackDice -= SaturationRocketAddAttackDice;
        }

        public void RegisterBonusAttack(GenericShip ship)
        {
            IsBonusAttack = true;

            if (HostUpgrade.State.Charges > 0)
            {
                RegisterAbilityTrigger(
                    TriggerTypes.OnCombatCheckExtraAttack,
                    AskBonusAttack);
            }

            HostShip.OnCombatCheckExtraAttack -= RegisterBonusAttack;
        }

        public void AskBonusAttack(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                UseBonusAttack,
                showSkipButton: false,
                descriptionLong: "Do you want to spend an additional charge for a bonus attack?"
                );
        }

        public void UseBonusAttack(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + " spent a charge to get a bonus attack.");

            // Upgrade uses full initial charges instead of just 1 charge, we restore a charge first to balance it and not throw an error
            HostUpgrade.State.RestoreCharge();

            HostShip.OnCheckIsForbiddenWeapon += AllowUpgradeOnly;

            Combat.StartSelectAttackTarget(
                HostShip,
                FinishBonusAttack,
                abilityName: HostUpgrade.UpgradeInfo.Name,
                description: $"Select a target for bonus attack",
                showSkipButton: true,
                extraAttackFilter: BonusAttackCloseRangeOnly
            );
        }

        private bool BonusAttackCloseRangeOnly(GenericShip target, IShipWeapon weapon, bool isSilent)
        {
            return (weapon == HostUpgrade && Combat.ShotInfo.Range <= 1);
        }

        private void AllowUpgradeOnly(GenericShip target, IShipWeapon weapon, ref bool isForbidden)
        {
            isForbidden = weapon != HostUpgrade;
        }

        private void FinishBonusAttack()
        {
            HostShip.OnCheckIsForbiddenWeapon -= AllowUpgradeOnly;

            //if bonus attack was skipped, refund charge
            if (Selection.ThisShip.IsAttackSkipped)
            {
                HostUpgrade.State.RestoreCharge();
            }

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        public void ClearVariables()
        {
            IsBonusAttack = false;
        }
    }

    public class LieutenantKarsabiSLAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetReloadChargesCount += RegisterLieutenantKarsabiAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetReloadChargesCount -= RegisterLieutenantKarsabiAbility;
        }

        public void RegisterLieutenantKarsabiAbility(GenericUpgrade upgrade, ref int count)
        {
            count = 2;
        }
    }
}
