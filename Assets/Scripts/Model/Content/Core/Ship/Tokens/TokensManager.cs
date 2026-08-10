using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;

namespace Ship
{
    public class TokensManager
    {
        private readonly ITargetLockable Host;
        private readonly List<GenericToken> AssignedTokens = new ();
        public GenericToken TokenToAssign;

        public TokensManager(ITargetLockable host)
        {
            Host = host;
        }

        public List<GenericToken> GetAllTokens()
        {
            return AssignedTokens;
        }

        public List<GenericToken> GetTokensByColor(params TokenColors[] colors)
        {
            return AssignedTokens.Where(n => colors.Contains(n.TokenColor)).ToList();
        }

        public List<GenericToken> GetNonLockRedOrangeTokens()
        {
            return AssignedTokens.Where(n => (n.TokenColor == TokenColors.Red || n.TokenColor == TokenColors.Orange) && n.GetType().BaseType != typeof(GenericTargetLockToken)).ToList();
        }

        public List<GenericToken> GetNonLockRedTokens()
        {
            return AssignedTokens.Where(n => (n.TokenColor == TokenColors.Red) && n.GetType().BaseType != typeof(GenericTargetLockToken)).ToList();
        }

        public List<GenericToken> GetNonStressRedOrangeTokens()
        {
            return AssignedTokens.Where(n => (n.TokenColor == TokenColors.Red || n.TokenColor == TokenColors.Orange) && n.GetType() != typeof(StressToken)).ToList();
        }

        public List<GenericToken> GetTokensByShape(TokenShapes shape)
        {
            return AssignedTokens.Where(n => n.TokenShape == shape).ToList();
        }

        public bool HasGreenTokens => HasTokenByColor(TokenColors.Green);

        public bool HasTokenByColor(TokenColors tokensColor)
        {
            foreach (GenericToken tok in AssignedTokens)
            {
                if (tok.TokenColor == tokensColor)
                    return true;
            }

            return false;
        }

        public bool HasToken(Type type, char letter = ' ')
        {
            return GetToken(type, letter) != null;
        }

        public int CountTokensByType(Type type)
        {
            return GetAllTokens().Count(n => n.GetType() == type);
        }

        public int CountTokensByShape(TokenShapes shape)
        {
            return GetAllTokens().Count(n => n.TokenShape == shape);
        }

        public int CountTokensByColor(TokenColors color)
        {
            return GetAllTokens().Count(n => n.TokenColor == color);
        }

        public bool HasToken<T>(char letter = ' ') where T : GenericToken
        {
            return GetToken<T>(letter) != null;
        }

        public int CountTokensByType<T>() where T : GenericToken
        {
            return GetAllTokens().Count(n => n is T);
        }

        public GenericToken GetToken(Type type, char letter = ' ')
        {
            GenericToken result = null;

            foreach (GenericToken assignedToken in AssignedTokens)
            {
                if (type.IsAssignableFrom(assignedToken.GetType()))
                {
                    if (assignedToken is GenericTargetLockToken)
                    {
                        if ((assignedToken as GenericTargetLockToken).Letter == letter || letter == '*')
                        {
                            return assignedToken;
                        }
                    }
                    else
                    {
                        return assignedToken;
                    }
                }
            }

            return result;
        }

        public T GetToken<T>(char letter = ' ') where T : GenericToken
        {
            T result = AssignedTokens
                .OfType<T>()
                .Where(t => t is not GenericTargetLockToken || letter == '*' || (t as GenericTargetLockToken).Letter == letter)
                .FirstOrDefault();
            return result;
        }

        public List<T> GetTokens<T>(char letter = ' ') where T : GenericToken
        {
            List<T> result = AssignedTokens
                .OfType<T>()
                .Where(t => t is not GenericTargetLockToken || letter == '*' || (t as GenericTargetLockToken).Letter == letter)
                .ToList();
            return result;
        }

