using Upgrade;
using Ship;
using System;
using Actions;
using ActionsList;
using Content;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class ClanTraining : GenericUpgrade
    {
        public ClanTraining()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "Clan Training",
                UpgradeType.Talent,
                cost: 3,
                restriction: new TagRestriction(Tags.Mandalorian),
                abilityType: typeof(Abilities.SecondEdition.ClanTrainingAbility),
                charges: 1
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ClanTrainingAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCombatActivation += CheckAbility;
            HostShip.OnAttackFinishAsAttacker += CheckKillAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatActivation -= CheckAbility;
            HostShip.OnAttackFinishAsAttacker -= CheckKillAbility;
        }

        private void CheckKillAbility(GenericShip ship)
        {
            if (Combat.Defender.IsDestroyed) RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, RecoverCharge);
        }

        private void RecoverCharge(object sender, EventArgs e)
        {
            Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name} recovers 1 charge");
            HostUpgrade.State.RestoreCharge();
            Triggers.FinishTrigger();
        }

        private void CheckAbility(GenericShip ship)
        {
            if (HostUpgrade.State.Charges > 0 && !HostShip.Tokens.HasToken(typeof(FocusToken)) && 
                BoardTools.Board.GetShipsInArcAtRange(HostShip, Arcs.ArcType.Front, new UnityEngine.Vector2(1,1), Team.Type.Enemy).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, UseAbility);
            }            
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            HostShip.BeforeActionIsPerformed += RegisterSpendChargeTrigger;
            HostShip.AskPerformFreeAction(
                new FocusAction() { Color = ActionColor.Red },
                CleanUp,
                HostUpgrade.UpgradeInfo.Name,
                "Before you engage, if you are not focused and there is an enemy ship in your Front Arc at range 1, you may spend 1 Charge to perform a red Focus action.",
                HostUpgrade
            );
        }

        private void RegisterSpendChargeTrigger(GenericAction action, ref bool isFreeAction)
        {
            HostShip.BeforeActionIsPerformed -= RegisterSpendChargeTrigger;
            RegisterAbilityTrigger(
                TriggerTypes.OnFreeAction,
                delegate {
                    HostUpgrade.State.SpendCharge();
                    Triggers.FinishTrigger();
                }
            );
        }

        private void CleanUp()
        {
            HostShip.BeforeActionIsPerformed -= RegisterSpendChargeTrigger;
            Triggers.FinishTrigger();
        }
    }
}