using Editions;
using Ship;

namespace Tokens
{
    public class TractorBeamToken : GenericToken
    {
        public Players.GenericPlayer Assigner;

        public TractorBeamToken(GenericShip host, Players.GenericPlayer assigner) : base(host)
        {
            Name = "Tractor Beam Token";
            ImageName = (Edition.Current is SecondEdition) ? "Tractor Beam Token SE" : "Tractor Beam Token FE";
            Temporary = true;
            TokenColor = TokenColors.Orange;
            TokenShape = TokenShapes.Cirular;
            PriorityUI = 40;
            Assigner = assigner;
        }
    }
}