using Ship;

namespace Tokens
{
    public class CritToken : GenericToken
    {
        public CritToken(GenericShip host) : base(host)
        {
            Name = ImageName = "Critical Hit Token";
            Temporary = false;
            PriorityUI = 33;
        }
    }
}