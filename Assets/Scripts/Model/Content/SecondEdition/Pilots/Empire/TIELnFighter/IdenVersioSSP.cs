using Abilities.SecondEdition;
using Content;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.TIELnFighter
{
    public class IdenVersioSSP : TIELnFighter
    {
        public IdenVersioSSP() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Iden Versio",
                "Inferno Leader",
                Faction.Imperial,
                4,
                4,
                0,
                isLimited: true,
                abilityType: typeof(IdenVersioSSPAbility),
                charges: 1,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent
                },
                tags: new List<Tags>
                {
                    Tags.Tie
                },
                skinName: "Inferno",
                isStandardLayout: true,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            MustHaveUpgrades.Add(typeof(Disciplined));
            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.Elusive));

            PilotNameCanonical = "idenversio-swz105";
        }
    }

    public class IdenVersioSSPXWA : IdenVersioSSP
    {
        public IdenVersioSSPXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 9;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}


namespace Abilities.SecondEdition
{
    public class IdenVersioSSPAbility : GenericAbility
    {
        private GenericShip defender;

        public override void ActivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal += CheckIdenVersioSSPAbilitySE;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal -= CheckIdenVersioSSPAbilitySE;
        }

        private void CheckIdenVersioSSPAbilitySE(GenericShip ship, DamageSourceEventArgs e)
        {
            defender = ship;

            // Is the defender on our team? If not return.
            if (defender.Owner.PlayerNo != HostShip.Owner.PlayerNo)
                return;

            if (!(defender is Ship.SecondEdition.TIELnFighter.TIELnFighter))
                return;

            // If the defender is at range one of us we register our trigger to prevent damage.
            BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(defender, HostShip);
            if (distanceInfo.Range <= 1)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTryDamagePrevention, UseIdenVersioSSPAbilitySE);
            }
        }

        private void UseIdenVersioSSPAbilitySE(object sender, System.EventArgs e)
        {
            // Are there any non-crit damage results in the damage queue?
            if (HostShip.State.Charges > 0)
            {
                // If there are we prompt to see if they want to use the ability.
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    delegate { HostShip.RemoveCharge(BlankDamage); },
                    descriptionLong: "Do you want to spend 1 Charge to prevent damage?",
                    imageHolder: HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void BlankDamage()
        {
            defender.AssignedDamageDiceroll.RemoveAll();
            DecisionSubPhase.ConfirmDecision();
        }
    }
}