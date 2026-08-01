using Actions;
using ActionsList;
using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class PloKoon : GenericUpgrade
    {
        public PloKoon() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Plo Koon",
                UpgradeType.Crew,
                cost: 9,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Republic),
                addAction: new ActionInfo(typeof(ReinforceAction), ActionColor.Purple),
                abilityType: typeof(Abilities.SecondEdition.PloKoonCrewAbility),
                addForce: 1
            );

            Avatar = new AvatarInfo(
                Faction.Republic,
                new Vector2(297, 3),
                new Vector2(147, 147)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PloKoonCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (HostShip.IsReinforced && HasTargetForAbility())
            {
                RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, AskToSelectShip);
            }
        }

        private bool HasTargetForAbility()
        {
            foreach (GenericShip ship in HostShip.Owner.Ships.Values)
            {
                if (FilterTargets(ship)) return true;
            }

            return false;
        }

        private void AskToSelectShip(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                AskShipForDecision,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostUpgrade.UpgradeInfo.Name,
                description: "Choose a ship - it can remove 1 deplete or strain token, or repair 1 faceup damage card",
                imageSource: HostUpgrade
            );
        }

        private void AskShipForDecision()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            PloKoonDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<PloKoonDecisionSubphase>("Plo Koon Decision", Triggers.FinishTrigger);

            subphase.DecisionOwner = HostShip.Owner;
            subphase.DescriptionShort = HostUpgrade.UpgradeInfo.Name;
            subphase.DescriptionLong = "Choose one:";
            subphase.ImageSource = HostUpgrade;

            subphase.ShowSkipButton = false;

            if (TargetShip.Tokens.HasToken<Tokens.StrainToken>())
            {
                subphase.AddDecision("Remove Strain Token", delegate { RemoveToken(typeof(Tokens.StrainToken)); });
            }

            if (TargetShip.Tokens.HasToken<Tokens.DepleteToken>())
            {
                subphase.AddDecision("Remove Deplete Token", delegate { RemoveToken(typeof(Tokens.DepleteToken)); });
            }

            foreach (GenericDamageCard damageCard in TargetShip.Damage.GetFaceupCrits())
            {
                subphase.AddDecision($"Repair {damageCard.Name}", delegate { Repair(damageCard); });
            }

            subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;

            subphase.Start();
        }

        private class PloKoonDecisionSubphase : DecisionSubPhase { }

        private void Repair(GenericDamageCard damageCard)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            TargetShip.Damage.FlipFaceupCritFacedown(damageCard);
            Triggers.FinishTrigger();
        }

        private void RemoveToken(Type type)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            TargetShip.Tokens.RemoveToken(type, Triggers.FinishTrigger);
        }

        private bool FilterTargets(GenericShip ship)
        {
            if (!Tools.IsFriendly(HostShip, ship)) return false;

            DistanceInfo distInfo = new(HostShip, ship);
            if (distInfo.Range == 0
                || HostShip.SectorsInfo.RangeToShipBySector(ship, Arcs.ArcType.Left) == 1
                || HostShip.SectorsInfo.RangeToShipBySector(ship, Arcs.ArcType.Right) == 1
            )
            {
                return (ship.Tokens.HasToken<Tokens.StrainToken>()
                    || ship.Tokens.HasToken<Tokens.DepleteToken>()
                    || ship.Damage.HasFaceupCards
                );
            }

            return false;
        }

        private int GetAiPriority(GenericShip ship)
        {
            int result = 0;

            if (ship.Tokens.HasToken<Tokens.StrainToken>()
                || ship.Tokens.HasToken<Tokens.DepleteToken>()
                || ship.Damage.HasFaceupCards)
            {
                result = 100 + ship.PilotInfo.Cost;
            }

            return result;
        }
    }
}