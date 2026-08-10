using Abilities.SecondEdition;
using Content;
using SubPhases;
using System.Collections.Generic;
using Tokens;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.T65XWing
{
    public class WedgeAntillesBoE : T65XWingBoE
    {
        public WedgeAntillesBoE() : base()
        {
            PilotInfo = new PilotCardInfo25(
                "Wedge Antilles",
                "Battle Over Endor",
                Faction.Rebel,
                6,
                5,
                0,
                isLimited: true,
                abilityType: typeof(WedgeAntillesBoEAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Torpedo,
                    UpgradeType.Astromech
                },
                tags: new List<Tags>
                {
                    Tags.XWing
                },
                isStandardLayout: true,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            MustHaveUpgrades.Add(typeof(ItsATrap));
            MustHaveUpgrades.Add(typeof(Predator));
            MustHaveUpgrades.Add(typeof(AdvProtonTorpedoes));
            MustHaveUpgrades.Add(typeof(R2A3BoE));

            ShipInfo.Shields++;

            ImageUrl = "https://infinitearenas.com/xw2/images/quickbuilds/wedgeantilles-battleoverendor.png";

            PilotNameCanonical = "wedgeantilles-battleoverendor";

            ModelInfo.SkinName = "Wedge Antilles";
        }
    }

    public class WedgeAntillesBoEXWA : WedgeAntillesBoE
    {
        public WedgeAntillesBoEXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 15;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };

            ImageUrl = "https://infinitearenas.com/xw2xwa/images/quickbuilds/wedgeantilles-battleoverendor.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WedgeAntillesBoEAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackHitAsAttacker += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackHitAsAttacker -= RegisterAbility;
        }

        public void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackHit, AskAssignFocus);
        }

        private void AskAssignFocus(object sender, System.EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    AssignToken,
                    descriptionLong: "Do you want to gain 1 Focus Token?",
                    imageHolder: HostShip,
                    showAlwaysUseOption: true
                );
            }
            else
            {
                HostShip.Tokens.AssignToken(typeof(FocusToken), Triggers.FinishTrigger);
            }
        }

        private void AssignToken(object sender, System.EventArgs e)
        {
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName} gains Focus token");
            HostShip.Tokens.AssignToken(typeof(FocusToken), DecisionSubPhase.ConfirmDecision);
        }
    }
}