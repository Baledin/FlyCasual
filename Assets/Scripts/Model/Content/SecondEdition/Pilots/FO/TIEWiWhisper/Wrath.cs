using BoardTools;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.TIEWiWhisperModifiedInterceptor
{
    public class Wrath : TIEWiWhisperModifiedInterceptor
    {
        public Wrath() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "\"Wrath\"",
                "Herald of Destruction",
                Faction.FirstOrder,
                5,
                5,
                15,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.WrathPilotAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Cannon,
                    UpgradeType.Tech,
                    UpgradeType.Tech,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.Tie
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class WrathXWA : Wrath
    {
        public WrathXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 12;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 14;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Gunner,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Tech,
                UpgradeType.Cannon,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WrathPilotAbility : GenericAbility
    {
        private GenericShip OriginalDefender;

        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += SaveOriginalDefender;
            HostShip.OnAttackFinishAsAttacker += CheckWrathAbility;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= SaveOriginalDefender;
            HostShip.OnAttackFinishAsAttacker -= CheckWrathAbility;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void SaveOriginalDefender()
        {
            OriginalDefender = Combat.Defender;
        }

        private void CheckWrathAbility(GenericShip ship)
        {
            if (Combat.ArcForShot.ArcType == Arcs.ArcType.Bullseye
                && HasOrangeOrRedNonLockTokens()
                && !IsAbilityUsed)
            {
                IsAbilityUsed = true;

                HostShip.OnCombatCheckExtraAttack += RegisterWrathAbility;
            }
        }

        private bool HasOrangeOrRedNonLockTokens()
        {
            if (HostShip.Tokens.CountTokensByColor(TokenColors.Orange) > 0) return true;
            if (HostShip.Tokens.GetTokensByColor(TokenColors.Red).Count(n => n is not RedTargetLockToken) > 0) return true;
            return false;
        }

        private void RegisterWrathAbility(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterWrathAbility;

            if (AnotherTargetsPresent()) RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, UseWrathAbility);
        }

        private bool AnotherTargetsPresent()
        {
            foreach (GenericShip ship in HostShip.Owner.EnemyShips.Values)
            {
                if (Tools.IsSameShip(ship, OriginalDefender)) continue;

                foreach (IShipWeapon weapon in HostShip.GetAllWeapons())
                {
                    ShotInfo shotInfo = new (HostShip, ship, weapon);
                    if (shotInfo.IsShotAvailable) return true;
                }
            }

            return false;
        }

        private void UseWrathAbility(object sender, EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " can perform a bonus attack against a different target");

                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartSelectAttackTarget
                (
                    HostShip,
                    FinishAdditionalAttack,
                    IsAnotherShip,
                    HostShip.PilotInfo.PilotName,
                    "You may perform a bonus attack against a different target",
                    HostShip
                );
            }
            else
            {
                Messages.ShowErrorToHuman(HostShip.PilotInfo.PilotName + " cannot perform a second Cluster Missiles attack");
                Triggers.FinishTrigger();
            }
        }

        private bool IsAnotherShip(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = true;
            string TargetingFailure = "";

            if (defender.ShipId == OriginalDefender.ShipId)
            {
                result = false;
                TargetingFailure = "The attack cannot be performed. You must choose a different target.";
            }

            if (result == false && !isSilent) Messages.ShowErrorToHuman(TargetingFailure);

            return result;
        }

        private void FinishAdditionalAttack()
        {
            // If attack is skipped, set this flag, otherwise regular attack can be performed second time
            Selection.ThisShip.IsAttackPerformed = true;

            //if bonus attack was skipped, allow bonus attacks again
            if (Selection.ThisShip.IsAttackSkipped) Selection.ThisShip.IsCannotAttackSecondTime = false;

            Triggers.FinishTrigger();
        }
    }
}