        public GenericTargetLockToken GetTargetLockToken(char letter)
        {
            return (GenericTargetLockToken)AssignedTokens.Find(n => n.GetType().BaseType == typeof(GenericTargetLockToken) && (n as GenericTargetLockToken).Letter == letter);
        }

        public List<char> GetTargetLockLetterPairsOn(ITargetLockable targetShip)
        {
            List<char> result = new();

            List<BlueTargetLockToken> blueTokens = GetTokens<BlueTargetLockToken>('*');

            foreach (BlueTargetLockToken blueToken in blueTokens)
            {
                char foundLetter = (blueToken as BlueTargetLockToken).Letter;

                GenericToken redToken = targetShip.GetAnotherToken(typeof(RedTargetLockToken), foundLetter);
                if (redToken != null)
                {
                    result.Add(blueToken.Letter);
                }
            }

            return result;
        }

        public void AssignToken(GenericToken token, Action callBack)
        {
            TokenToAssign = token;

            if(Host is GenericShip)
            {
                (Host as GenericShip).CallBeforeAssignToken(
                    TokenToAssign,
                    delegate { FinalizeAssignToken(callBack); }
                );
            }
        }

        public void AssignToken(Type tokenType, Action callback, Players.GenericPlayer assigner = null)
        {
            if (tokenType == typeof(JamToken) || tokenType == typeof(TractorBeamToken))
            {
                if (assigner == null)
                    throw new InvalidOperationException("assigner must be specified when assigning a " + tokenType.ToString());

                AssignToken((GenericToken)Activator.CreateInstance(tokenType, Host, assigner), callback);
            }
            else
                AssignToken((GenericToken)Activator.CreateInstance(tokenType, Host), callback);
        }

        public void AssignTokens(Func<GenericToken> createToken, int count, Action callback, char letter = ' ')
        {
            if (count > 0)
            {
                count--;
                AssignToken(createToken(), delegate { AssignTokens(createToken, count, callback, letter); });
            }
            else
            {
                callback();
            }
        }

        private void FinalizeAssignToken(Action callback)
        {
            if (TokenToAssign == null)
            {
                callback();
                return;
            }

            AssignedTokens.Add(TokenToAssign);

            TokenToAssign.InitializeTooltip();
            TokenToAssign.WhenAssigned();

            if(Host is GenericShip)
            {
                (Host as GenericShip).CallOnTokenIsAssigned(TokenToAssign, callback);
            }
        }

        public void RemoveCondition(GenericToken token)
        {
            if (AssignedTokens.Remove(token))
            {
                token.WhenRemoved();

                if (Host is GenericShip)
                {
                    (Host as GenericShip).CallOnConditionIsRemoved(token);
                }
            }
        }

        public void RemoveToken(Type type, Action callback, char letter = ' ')
        {
            GenericToken assignedToken = GetToken(type, letter);

            if (assignedToken == null)
            {
                callback();
            }
            else
            {
                RemoveToken(assignedToken, callback);
            }
        }

        public void RemoveToken(GenericToken tokenToRemove, Action callback)
        {
            if (Host is not GenericShip || (Host as GenericShip).CanRemoveToken(tokenToRemove))
            {
                AssignedTokens.Remove(tokenToRemove);

                if (tokenToRemove.GetType().BaseType == typeof(GenericTargetLockToken))
                {
                    ITargetLockable otherTokenOwner = (tokenToRemove as GenericTargetLockToken).OtherTargetLockTokenOwner;
                    ActionsHolder.ReleaseTargetLockLetter((tokenToRemove as GenericTargetLockToken).Letter);
                    Type oppositeType = (tokenToRemove.GetType() == typeof(BlueTargetLockToken)) ? typeof(RedTargetLockToken) : typeof(BlueTargetLockToken);

                    char letter = (tokenToRemove as GenericTargetLockToken).Letter;
                    GenericToken otherTargetLockToken = otherTokenOwner.GetAnotherToken(oppositeType, letter);

                    if (otherTargetLockToken != null)
                    {
                        otherTokenOwner.RemoveToken(otherTargetLockToken);

                        if (otherTokenOwner is GenericShip)
                        {
                            (otherTokenOwner as GenericShip).CallOnRemoveTokenEvent(otherTargetLockToken);
                        }
                    }
                }

                tokenToRemove.WhenRemoved();

                if(Host is GenericShip)
                {
                    (Host as GenericShip).CallOnRemoveTokenEvent(tokenToRemove);
                }
            }

            Triggers.ResolveTriggers(TriggerTypes.OnTokenIsRemoved, callback);
        }

