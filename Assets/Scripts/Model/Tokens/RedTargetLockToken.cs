using Ship;

namespace Tokens
{
    public class RedTargetLockToken : GenericTargetLockToken
    {
        public ITargetLockable HostTargetable { get; private set; }

        public RedTargetLockToken(ITargetLockable host) : base(null)
        {
            Name = ImageName = "Red Target Lock Token";
            TokenColor = TokenColors.Red;
            TokenShape = TokenShapes.Square;
            PriorityUI = 20;

            HostTargetable = host;
            if (host is GenericShip) Host = host as GenericShip;
        }
    }
}