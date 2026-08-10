using ActionsList;
using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class TA175 : GenericUpgrade
    {
        public TA175() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "TA-175",
                UpgradeType.TacticalRelay,
                cost: 6,
                isLimited: true,
                isSolitary: true,
                restriction: new FactionRestriction(Faction.Separatists),
                abilityType: typeof(Abilities.SecondEdition.TA175Ability),
                legalityInfo: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            Avatar = new AvatarInfo(
                Faction.Separatists,
                new Vector2(275, 21)
            );
        }
    }

    public class TA175XWA : TA175
    {
        public TA175XWA() : base()
        {
            UpgradeInfo.Cost = 5;
            UpgradeInfo.LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TA175Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, bool isFled)
        {

            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo
                && ship.ActionBar.PrintedActions.Any(n => n is CalculateAction)
                && Board.CheckInRange(HostShip, ship, 0, 3, RangeCheckReason.UpgradeCard))
            {
                RegisterAbilityTrigger(
                    TriggerTypes.OnShipIsDestroyed,
                    delegate { AssignCalculateTokens(ship); }
                );
            }
        }

        private void AssignCalculateTokens(GenericShip destroyedShips)
        {
            List<GenericShip> friendlyShipsInRange = Board.GetShipsAtRange(destroyedShips, new Vector2(0, 3), Team.Type.Friendly)
                .Where(n => n.Owner.PlayerNo == destroyedShips.Owner.PlayerNo)
                .Where(n => n.ShipId != destroyedShips.ShipId)
                .Where(n => n.ActionBar.PrintedActions.Any(a => a is CalculateAction))
                .ToList();

            AssignCalculateTokensRecursive(friendlyShipsInRange);
        }

        private void AssignCalculateTokensRecursive(List<GenericShip> friendlyShipsInRange)
        {
            if (friendlyShipsInRange.Count > 0)
            {
                GenericShip shipToAssign = friendlyShipsInRange.First();
                friendlyShipsInRange.Remove(shipToAssign);

                Messages.ShowInfo("TA-175: " + shipToAssign.PilotInfo.PilotName + " gets Calculate token", allowCopies: true);

                shipToAssign.Tokens.AssignToken(
                    typeof(Tokens.CalculateToken),
                    delegate { AssignCalculateTokensRecursive(friendlyShipsInRange); }
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}