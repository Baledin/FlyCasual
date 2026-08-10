using Actions;
using ActionsList;
using Ship;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class BattleMeditation : GenericUpgrade
    {
        public BattleMeditation() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Battle Meditation",
                UpgradeType.ForcePower,
                cost: 6,
                restriction: new FactionRestriction(Faction.Republic),
                addAction: new ActionInfo(typeof(CoordinateAction), ActionColor.Purple),
                abilityType: typeof(Abilities.SecondEdition.BattleMeditationAbility)
            );
        }
    }

    public class BattleMeditationXWA : BattleMeditation
    {
        public BattleMeditationXWA() : base()
        {
            UpgradeInfo.Limited = 3;
        }
    }
}


namespace Abilities.SecondEdition
{
    // You cannot coordinate limited ships. While you perform a purple coordinate action, you may coordinate
    // 1 additional friendly non-limited ship of the same type. Both ships must perform the same action.
    public class BattleMeditationAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckCanCoordinate += ForbitLimitedShips;
            HostShip.BeforeActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckCanCoordinate -= ForbitLimitedShips;
            HostShip.BeforeActionIsPerformed -= CheckAbility;
        }

        private void ForbitLimitedShips(GenericShip ship, ref bool canBeCoordinated)
        {
            if (ship.PilotInfo.IsLimited) canBeCoordinated = false;
        }

        private void CheckAbility(GenericAction action, ref bool isFree)
        {
            if (action is CoordinateAction && action.Color == ActionColor.Purple)
            {
                HostShip.OnCheckCoordinateModeModification += SetCustomCoordinateMode;
            }
        }

        private void SetCustomCoordinateMode(ref CoordinateActionData coordinateActionData)
        {
            coordinateActionData.MaxTargets = 2;
            coordinateActionData.SameShipTypeLimit = true;
            coordinateActionData.SameActionLimit = true;

            HostShip.OnCheckCoordinateModeModification -= SetCustomCoordinateMode;
        }
    }
}