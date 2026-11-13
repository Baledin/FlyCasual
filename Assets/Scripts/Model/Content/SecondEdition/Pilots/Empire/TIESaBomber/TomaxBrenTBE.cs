using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.TIESaBomber
    {
        public class TomaxBrenTBE : TIESaBomber
        {
            public TomaxBrenTBE()
            {
                PilotInfo = new PilotCardInfo25(
                    "Tomax Bren",
                    "Scimitar Veteran",
                    Faction.Imperial,
                    5,
                    4,
                    loadoutValue: 0,
                    isStandardLayout: true,
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    isLimited: true,
                    charges: 2,
                    regensCharges: 1,
                    abilityType: typeof(Abilities.SecondEdition.TomaxBrenTBEAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Device
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
                PilotNameCanonical = "tomaxbren-swz98";

                MustHaveUpgrades.Add(typeof(PlasmaTorpedoes));
                MustHaveUpgrades.Add(typeof(IonBombs));
                MustHaveUpgrades.Add(typeof(TrueGrit));
            }
        }

        public class TomaxBrenTBEXWA : TomaxBrenTBE
        {
            public TomaxBrenTBEXWA() : base()
            {
                var pilot = (PilotCardInfo25)PilotInfo;
                pilot.Cost = 12;
                pilot.LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform a Barrel Roll action, you may spend 2 charges. If you do, gain a focus token.
    public class TomaxBrenTBEAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
        }

        private void CheckConditions(GenericAction action)
        {
            if (action is BarrelRollAction && HostShip.State.Charges > 1)
            {
                HostShip.OnActionDecisionSubphaseEnd += RegisterActionTrigger;
            }
        }

        private void RegisterActionTrigger(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= RegisterActionTrigger;

            RegisterAbilityTrigger(TriggerTypes.OnFreeAction, AskToUseOwnAbility);
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                NeverUseByDefault,
                ActivateOwnAbility,
                descriptionLong: "Do you want to spend 2 charges to gain a focus token?",
                imageHolder: HostUpgrade
            );
        }

        private void ActivateOwnAbility(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.AssignToken(typeof(Tokens.FocusToken), Cleanup);
        }

        private void Cleanup()
        {
            HostShip.SpendCharges(2);
            Triggers.FinishTrigger();
        }
    }
}