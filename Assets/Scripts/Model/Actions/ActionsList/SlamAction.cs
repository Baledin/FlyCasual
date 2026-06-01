using BoardTools;
using Movement;
using System.Collections.Generic;
using System.Linq;
using Tokens;

namespace ActionsList
{

    public class SlamAction : GenericAction
    {
        private bool canBePerformedAsFreeAction = false;
        public override bool CanBePerformedAsAFreeAction { get { return canBePerformedAsFreeAction; } }
        private List<ManeuverHolder> allowedManeuverTemplates;

        public SlamAction()
        {
            Name = DiceModificationName = "SLAM";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/SlamAction.png";
        }

        public SlamAction(bool canBePerformedAsFreeAction) : this()
        {
            this.canBePerformedAsFreeAction = canBePerformedAsFreeAction;
        }

        public override void ActionTake()
        {
            if (Selection.ThisShip.Owner.UsesHotacAiRules)
            {
                Phases.CurrentSubPhase.CallBack();
            }
            else
            {
                Phases.CurrentSubPhase.Pause();

                allowedManeuverTemplates = Selection.ThisShip.GetAvailableSlamTemplates(this);

                Selection.ThisShip.Owner.SelectManeuverFromList(
                    ShipMovementScript.SendAssignManeuverCommand,
                    ExecuteSelectedManeuver,
                    allowedManeuverTemplates.ToDictionary(a=>a.ToString(),a=>a.ColorComplexity)
                );
            }
        }

        private void ExecuteSelectedManeuver()
        {
            Selection.ThisShip.AssignedManeuver.IsRevealDial = false;

            Selection.ThisShip.CallUpdateChosenSlamTemplate(Selection.ThisShip.AssignedManeuver);

            ShipMovementScript.LaunchMovement(AssignWeaponsDisabledToken);
        }

        private void AssignWeaponsDisabledToken()
        {
            Selection.ThisShip.Tokens.AssignToken(typeof(WeaponsDisabledToken), FinishSlam);
        }

        private void FinishSlam()
        {
            Selection.ThisShip.CallSlam(Phases.CurrentSubPhase.CallBack);
        }

        private void PerformSlamManeuver(object sender, System.EventArgs e)
        {
            Selection.ThisShip.AssignedManeuver.Perform();
        }

        public override int GetActionPriority()
        {
            int result = 0;
            return result;
        }

    }

}