using Arcs;
using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.RZ2AWing
{
    public class RonithBlario : RZ2AWing
    {
        public RonithBlario() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Ronith Blario",
                "Reckless Rookie",
                Faction.Resistance,
                2,
                4,
                10,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.RonithBlarioAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Missile
                },
                tags: new List<Tags>
                {
                    Tags.AWing
                },
                skinName: "Red",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class RonithBlarioXWA : RonithBlario
    {
        public RonithBlarioXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 8;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 5;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Missile
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you defend or perform an attack, if the enemy ship is in another friendly ship's turret arc, 
    //you may spend 1 focus token from that ship to change 1 of your focus results to an evade or hit result.
    public class RonithBlarioAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Change,
                1,
                new List<DieSide> { DieSide.Focus },
                DieSide.Success,
                payAbilityCost: PayAbilityCost
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private int GetDiceModificationAiPriority()
        {
            return 40;
        }

        private GenericShip EnemyShip => Combat.Attacker == HostShip ? Combat.Defender : Combat.Attacker;

        private bool IsDiceModificationAvailable()
        {
            if (Combat.CurrentDiceRoll.Focuses == 0)
                return false;

            if ((Combat.AttackStep == CombatStep.Attack) || (Combat.AttackStep == CombatStep.Defence))
            {
                return GetFocusedShipsWithEnemyInTurretArc(EnemyShip).Any();
            }

            return false;
        }

        private List<GenericShip> GetFocusedShipsWithEnemyInTurretArc(GenericShip enemyShip)
        {
            return Board
                .GetShipsAtRange(HostShip, new UnityEngine.Vector2(0, int.MaxValue), Team.Type.Friendly)
                .Where(ship => ship != HostShip && ship.Tokens.HasToken<FocusToken>() && HasEnemyInTurretArc(ship, enemyShip))
                .ToList();
        }

        private bool HasEnemyInTurretArc(GenericShip ship, GenericShip enemyShip)
        {
            IEnumerable<GenericArc> turretArcs = ship.ArcsInfo.Arcs.Where(arc => arc is ArcSingleTurret || arc is ArcDualTurretA || arc is ArcDualTurretB);
            return turretArcs.Any(arc => new ShotInfoArc(ship, enemyShip, arc).InArc);
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            SelectTargetForAbility(
                () => SpendToken(callback),
                FilterAbilityTarget,
                GetAiAbilityPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Choose a friendly ship to spend 1 focus from",
                HostShip,
                callback: () => callback(false));
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;

            int shipFocusTokens = ship.Tokens.CountTokensByType(typeof(FocusToken));

            result += shipFocusTokens * 1000;

            if (ship.IsActivatedDuringCombat)
                result += 100;

            result -= (ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost));

            return result;
        }

        private bool FilterAbilityTarget(GenericShip ship)
        {
            return
                FilterByTargetType(ship, TargetTypes.OtherFriendly) &&
                ship.Tokens.HasToken<FocusToken>() &&
                HasEnemyInTurretArc(ship, EnemyShip);
        }

        private void SpendToken(Action<bool> callback)
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            if (TargetShip.Tokens.HasToken<FocusToken>())
            {
                TargetShip.Tokens.SpendToken(typeof(FocusToken), () => callback(true));
            }
            else
            {
                callback(false);
            }
        }
    }
}