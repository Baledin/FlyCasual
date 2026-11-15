using Arcs;
using Ship;
using SubPhases;
using System;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class GarSaxonGunner : GenericUpgrade
    {
        public GarSaxonGunner() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Gar Saxon",
                UpgradeType.Gunner,
                cost: 6,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new BaseSizeRestriction(BaseSize.Large),
                    new FactionRestriction(Faction.Scum)
                ),
                abilityType: typeof(Abilities.SecondEdition.GarSaxonGunner)
            );

            NameCanonical = "garsaxon-gunner";

            ImageUrl = "Upgrades/garsaxon";
        }

        public class GarSaxonGunnerXWA : GarSaxonGunner
        {
            public GarSaxonGunnerXWA() : base()
            {
                NameCanonical = "garsaxon";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GarSaxonGunner : GenericAbility
    {
        // When you perform the Lock action, you can only choose an object in your Front Arc or Rear Arc.
        // While you perform a primary attack, if the defender is in your Front Arc or Rear Arc, you may remove 1 orange or red token from the defender to roll an additional die, to a maximum of 4. 

        public override void ActivateAbility()
        {
            RulesList.TargetLocksRule.OnCheckTargetLockIsDisallowed += CanPerformTargetLock;
            HostShip.OnShotStartAsAttacker += CheckAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            RulesList.TargetLocksRule.OnCheckTargetLockIsDisallowed -= CanPerformTargetLock;
            HostShip.OnShotStartAsAttacker -= CheckAbilityTrigger;
        }

        private void CheckAbilityTrigger()
        {
            if (IsGarSaxonAbilityAvailable())
            {
                RegisterAbilityTrigger(TriggerTypes.OnShotStart, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {

            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                UseAbilityDecision,
                descriptionLong: "Do you want to remove 1 orange or red token from the defender to roll 1 additional attack die to a maximum of 4?",
                imageHolder: HostUpgrade
            );
        }

        private void UseAbilityDecision(object sender, EventArgs e)
        {
            if (IsGarSaxonAbilityAvailable())
            {
                GarSaxonGunnerAbilityDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<GarSaxonGunnerAbilityDecisionSubPhase>(
                    "Gar Saxon: Select token to remove",
                    delegate { DecisionSubPhase.ConfirmDecision(); }
                );

                subphase.DescriptionShort = HostUpgrade.UpgradeInfo.Name;
                subphase.DescriptionLong = "Gar Saxon: Select 1 orange or red token to remove from the defender";
                subphase.ImageSource = HostUpgrade;

                subphase.DecisionOwner = HostShip.Owner;
                subphase.Start();
                AllowRollAdditionalDie();
            }
        }

        private void AllowRollAdditionalDie()
        {
            HostShip.AfterGotNumberOfAttackDice += RollExtraDie;
        }

        protected void RollExtraDie(ref int diceCount)
        {
            HostShip.AfterGotNumberOfAttackDice -= RollExtraDie;
            if (diceCount >= 4)
            {
                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": Cannot roll more than 4 dice.");
            }
            else
            {
                Messages.ShowInfo(HostName + ": +1 attack die");
                diceCount++;
            }
        }

        private bool IsGarSaxonAbilityAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker == HostShip
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                && IsInFrontOrRearArc(Combat.Defender)
                && (Combat.Defender.Tokens.HasTokenByColor(Tokens.TokenColors.Red) || Combat.Defender.Tokens.HasTokenByColor(Tokens.TokenColors.Orange));
        }

        private void CanPerformTargetLock(ref bool result, GenericShip ship, ITargetLockable defender)
        {
            //Todo change to account for obstacles as well
            if (!Tools.IsSameShip(ship, HostShip)) return;

            if (defender is GenericShip)
            {
                result = IsInFrontOrRearArc(defender as GenericShip);
            }

            return;
        }

        private bool IsInFrontOrRearArc(GenericShip target)
        {
            return HostShip.SectorsInfo.GetSectorInfo(target, ArcType.Front).InArc || HostShip.SectorsInfo.GetSectorInfo(target, ArcType.Rear).InArc;
        }
    }
}

namespace SubPhases
{
    public class GarSaxonGunnerAbilityDecisionSubPhase : RemoveBadTokenFromDefenderDecisionSubPhase { }
}