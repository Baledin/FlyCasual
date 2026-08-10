using Arcs;
using Content;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.TIEWiWhisperModifiedInterceptor
{
    public class KyloRenWhisper : TIEWiWhisperModifiedInterceptor
    {
        public KyloRenWhisper() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Kylo Ren",
                "Supreme Leader of the First Order",
                Faction.FirstOrder,
                5,
                5,
                14,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.KyloRenWhisperPilotAbility),
                force: 3,
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.ForcePower,
                    UpgradeType.ForcePower,
                    UpgradeType.Talent,
                    UpgradeType.Missile,
                    UpgradeType.Tech,
                    UpgradeType.Tech,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.DarkSide,
                    Tags.LightSide,
                    Tags.Tie
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            PilotNameCanonical = "kyloren-tiewiwhispermodifiedinterceptor";
        }
    }

    public class KyloRenWhisperXWA : KyloRenWhisper
    {
        public KyloRenWhisperXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 14;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 12;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.ForcePower,
                UpgradeType.ForcePower,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Tech,
                UpgradeType.Missile,
                UpgradeType.Configuration
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KyloRenWhisperPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnDamageCardSeverityIsCheckedGlobal += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnDamageCardSeverityIsCheckedGlobal -= CheckConditions;
        }

        private void CheckConditions(GenericShip ship)
        {
            if (!Combat.CurrentCriticalHitCard.IsFaceup
                && !Tools.IsSameTeam(HostShip, ship)
                && HostShip.State.Force > 0
                && HostShip.SectorsInfo.IsShipInSector(ship, ArcType.Bullseye))
            {
                RegisterAbilityTrigger(TriggerTypes.OnDamageCardSeverityIsChecked, AskUseKyloRenAbility);
            }
        }

        private void AskUseKyloRenAbility(object sender, System.EventArgs e)
        {
            GenericShip previousShip = Selection.ActiveShip;
            Selection.ActiveShip = sender as GenericShip;

            AskToUseAbility
            (
                descriptionShort: HostShip.PilotInfo.PilotName,
                IsShouldUseAbility,
                UseAbility,
                delegate
                {
                    Selection.ActiveShip = previousShip;
                    DecisionSubPhase.ConfirmDecision();
                },
                descriptionLong: "Do you want to spend 1 Force to deal that facedown damage card as faceup instead?",
                imageHolder: HostShip
            );
        }

        private bool IsShouldUseAbility()
        {
            return true;
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            Combat.CurrentCriticalHitCard.IsFaceup = true;
            HostShip.State.SpendForce(1, DecisionSubPhase.ConfirmDecision);
        }
    }
}
