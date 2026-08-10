using Content;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Fireball
{
    public class R1J5 : Fireball
    {
        public R1J5() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "R1-J5",
                "Bucket",
                Faction.Resistance,
                1,
                3,
                8,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.R1J5PilotAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Crew,
                    UpgradeType.Modification,
                    UpgradeType.Missile,
                    UpgradeType.Title
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            ShipInfo.ActionIcons.SwitchToDroidActions();
        }
    }

    public class R1J5XWA : R1J5
    {
        public R1J5XWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 7;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 10;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Crew,
                UpgradeType.Illicit,
                UpgradeType.Modification,
                UpgradeType.Missile,
                UpgradeType.Title
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    // Before you expose 1 of your damage cards, you may look at your facedown damage cards,
    // choose 1, and expose that card instead.

    public class R1J5PilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnSelectDamageCardToExpose += AskToSelect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSelectDamageCardToExpose -= AskToSelect;
        }

        private void AskToSelect(ref bool isOverriden)
        {
            // Only if player can select another card - only if there are 2 or more facedown damage cards
            if (HostShip.Damage.GetFacedownCards().Count > 1)
            {
                isOverriden = true;

                RegisterAbilityTrigger(TriggerTypes.OnSelectDamageCardToExpose, ShowFacedownDamageCardsToSelect);
            }
        }

        private void ShowFacedownDamageCardsToSelect(object sender, EventArgs e)
        {
            ShowShipCrits();
        }

        protected void ShowShipCrits()
        {
            SelectShipCritDecision subphase = (SelectShipCritDecision)Phases.StartTemporarySubPhaseNew(
                "Select Damage Card",
                typeof(SelectShipCritDecision),
                Triggers.FinishTrigger
            );

            foreach (GenericDamageCard card in HostShip.Damage.DamageCards)
            {
                Decision existingDecision = subphase.GetDecisions().Find(n => n.Name == card.Name);
                if (existingDecision == null)
                {
                    subphase.AddDecision(card.Name, delegate { SelectDamageCard(card); }, card.ImageUrl, 1);
                }
                else
                {
                    existingDecision.SetCount(existingDecision.Count + 1);
                }
            }

            subphase.DecisionViewType = DecisionViewTypes.ImagesDamageCard;

            subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName + ": Select Damage Card to Expose";

            subphase.RequiredPlayer = HostShip.Owner.PlayerNo;

            subphase.Start();
        }

        protected void SelectDamageCard(GenericDamageCard damageCard)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Messages.ShowInfo(damageCard.Name + " has been selected");

            damageCard.Expose(Triggers.FinishTrigger);
        }

        protected class SelectShipCritDecision : DecisionSubPhase { };
    }
}