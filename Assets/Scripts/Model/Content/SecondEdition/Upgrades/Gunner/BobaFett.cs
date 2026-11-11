using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class BobaFettGunner : GenericUpgrade
    {
        public BobaFettGunner() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Boba Fett",
                UpgradeType.Gunner,
                cost: 2,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum, Faction.Separatists),
                abilityType: typeof(Abilities.SecondEdition.BobaFettGunnerAbility),
                legalityInfo: new() { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            NameCanonical = "bobafett-gunner";

            ImageUrl = "Upgrades/bobafett";

            Avatar = new AvatarInfo(
                Faction.Scum,
                new Vector2(233, 12)
            );
        }
    }

    public class BobaFettGunnerXWA : BobaFettGunner
    {
        public BobaFettGunnerXWA() : base()
        {
            UpgradeInfo.Cost = 4;
            UpgradeInfo.LegalityInfo = new() { Legality.XWA };
            NameCanonical = "bobafett";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BobaFettGunnerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Boba Fett",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Focus },
                sideCanBeChangedTo: DieSide.Success
            );
        }

        private bool IsAvailable()
        {
            int shipsInAttackArc = 0;
            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                ShotInfoArc shotInfoArc = new ShotInfoArc(HostShip, ship, Combat.ArcForShot);
                if (shotInfoArc.InArc) shipsInAttackArc++;
            }

            return Combat.AttackStep == CombatStep.Attack
                && shipsInAttackArc == 1
                && Combat.DiceRollAttack.HasResult(DieSide.Focus);
        }

        private int GetAiPriority()
        {
            return 55;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}