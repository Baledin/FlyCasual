using ActionsList;
using Actions;
using System.Collections.Generic;
using Upgrade;
using Movement;
using BoardTools;
using System;

namespace UpgradesList.SecondEdition
{
    public class Reckless : GenericUpgrade
    {
        public Reckless()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "Reckless",
                UpgradeType.Talent,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.RecklessAbility)
            );

            IsHidden = true;
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you perform a red boost or red barrel-roll action, you may use the [1 left turn] or [1 right turn] template instead.
    //If you do, roll an attack die. On a hit/crit result, gain a stress token.
    public class RecklessAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBoostTemplates += ChangeBoostTemplates;
            HostShip.OnGetAvailableBarrelRollActionTemplates += ChangeBarrelRollTemplates;
            HostShip.OnActionIsPerformed += CheckCost;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBoostTemplates -= ChangeBoostTemplates;
            HostShip.OnGetAvailableBarrelRollActionTemplates -= ChangeBarrelRollTemplates;
            HostShip.OnActionIsPerformed -= CheckCost;
        }

        private void CheckCost(GenericAction action)
        {
            List<String> diceRollManeuvers = new List<string>() {"Turn 1 Right", "Turn 1 Left"};
            if ((action is BoostAction && diceRollManeuvers.Contains((action as BoostAction).SelectedBoostTemplate)) ||
                (action is BarrelRollAction && diceRollManeuvers.Contains((action as BarrelRollAction).SelectedTemplate.Name)))
            {
                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": if you use the 1 Left or 1 Right Turn template, roll an attack die. On a hit or crit result, gain a stress token.");
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, RollDiceForStress);
            }
        }

        private void RollDiceForStress(object sender, EventArgs e)
        {
            PerformDiceCheck(
                HostUpgrade.UpgradeInfo.Name,
                DiceKind.Attack,
                1,
                FinishAction,
                Triggers.FinishTrigger
            );
        }

        private void FinishAction()
        {
            if (DiceCheckRoll.RegularSuccesses > 0)
            {
                HostShip.Tokens.AssignToken(typeof(Tokens.StressToken), AbilityDiceCheck.ConfirmCheck);
            }
            else
            {
                AbilityDiceCheck.ConfirmCheck();
            }
        }

        private void ChangeBoostTemplates(List<BoostMove> availableMoves, GenericAction action)
        {
            if (action.Color == ActionColor.Red)
            {
                availableMoves.Add(new BoostMove(ActionsHolder.BoostTemplates.LeftTurn1, isRed: true));
                availableMoves.Add(new BoostMove(ActionsHolder.BoostTemplates.RightTurn1, isRed: true));
            }
        }

        private void ChangeBarrelRollTemplates(List<ManeuverTemplate> availableTemplates, GenericAction action)
        {
            if (action.Color == ActionColor.Red)
            {
                availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Turn, ManeuverDirection.Left, ManeuverSpeed.Speed1));
                availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Turn, ManeuverDirection.Right, ManeuverSpeed.Speed1));
            }
        }
    } 
}