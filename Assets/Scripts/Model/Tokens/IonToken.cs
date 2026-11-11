using Editions;
using Ship;
using System.Linq;

namespace Tokens
{
    public class IonToken : GenericToken
    {
        public IonToken(GenericShip host) : base(host)
        {
            Name = ImageName = "Ion Token";
            Temporary = false;
            TokenColor = TokenColors.Red;
            TokenShape = TokenShapes.Square;
            PriorityUI = 35;
        }

        public override void WhenAssigned()
        {
            if (IsIonized())
            {
                Messages.ShowInfo("This ship is ionized!");
                Host.ToggleIonized(true);
            }
        }

        public bool IsIonized()
        {
            int ionTokensCount = Host.Tokens.GetAllTokens().Count(n => n is IonToken);
            return (ionTokensCount >= Edition.Current.NegativeTokensToAffectShip[Host.ShipInfo.BaseSize]);
        }

        public override void WhenRemoved()
        {
            if (!IsIonized())
            {
                Host.ToggleIonized(false);
            }
        }
    }
}