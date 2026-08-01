using Ship;
using Tokens;

namespace RulesList
{
    public class RemotesRule
    {
        public void AllowOnlyLocksAndCharges(GenericShip ship, GenericToken token)
        {
            if (token is not RedTargetLockToken && token is not ChargeToken)
            {
                // Message doesn't cover Charges as those should never be assigned during normal gameplay, message is for players trying to assign Focus, Calculate, etc.
                Messages.ShowInfo("Remotes cannot be assigned tokens except for locks.");
                ship.Tokens.TokenToAssign = null;
            }
        }
    }
}