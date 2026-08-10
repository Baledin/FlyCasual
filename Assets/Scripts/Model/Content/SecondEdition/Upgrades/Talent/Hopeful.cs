using ActionsList;
using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace UpgradesList.SecondEdition
{
    public class Hopeful : GenericUpgrade
    {
        public Hopeful() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Hopeful",
                UpgradeType.Talent,
                cost: 1,
                abilityType: typeof(Abilities.SecondEdition.HopefulAbility),
                restriction: new FactionRestriction(Faction.Rebel)
            );


        }
    }
}

namespace Abilities.SecondEdition
{
    public class HopefulAbility : GenericAbility
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
            if (!ship.PilotInfo.IsLimited && !ship.UpgradeBar.HasUpgradeInstalled(typeof(Hopeful))) return;

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
                    new FocusAction(){ HostShip = HostShip },
                    new BoostAction(){ HostShip = HostShip },
                },
                FinishAbility,
                descriptionShort: HostUpgrade.UpgradeInfo.Name,
                descriptionLong: "You may perform a Focus or Boost action",
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