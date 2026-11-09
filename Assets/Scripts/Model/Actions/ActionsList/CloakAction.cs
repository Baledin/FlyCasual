using Tokens;

namespace ActionsList
{
    public class CloakAction : GenericAction
    {
        public CloakAction()
        {
            Name = "Cloak";
            ImageUrl = "Reference/CloakAction";
        }

        public override void ActionTake()
        {
            Selection.ThisShip.Tokens.AssignToken(typeof(CloakToken), Phases.CurrentSubPhase.CallBack);
        }

        public override bool IsActionAvailable()
        {
            return !Selection.ThisShip.Tokens.HasToken(typeof(CloakToken));
        }

        public override void RevertActionOnFail(bool hasSecondChance = false)
        {
            Phases.GoBack();
        }
    }
}