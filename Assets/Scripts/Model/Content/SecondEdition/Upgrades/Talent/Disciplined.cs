using ActionsList;
using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace UpgradesList.SecondEdition
{
    public class Disciplined : GenericUpgrade
    {
        public Disciplined() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Disciplined",
                UpgradeType.Talent,
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.DisciplinedAbility),
                restriction: new FactionRestriction(Faction.Imperial)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DisciplinedAbility : GenericAbility
    {
        private GenericShip PreviousCurrentShip { get; set; }

        public override void ActivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, bool flag)
        {
            if (!Tools.IsAnotherFriendly(HostShip, ship)) return;
            if (!ship.PilotInfo.IsLimited && !ship.UpgradeBar.HasUpgradeInstalled(typeof(Disciplined))) return;

            DistanceInfo distanceInfo = new DistanceInfo(HostShip, ship);
            if (distanceInfo.Range > 3) return;

            RegisterAbilityTrigger(
                TriggerTypes.OnShipIsDestroyed,
                AskWhatToDo,
                customTriggerName: $"{HostUpgrade.UpgradeInfo.Name} ({HostShip.PilotInfo.PilotName})"
            );
        }

        private void AskWhatToDo(object sender, EventArgs e)
        {
            PreviousCurrentShip = Selection.ThisShip;

            Selection.ChangeActiveShip(HostShip);
            Selection.ThisShip.AskPerformFreeAction(
                new List<GenericAction>()
                {
                    new TargetLockAction(){ HostShip = HostShip },
                    new BarrelRollAction(){ HostShip = HostShip },
                },
                FinishAbility,
                descriptionShort: HostUpgrade.UpgradeInfo.Name,
                descriptionLong: "You may perform a Lock or Barrel Roll action",
                imageHolder: HostUpgrade
            );
        }

        private void FinishAbility()
        {
            Selection.ChangeActiveShip(PreviousCurrentShip);
            Triggers.FinishTrigger();
        }
    }
}