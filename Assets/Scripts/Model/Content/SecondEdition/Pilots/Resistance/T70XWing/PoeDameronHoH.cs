using ActionsList;
using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.T70XWing
{
    public class PoeDameronHoH : T70XWing
    {
        public PoeDameronHoH() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Poe Dameron",
                "Resistance Commander",
                Faction.Resistance,
                6,
                6,
                19,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.PoeDameronHoHAbility),
                charges: 2,
                regensCharges: 1,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Tech,
                    UpgradeType.Tech,
                    UpgradeType.Astromech,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.XWing
                },
                skinName: "Poe Dameron (RoS)",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            PilotNameCanonical = "poedameron-swz68";
        }
    }

    public class PoeDameronHoHXWA : PoeDameronHoH
    {
        public PoeDameronHoHXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 15;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 13;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Talent,
                UpgradeType.Talent,
                UpgradeType.Astromech,
                UpgradeType.Sensor,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PoeDameronHoHAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnActionIsPerformedGlobal += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnActionIsPerformedGlobal -= CheckConditions;
        }

        private void CheckConditions(GenericAction action)
        {
            if (HostShip.State.Charges >= 2
                && HostShip.Owner.PlayerNo == Selection.ThisShip.Owner.PlayerNo
                && Phases.CurrentPhase is MainPhases.ActivationPhase
                && (Phases.CurrentPhase as MainPhases.ActivationPhase).ActivationShip == Selection.ThisShip
            )
            {
                DistanceInfo distanceInfo = new DistanceInfo(HostShip, Selection.ThisShip);
                if (distanceInfo.Range <= 2)
                {
                    Selection.ThisShip.OnActionDecisionSubphaseEnd += DoAnotherAction;
                }
            }
        }

        private void DoAnotherAction(GenericShip ship)
        {
            Selection.ThisShip.OnActionDecisionSubphaseEnd -= DoAnotherAction;

            RegisterAbilityTrigger(TriggerTypes.OnFreeAction, PerformAction);
        }

        private void PerformAction(object sender, System.EventArgs e)
        {
            if (HostShip.State.Charges >= 2) {
                Selection.ThisShip.BeforeActionIsPerformed += PayChargeCost;

                List<GenericAction> actions = Selection.ThisShip.GetAvailableActions();
                List<GenericAction> whiteActionBarActionsAsRed = actions
                    .Where(n => n.Color == Actions.ActionColor.White)
                    .Select(n => n.AsRedAction)
                    .ToList();

                Selection.ThisShip.AskPerformFreeAction(
                    whiteActionBarActionsAsRed,
                    CleanUp,
                    HostShip.PilotInfo.PilotName,
                    "After a friendly ship performs an action, Poe Dameron may spend 2 Charges to allow that ship to perform a white action, treating it as red",
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
            HostShip.State.Charges -= 2;
            Selection.ThisShip.BeforeActionIsPerformed -= PayChargeCost;
        }

        private void CleanUp()
        {
            Selection.ThisShip.BeforeActionIsPerformed -= PayChargeCost;
            Triggers.FinishTrigger();
        }
    }
}
