using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NantexClassStarfighter
    {
        public class Gorgol : NantexClassStarfighter
        {
            public Gorgol() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Gorgol",
                    "Handy Engineer",
                    Faction.Separatists,
                    2,
                    4,
                    9,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.GorgolAbility),
                    abilityText: "During the System Phase, you may gain 1 disarm token and choose a friendly ship at range 1-2. If you do, it gains 1 tractor token, then repairs 1 of its faceup Ship trait damage cards.",
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class GorgolXWA : Gorgol
        {
            public GorgolXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 9;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 11;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GorgolAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnSystemsPhaseStart += RegisterOwnAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsPhaseStart -= RegisterOwnAbilityTrigger;
        }

        private void RegisterOwnAbilityTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnSystemsPhaseStart, StartSelectionSubphase);
        }

        private void StartSelectionSubphase(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                TargetIsSelected,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostShip.PilotInfo.PilotName,
                description: "You may gain 1 disarm token to choose a friednly ship, assign 1 tractor token to it, and then repair it's 1 faceup Ship damage card",
                imageSource: HostShip
            );
        }

        private int GetAiPriority(GenericShip ship)
        {
            // AI doesn't use this ability
            return 0;
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.OtherFriendly)
                && FilterTargetsByRange(ship, 1, 2);
        }

        private void TargetIsSelected()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            HostShip.Tokens.AssignToken(
                typeof(Tokens.WeaponsDisabledToken),
                AssignTractorTokenToTaget
            );
        }

        private void AssignTractorTokenToTaget()
        {
            Selection.ChangeActiveShip(TargetShip);

            TargetShip.Tokens.AssignToken(
                new Tokens.TractorBeamToken(TargetShip, HostShip.Owner),
                TryToRepairCrit
            );
        }

        private void TryToRepairCrit()
        {
            List<GenericDamageCard> shipCritsList = Selection.ThisShip.Damage.GetFaceupCrits(CriticalCardType.Ship);

            if (shipCritsList.Count == 1)
            {
                GenericDamageCard card = shipCritsList.First();
                Selection.ThisShip.Damage.FlipFaceupCritFacedown(card);
                Triggers.FinishTrigger();
            }
            else if (shipCritsList.Count > 1)
            {
                GorgolDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<GorgolDecisionSubPhase>(
                    HostShip.PilotInfo.PilotName + ": Select faceup ship Crit",
                    Triggers.FinishTrigger
                );
                subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
                subphase.DescriptionLong = "Select a faceup ship Crit damage card to repair it";
                subphase.ImageSource = HostShip;
                subphase.Start();
            }
            else
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": To Ship faceup crits to repair");
                Triggers.FinishTrigger();
            }
        }
    }

}

namespace SubPhases
{

    public class GorgolDecisionSubPhase : DecisionSubPhase
    {
        public override void PrepareDecision(System.Action callBack)
        {
            DecisionViewType = DecisionViewTypes.ImagesDamageCard;

            foreach (var shipCrit in Selection.ThisShip.Damage.GetFaceupCrits(CriticalCardType.Ship).ToList())
            {
                AddDecision(shipCrit.Name, delegate { RepairCrit(shipCrit); }, shipCrit.ImageUrl);
            }

            DefaultDecisionName = GetDecisions().First().Name;

            callBack();
        }

        private void RepairCrit(GenericDamageCard critCard)
        {
            Selection.ThisShip.Damage.FlipFaceupCritFacedown(critCard);
            ConfirmDecision();
        }

    }

}