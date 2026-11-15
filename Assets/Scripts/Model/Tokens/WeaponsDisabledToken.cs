using Editions;
using Ship;

namespace Tokens
{
    public class WeaponsDisabledToken : GenericToken
    {
        public WeaponsDisabledToken(GenericShip host) : base(host)
        {
            Name = ImageName = (Edition.Current is SecondEdition) ? "Disarm Token" : "Weapons Disabled Token";
            Temporary = true;
            PriorityUI = 45;
            TokenColor = TokenColors.Orange;
            TokenShape = TokenShapes.Cirular;
        }
    }
}