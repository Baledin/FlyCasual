using Editions;
using Ship;

namespace Tokens
{
    public class JamToken : GenericToken
    {
        public Players.GenericPlayer Assigner;

        public JamToken(GenericShip host, Players.GenericPlayer assigner) : base(host)
        {
            Name = "Jam Token";
            ImageName = (Edition.Current is SecondEdition) ? "Jam Token SE" : "Jam Token FE";
            Temporary = Edition.Current is SecondEdition;
            TokenColor = TokenColors.Orange;
            TokenShape = TokenShapes.Cirular;
            PriorityUI = 40;
            Assigner = assigner;
        }
    }
}