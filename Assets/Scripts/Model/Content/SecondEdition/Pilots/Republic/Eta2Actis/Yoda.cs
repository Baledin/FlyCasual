using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Eta2Actis
{
    public class Yoda : Eta2Actis
    {
        public Yoda()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Yoda",
                "Grand Master",
                Faction.Republic,
                3,
                4,
                12,
                isLimited: true,
                force: 3,
                abilityType: typeof(Abilities.SecondEdition.YodaPilotAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.ForcePower,
                    UpgradeType.Cannon,
                    UpgradeType.Astromech,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class YodaXWA : Yoda
    {
        public YodaXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 10;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 11;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.ForcePower,
                UpgradeType.ForcePower,
                UpgradeType.Talent,
                UpgradeType.Astromech,
                UpgradeType.Modification,
                UpgradeType.Cannon
            };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class YodaPilotAbility : GenericAbility
    {
        private GenericShip TriggeredShip { get; set; }

        public override void ActivateAbility()
        {
            GenericShip.OnForceTokensAreSpent += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnForceTokensAreSpent -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship, ref int count)
        {
            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);

            if (count > 0
                && HostShip.State.Force > 0
                && Tools.IsAnotherFriendly(ship, HostShip)
                && distInfo.Range < 4
            )
            {
                TriggeredShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnForceTokensAreSpent, AskToRestoreCharge);
            }
        }

        private void AskToRestoreCharge(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                AlwaysUseByDefault,
                RestoreForceCharge,
                descriptionLong: $"Do you want to spend 1 Force to recover used 1 Force of {TriggeredShip.PilotInfo.PilotName}?",
                imageHolder: HostShip,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void RestoreForceCharge(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            TriggeredShip.State.RestoreForce();
            HostShip.State.SpendForce(1, Triggers.FinishTrigger);
        }
    }
}
