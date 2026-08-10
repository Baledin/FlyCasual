using Abilities.SecondEdition;
using Actions;
using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class SoontirFelBoE : TIEInterceptor
        {
            public SoontirFelBoE()
            {
                PilotInfo = new PilotCardInfo25(
                    "Soontir Fel",
                    "Battle Over Endor",
                    Faction.Imperial,
                    6,
                    5,
                    loadoutValue: 0,
                    isStandardLayout: true,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Illicit
                    },

                    abilityType: typeof(SoontirFelBattleOverEndorAbility),
                    charges: 2,
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );

                PilotNameCanonical = "soontirfel-battleoverendor";
                AutoThrustersAbility oldAbility = (AutoThrustersAbility)ShipAbilities.First(n => n.GetType() == typeof(AutoThrustersAbility));
                ShipAbilities.Remove(oldAbility);
                ShipAbilities.Add(new SensitiveControlsRealAbility());
                ModelInfo.SkinName = "Red Stripes";

                MustHaveUpgrades.Add(typeof(NoEscape));
                MustHaveUpgrades.Add(typeof(ApexPredator));
                MustHaveUpgrades.Add(typeof(BlankSignature));
                MustHaveUpgrades.Add(typeof(FeedbackEmitter));

                ImageUrl = "https://infinitearenas.com/xw2/images/quickbuilds/soontirfel-battleoverendor.png";
            }
        }

        public class SoontirFelBoEXWA : SoontirFelBoE
        {
            public SoontirFelBoEXWA() : base()
            {
                var pilotInfo = PilotInfo as PilotCardInfo25;
                pilotInfo.Cost = 15;
                pilotInfo.LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform an attack, you may spend 1 Charge and gain 1 deplete token to boost or barrel roll.
    public class SoontirFelBattleOverEndorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship)
        {
            if (HostShip.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskToUseAbility);
            }

        }

        private void AskToUseAbility(object sender, EventArgs e)
        {
            HostShip.BeforeActionIsPerformed += RegisterSpendChargeTrigger;
            CameraScript.RestoreCamera();

            HostShip.AskPerformFreeAction(
                new List<GenericAction>()
                {
                    new SoontirFelBoost(){CanBePerformedWhileStressed = true, Color = ActionColor.White},
                    new SoontirFelBarrelRoll(){CanBePerformedWhileStressed = true, Color = ActionColor.White}
                },
                CleanUp,
                HostShip.PilotInfo.PilotName,
                "After you perform an attack, you may spend 1 Charge and gain 1 Deplete token to perform a Barrel Roll or Boost.",
                HostShip
            );
        }

        private void RegisterSpendChargeTrigger(GenericAction action, ref bool isFreeAction)
        {
            HostShip.BeforeActionIsPerformed -= RegisterSpendChargeTrigger;
            RegisterAbilityTrigger(
                TriggerTypes.OnFreeAction,
                delegate
                {
                    HostShip.SpendCharge();
                    HostShip.Tokens.AssignToken(typeof(DepleteToken), Triggers.FinishTrigger);
                }
            );
        }
        private void CleanUp()
        {
            HostShip.BeforeActionIsPerformed -= RegisterSpendChargeTrigger;
            Triggers.FinishTrigger();
        }
    }

    public class SoontirFelBoost : BoostAction
    {
        public SoontirFelBoost() : base()
        {
            Name = "Soontir Fel Boost";
        }
    }

    public class SoontirFelBarrelRoll : BarrelRollAction
    {
        public SoontirFelBarrelRoll() : base()
        {
            Name = "Soontir Fel Barrel Roll";
        }
    }
}