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
                && (Combat.ShotInfo.InArcByType(Arcs.ArcType.Rear) || Combat.ShotInfo.InArcByType(Arcs.ArcType.Front))
                && (Combat.Defender.Tokens.HasTokenByColor(Tokens.TokenColors.Red) || Combat.Defender.Tokens.HasTokenByColor(Tokens.TokenColors.Orange));
                
        }

        private void CanPerformTargetLock(ref bool result, GenericShip ship, ITargetLockable defender)
        {
            //Todo change to account for obstacles as well
            if (!Tools.IsSameShip(ship, HostShip)) return;

            if (defender is GenericShip)
            {
                result = BoardTools.Board.IsShipInArcByType(HostShip, defender as GenericShip, Arcs.ArcType.Front) || BoardTools.Board.IsShipInArcByType(HostShip, defender as GenericShip, Arcs.ArcType.Rear);
            }

            return;
        }
    }
}

namespace SubPhases
{
    public class GarSaxonGunnerAbilityDecisionSubPhase : RemoveBadTokenFromDefenderDecisionSubPhase { }
}