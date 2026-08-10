using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace Obstacles
{
    public abstract class GenericObstacle : ITargetLockable, IBoardObject
    {
        public string Name { get; set; }
        public string ShortName { get; protected set; }
        public bool IsPlaced { get; set; }
        public GameObject ObstacleGO { get; set; }

        public TokensManager Tokens { get; protected set; }

        private MeshCollider collider;
        public MeshCollider Collider => collider;
        public BoardObjectType BoardObjectType => BoardObjectType.Obstacle;

        public GenericObstacle(string name, string shortName)
        {
            Name = name;
            ShortName = shortName;
            Tokens = new(this);
        }

        public abstract string GetTypeName { get; }

        public abstract void OnHit(GenericShip ship);

        public void OnLanded(GenericShip ship)
        {
            if (Editions.Edition.Current.RuleSet is Editions.RuleSets.RuleSet25 || this is Asteroid)
            {
                ship.OnTryPerformAttack += DenyAttack;
            }

            if (Editions.Edition.Current.RuleSet is Editions.RuleSets.RuleSet25 && !Selection.ThisShip.IsIgnoreObstacles)
            {
                Messages.ShowErrorToHuman(ship.PilotInfo.PilotName + " landed on an obstacle during movement, their action subphase is skipped");
                Selection.ThisShip.IsSkipsActionSubPhase = true;
            }
        }

        public virtual void OnShotObstructedExtra(GenericShip attacker, GenericShip defender, ref int result)
        {
            // Does nothing by default
        }

        public void Spawn(string name, Transform obstacleHolder)
        {
            GameObject obstacleModelPrefab = Resources.Load<GameObject>(string.Format("Prefabs/Obstacles/{0}/{1}", GetTypeName, Name));
            ObstacleGO = GameObject.Instantiate<GameObject>(obstacleModelPrefab, obstacleHolder);
            collider = ObstacleGO.GetComponentInChildren<MeshCollider>();
            Name = name;
            ObstacleGO.name = name;
            ObstacleGO.transform.Find("default").name = name;
            Board.RegisterObstacle(this);
        }


        // ITargetLockable
        public int GetRangeToShip(GenericShip fromShip)
        {
            ShipObstacleDistance dist = new(fromShip, this);
            return dist.Range;
        }

        public void AssignToken(RedTargetLockToken token, Action callback)
        {
            //Tokens.Add(token);
            Tokens.AssignToken(token, callback);
            callback();
        }

        public List<char> GetTargetLockLetterPairsOn(ITargetLockable targetShip)
        {
            return Tokens.GetTargetLockLetterPairsOn(targetShip);
        }

        public GenericTargetLockToken GetAnotherToken(Type oppositeType, char letter)
        {
            return Tokens.GetToken(oppositeType, letter) as GenericTargetLockToken;
        }

        public void RemoveToken(GenericToken token)
        {
            Tokens.GetAllTokens().Remove(token);
        }

        public abstract void AfterObstacleRoll(GenericShip ship, DieSide side, Action callback);

        private void DenyAttack(ref bool result, List<string> stringList)
        {
            if (Selection.ThisShip.ObstaclesLanded.Contains(this) && !Selection.ThisShip.CanAttackWhileLandedOnObstacle())
            {
                result = false;
                Selection.ThisShip.CallCheckObstacleDenyAttack(this, ref result);
                if (!result) stringList.Add(Selection.ThisShip.PilotInfo.PilotName + " landed on an obstacle and cannot attack");
            }
        }

        public TokensManager GetTokens()
        {
            return this.Tokens;
        }
    }
}