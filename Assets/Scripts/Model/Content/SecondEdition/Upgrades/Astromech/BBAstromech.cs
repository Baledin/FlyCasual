using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class BBAstromech : GenericUpgrade
    {
        public BBAstromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "BB Astromech",
                UpgradeType.Astromech,
                charges: 2,
                cost: 4,
                restriction: new FactionRestriction(Faction.Resistance),
                abilityType: typeof(Abilities.SecondEdition.BBAstromechAbility),
                legalityInfo: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class BBAstromechXWA : BBAstromech
    {
        public BBAstromechXWA() : base()
        {
            UpgradeInfo.Cost = 3;
            UpgradeInfo.LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    // During the system phase, you may spend 1 charge to perform a barrel roll action.

    public class BBAstromechAbility : GenericAbility
    {
        protected List<GenericAction> AbilityActions = new() { new BarrelRollAction() };
        protected GenericShip selectedShip;

        public override void ActivateAbility()
        {
            HostShip.OnSystemsPhaseStart += PlanAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsPhaseStart -= PlanAction;
        }

        private void PlanAction(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnSystemsPhaseStart, AskPerformAction);
        }

        private void AskPerformAction(object sender, EventArgs e)
        {
            if (HostUpgrade.State.Charges < 1)
            {
                Triggers.FinishTrigger();
                return;
            }

            HostShip.OnActionIsPerformed += SpendCharge;

            Selection.ThisShip = HostShip; // System phase doesn't have an active ship set yet

            HostShip.AskPerformFreeAction(
                AbilityActions,
                CleanUp,
                HostUpgrade.UpgradeInfo.Name,
                "You may spend 1 Charge to perform a Barrel Roll action",
                HostUpgrade
            );
        }

        private void SpendCharge(GenericAction action)
        {
            HostShip.OnActionIsPerformed -= SpendCharge;
            Sounds.PlayShipSound("BB-8-Sound");
            HostUpgrade.State.SpendCharge();
        }

        private void CleanUp()
        {
            HostShip.OnActionIsPerformed -= SpendCharge;
            Triggers.FinishTrigger();
        }
    }
}