using Abilities.SecondEdition;
using Arcs;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.TIEVnSilencer
{
    public class KyloRenEoD : TIEVnSilencer
    {
        public KyloRenEoD() : base()
        {
            PilotInfo = new PilotCardInfo25(
                pilotName: "Kylo Ren",
                pilotTitle: "Evacuation of D'Qar",
                faction: Faction.FirstOrder,
                initiative: 5,
                cost: 17,
                loadoutValue: 0,
                isLimited: true,
                limited: 1,
                abilityType: typeof(KyloRenEoDAbility),
                force: 3,
                regensForce: 1,
                extraUpgradeIcons: new()
                {
                    UpgradeType.Talent,
                    UpgradeType.ForcePower,
                    UpgradeType.Torpedo
                },
                tags: new()
                {
                    Tags.DarkSide,
                    Tags.Tie
                },
                isStandardLayout: true,
                legality: new List<Legality>() { Legality.XWA }
            );

            PilotNameCanonical = "kyloren-evacuationofdqar";

            MustHaveUpgrades.Add(typeof(Determination));
            MustHaveUpgrades.Add(typeof(Malice));
            MustHaveUpgrades.Add(typeof(ProtonTorpedoes));
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KyloRenEoDAbility : GenericAbility
    {
        // Before an enemy ship in your bullseye is dealt a facedown damage card, you may spend 1 force. If you do, that damage card is dealt faceup instead.

        GenericShip targetShip;

        public override void ActivateAbility()
        {
            GenericShip.OnDamageCardIsDealtGlobal += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnDamageCardIsDealtGlobal -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship)
        {
            if (IsValidTargetOfAbility(ship))
            {
                targetShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnDamageCardIsDealt, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                AlwaysUseByDefault,
                UseDeterminationAbility,
                descriptionLong: $"Do you want to spend 1 force to flip a facedown damage card dealt to {targetShip.PilotInfo.PilotName}?",
                imageHolder: HostShip
            );
        }

        private void UseDeterminationAbility(object sender, EventArgs e)
        {
            HostShip.State.SpendForce(1, delegate
            {
                Combat.CurrentCriticalHitCard.IsFaceup = true;
                DecisionSubPhase.ConfirmDecision();
            });
        }

        private bool IsValidTargetOfAbility(GenericShip ship)
        {
            return Tools.IsAnotherTeam(HostShip, ship)
                && HostShip.SectorsInfo.IsShipInSector(ship, ArcType.Bullseye)
                && !Combat.CurrentCriticalHitCard.IsFaceup
                && HostShip.State.Force > 0;
        }
    }
}