using ActionsList;
using Content;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.T70XWing
{
    public class PoeDameron : T70XWing
    {
        public PoeDameron() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Poe Dameron",
                "Trigger-Happy Flyboy",
                Faction.Resistance,
                6,
                7,
                25,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.PoeDameronAbility),
                charges: 1,
                regensCharges: 1,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Tech,
                    UpgradeType.Torpedo,
                    UpgradeType.Astromech,
                    UpgradeType.Modification,
                    UpgradeType.Modification,
                    UpgradeType.Configuration,
                    UpgradeType.Title
                },
                tags: new List<Tags>
                {
                    Tags.XWing
                },
                skinName: "Black One",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class PoeDameronXWA : PoeDameron
    {
        public PoeDameronXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 17;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 20;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Talent,
                UpgradeType.Talent,
                UpgradeType.Astromech,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Configuration,
                UpgradeType.Title
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform an action, you may spend 1 charge to perform an action.
    public class PoeDameronAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
        }

        private void CheckConditions(GenericAction action)
        {
            if (HostShip.State.Charges > 0)
            {
                HostShip.OnActionDecisionSubphaseEnd += DoAnotherAction;
            }
        }

        private void DoAnotherAction(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= DoAnotherAction;

            RegisterAbilityTrigger(TriggerTypes.OnFreeAction, PerformAction);
        }

        private void PerformAction(object sender, System.EventArgs e)
        {
            if (HostShip.State.Charges > 0) {
                HostShip.BeforeActionIsPerformed += PayChargeCost;

                List<GenericAction> actions = Selection.ThisShip.GetAvailableActions();
                List<GenericAction> whiteActionBarActionsAsRed = actions
                    .Where(n => n.Color == Actions.ActionColor.White)
                    .Select(n => n.AsRedAction)
                    .ToList();

                HostShip.AskPerformFreeAction(
                    whiteActionBarActionsAsRed,
                    CleanUp,
                    HostShip.PilotInfo.PilotName,
                    "After you perform an action, you may spend 1 Charge to perform a white action, treating it as red",
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void PayChargeCost(GenericAction action, ref bool isFreeAction)
        {
            HostShip.State.Charges--;
            HostShip.BeforeActionIsPerformed -= PayChargeCost;
        }

        private void CleanUp()
        {
            HostShip.BeforeActionIsPerformed -= PayChargeCost;
            Triggers.FinishTrigger();
        }
    }
}