        public void RemoveAllTokensByType(Type tokenType, Action callback, char letter = '*')
        {
            GenericToken tokenToRemove = GetToken(tokenType, letter);
            if (tokenToRemove != null)
            {
                RemoveToken(
                    tokenType,
                    delegate { RemoveAllTokensByType(tokenType, callback, letter); },
                    letter: '*'
                );
            }
            else
            {
                callback();
            }
        }

        public void RemoveTokens(List<GenericToken> tokensToRemove, Action callback)
        {
            if (tokensToRemove != null && tokensToRemove.Count != 0)
            {
                GenericToken tokenToRemove = tokensToRemove.First();
                tokensToRemove.Remove(tokenToRemove);

                RemoveToken(
                    tokenToRemove,
                    delegate { RemoveTokens(tokensToRemove, callback); }
                );
            }
            else
            {
                callback();
            }
        }

        public void RemoveAllTokensByColor(TokenColors color, Action callback)
        {
            GenericToken tokenToRemove = AssignedTokens.FirstOrDefault(token => token.TokenColor == color);
            if (tokenToRemove != null)
            {
                RemoveToken(
                    tokenToRemove,
                    delegate { RemoveAllTokensByColor(color, callback); }
                );
            }
            else
            {
                callback();
            }
        }

        public void SpendToken(GenericToken assignedToken, Action callback)
        {
            if (assignedToken != null)
            {
                RemoveToken(
                    assignedToken,
                    delegate 
                    {
                        if(Host is GenericShip)
                        {
                            (Host as GenericShip).CallFinishSpendToken(assignedToken, callback);
                        }
                        else
                        {
                            callback();
                        }
                    }
                );
            }
            else
            {
                callback();
            }
        }

        public void SpendToken(Type type, Action callback, char letter = ' ')
        {
            SpendToken(GetToken(type, letter), callback);
        }

        public void TransferToken(Type tokenType, ITargetLockable targetShip, Action callback, Players.GenericPlayer assigner = null)
        {
            Host.GetTokens().RemoveToken(
                tokenType,
                () => targetShip.GetTokens().AssignToken(tokenType, callback, assigner)
            );
        }

        public void TransferToken(GenericToken token, ITargetLockable targetShip, Action callback)
        {
            Host.GetTokens().RemoveToken(
                token,
                () => targetShip.GetTokens().AssignToken(token, callback)
            );
        }

        // CONDITIONS - don't trigger any abilities

        public void RemoveCondition(Type type)
        {
            GenericToken assignedCondition = GetToken(type);
            RemoveCondition(assignedCondition);
        }

        public void AssignCondition(GenericToken token)
        {
            AssignedTokens.Add(token);

            token.InitializeTooltip();
            token.WhenAssigned();

            if(Host is GenericShip)
            {
                (Host as GenericShip).CallOnConditionIsAssigned(token);
            }
        }

        public void AssignCondition(Type tokenType)
        {
            GenericToken token = (GenericToken)Activator.CreateInstance(tokenType, Host);
            AssignCondition(token);
        }

        public static TokenColors GetTokenColorByType(Type tokenType)
        {
            GenericToken token;

            if (tokenType != typeof(TractorBeamToken) && tokenType != typeof(JamToken))
            {
                token = (GenericToken)Activator.CreateInstance(tokenType, Roster.AllUnits.First().Value);
            }
            else
            {
                token = (GenericToken)Activator.CreateInstance(tokenType, Roster.AllUnits.First().Value, Roster.Player1);
            }

            return token.TokenColor;
        }
    }
}