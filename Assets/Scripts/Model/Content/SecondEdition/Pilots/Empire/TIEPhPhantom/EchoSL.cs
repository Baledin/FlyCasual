using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.TIEPhPhantom
{
    public class EchoSL : TIEPhPhantom
    {
        public EchoSL() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "\"Echo\"",
                "Copycat",
                Faction.Imperial,
                4,
                5,
                0,
                isLimited: true,
                isStandardLayout: true,
                abilityType: typeof(Abilities.SecondEdition.EchoSLAbility),
                tags: new List<Tags>
                {
                    Tags.Tie
                },
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Modification
                },
                charges: 1,
                regensCharges: 1,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            PilotNameCanonical = "echo-ssl";

            MustHaveUpgrades.Add(typeof(SilentHunter));
            MustHaveUpgrades.Add(typeof(StealthGambit));
            MustHaveUpgrades.Add(typeof(ManualAilerons));
        }
    }

    public class EchoSLXWA : EchoSL
    {
        public EchoSLXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 14;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}


namespace Abilities.SecondEdition
{
    public class EchoSLAbility : GenericAbility
    {
        // After an enemy ship at range 0-1 performs an action on its action bar,
        // you may spend 1 charge to perform the same action, treating it as white.

        GenericAction savedAction;

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
            if (HostShip.State.Charges > 0 &&
                !HostShip.IsStressed &&
                !Tools.IsFriendly(action.HostShip, HostShip) &&
                HostShip.GetRangeToShip(action.HostShip) < 2 &&
                action.IsInActionBar &&
                HostShip.ActionBar.HasAction(action.GetType()))
            {
                // Generate a new copy of the action so that you don't modify the original action on the hostship
                savedAction = (GenericAction)Activator.CreateInstance(action.GetType());
                savedAction.Color = Actions.ActionColor.White;

                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskToUseAbility);
            }
        }

        private void AskToUseAbility(object sender, EventArgs e)
        {
            if (HostShip.State.Charges < 1) return; // Linked actions may queue multiple abilities here, ensure we still have a charge before asking

            HostShip.OnActionIsPerformed += SpendCharge;

            GenericShip originalShip = Selection.ThisShip;
            Selection.ThisShip = HostShip;

            HostShip.AskPerformFreeAction(
                savedAction,
                delegate
                {
                    HostShip.OnActionIsPerformed -= SpendCharge;

                    Selection.ThisShip = originalShip;

                    Triggers.FinishTrigger();
                },
                HostShip.PilotInfo.PilotName,
                descriptionLong: $"You may spend 1 charge to perform a {savedAction.Name} action."
            );
        }

        private void SpendCharge(GenericAction action)
        {
            HostShip.SpendCharge();
        }
    }
}