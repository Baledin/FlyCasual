using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.Hwk290LightFreighter
{
    public class JanOrs : Hwk290LightFreighter
    {
        public JanOrs() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Jan Ors",
                "Espionage Expert",
                Faction.Rebel,
                5,
                6,
                10,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.JanOrsAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Modification,
                        UpgradeType.Modification,
                        UpgradeType.Title
                },
                tags: new List<Tags>
                {
                        Tags.Freighter
                },
                seImageNumber: 42,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class JanOrsXWA : JanOrs
    {
        public JanOrsXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 14;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 10;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Crew,
                    UpgradeType.Modification,
                    UpgradeType.Device,
                    UpgradeType.Title
                };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JanOrsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal += RegisterJanOrsAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal -= RegisterJanOrsAbility;
        }

        protected virtual void RegisterJanOrsAbility()
        {
            if (Combat.Attacker.Owner.PlayerNo == HostShip.Owner.PlayerNo && Combat.Attacker.ShipId != HostShip.ShipId
                            && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
            {
                DistanceInfo distanceInfo = new DistanceInfo(Combat.Attacker, HostShip);
                if (distanceInfo.Range < 4 && Board.IsShipInArc(HostShip, Combat.Attacker))
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskJanOrsAbility);
                }
            }
        }

        private void AskJanOrsAbility(object sender, System.EventArgs e)
        {
            if (!HostShip.Tokens.HasToken(typeof(StressToken)))
            {
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    UseJanOrsAbility,
                    descriptionLong: "Do you want to gain 1 stress token to allow attacker may roll 1 additional attack die?",
                    imageHolder: HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseJanOrsAbility(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(StressToken), AllowRollAdditionalDice);
        }

        private void AllowRollAdditionalDice()
        {
            Combat.Attacker.AfterGotNumberOfAttackDice += IncreaseByOne;
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void IncreaseByOne(ref int value)
        {
            value++;
            Combat.Attacker.AfterGotNumberOfAttackDice -= IncreaseByOne;
        }
    }
}