using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class Slammer : V19TorrentStarfighter
    {
        public Slammer()
        {
            PilotInfo = new PilotCardInfo25
            (
                "\"Slammer\"",
                "Blue Three",
                Faction.Republic,
                1,
                3,
                7,
                isLimited: true,
                charges: 2,
                regensCharges: 1,
                abilityType: typeof(Abilities.SecondEdition.SlammerAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Missile,
                    UpgradeType.Missile,
                },
                tags: new List<Tags>
                {
                    Tags.Clone
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class SlammerXWA : Slammer
    {
        public SlammerXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 7;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 7;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Modification,
                UpgradeType.Modification,
                UpgradeType.Missile,
            };

        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a speed 3-5 maneuver you may spend 1 charge to perform a boost action, even while stressed.
    public class SlammerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            //AI doesn't use ability
            if (HostShip.Owner.UsesHotacAiRules) return;

            if (HostShip.State.Charges >= 2)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            HostShip.BeforeActionIsPerformed += RegisterSpendChargeTrigger;
            HostShip.AskPerformFreeAction(
                new SlamAction(HostShip),
                CleanUp,
                HostShip.PilotInfo.PilotName,
                "After you fully execute a maneuver you may spend 1 Charge to perform a Slam action, even while stressed.",
                HostShip
            );
        }

        private void RegisterSpendChargeTrigger(GenericAction action, ref bool isFreeAction)
        {
            HostShip.BeforeActionIsPerformed -= RegisterSpendChargeTrigger;
            RegisterAbilityTrigger(
                TriggerTypes.OnFreeAction,
                delegate
                {
                    HostShip.SpendCharges(2);
                    Triggers.FinishTrigger();
                }
            );
        }

        private void CleanUp()
        {
            HostShip.BeforeActionIsPerformed -= RegisterSpendChargeTrigger;
            Triggers.FinishTrigger();
        }
    }
}
