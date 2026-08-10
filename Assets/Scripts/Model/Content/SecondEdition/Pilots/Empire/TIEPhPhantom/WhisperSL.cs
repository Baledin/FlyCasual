using Abilities.SecondEdition;
using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.TIEPhPhantom
{
    public class WhisperSL : TIEPhPhantom
    {
        public WhisperSL() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "\"Whisper\"",
                "Unseen Assailant",
                Faction.Imperial,
                5,
                5,
                0,
                charges: 2,
                isLimited: true,
                isStandardLayout: true,
                abilityType: typeof(WhisperSLAbility),
                tags: new List<Tags>
                {
                    Tags.Tie
                },
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Modification
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            PilotNameCanonical = "whisper-ssl";

            MustHaveUpgrades.Add(typeof(WithoutATrace));
            MustHaveUpgrades.Add(typeof(RelaySystem));
            MustHaveUpgrades.Add(typeof(StygiumReserve));
        }
    }

    public class WhisperSLXWA : WhisperSL
    {
        public WhisperSLXWA() : base()
        {
            PilotCardInfo25 pilotInfo = (PilotCardInfo25)PilotInfo;
            pilotInfo.LegalityInfo = new List<Legality> { Legality.XWA };
            pilotInfo.Cost = 15;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WhisperSLAbility : GenericAbility
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
                new CloakAction(),
                CleanUp,
                HostShip.PilotInfo.PilotName,
                "After you perform an attack, you may spend 1 Charge to perform a Cloak action.",
                HostShip
            );
        }

        private void RegisterSpendChargeTrigger(GenericAction action, ref bool isFreeAction)
        {
            RegisterAbilityTrigger(
                TriggerTypes.OnFreeAction,
                delegate
                {
                    HostShip.SpendCharge();
                    Triggers.FinishTrigger();
                }
            );
        }

        private void CleanUp()
        {
            HostShip.BeforeActionIsPerformed -= RegisterSpendChargeTrigger;
            Triggers.FinishTrigger();
        }
    }
}