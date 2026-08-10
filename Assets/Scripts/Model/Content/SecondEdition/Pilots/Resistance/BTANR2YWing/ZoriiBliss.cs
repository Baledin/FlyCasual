using ActionsList;
using BoardTools;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.BTANR2YWing
{
    public class ZoriiBliss : BTANR2YWing
    {
        public ZoriiBliss() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Zorii Bliss",
                "Corsair of Kijimi",
                Faction.Resistance,
                5,
                4,
                9,
                isLimited: true,
                charges: 1,
                regensCharges: 1,
                abilityType: typeof(Abilities.SecondEdition.ZoriiBlissAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Device,
                    UpgradeType.Turret,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.YWing
                },
                skinName: "Red",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class ZoriiBlissXWA : ZoriiBliss
    {
        public ZoriiBlissXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 11;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 20;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Talent,
                UpgradeType.Astromech,
                UpgradeType.Illicit,
                UpgradeType.Modification,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Device,
                UpgradeType.Turret
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ZoriiBlissAbility : GenericAbility
    {
        GenericAction ActionPerformedNearby;

        public override void ActivateAbility()
        {
            GenericShip.OnActionIsPerformedGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnActionIsPerformedGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericAction action)
        {
            if (HostShip.State.Charges > 0
                && HostShip.ActionBar.HasAction(action.GetType())
                && IsPerformActionStep()
                && IsInRange1()
                && !HostShip.IsAlreadyExecutedAction(action)) // To track is action performed - to spend charge correctly
            {
                ActionPerformedNearby = action;
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskToPerformActionAsRed);
            }
        }

        private bool IsPerformActionStep()
        {
            return Phases.CurrentSubPhase.GetType() == typeof(SubPhases.ActionDecisonSubPhase);
        }

        private bool IsInRange1()
        {
            DistanceInfo distInfo = new DistanceInfo(HostShip, Selection.ThisShip);
            return distInfo.Range == 1;
        }

        private void AskToPerformActionAsRed(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            GenericAction sameActionAsRed = ActionPerformedNearby.AsRedAction;
            sameActionAsRed.HostShip = HostShip;

            HostShip.AskPerformFreeAction
            (
                sameActionAsRed,
                FinishAbility,
                descriptionShort: HostShip.PilotInfo.PilotName,
                descriptionLong: "You may perform action, treating it as red",
                imageHolder: HostShip
            );
        }

        private void FinishAbility()
        {
            if (HostShip.IsAlreadyExecutedAction(ActionPerformedNearby)) HostShip.SpendCharge();
            Selection.ChangeActiveShip(ActionPerformedNearby.HostShip);

            Triggers.FinishTrigger();
        }
    }
}
