using Content;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEPhPhantom
    {
        public class Whisper : TIEPhPhantom
        {
            public Whisper() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Whisper\"",
                    "Soft-Spoken Slayer",
                    Faction.Imperial,
                    5,
                    5,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.WhisperAbility),
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Gunner,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    seImageNumber: 131,
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class WhisperXWA : Whisper
        {
            public WhisperXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 14;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 15;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Gunner,
                    UpgradeType.Modification,
                    UpgradeType.Tech
                };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WhisperAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackHitAsAttacker += RegisterWhisperAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackHitAsAttacker -= RegisterWhisperAbility;
        }

        public void RegisterWhisperAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackHit, AskAssignEvade);
        }

        private void AskAssignEvade(object sender, System.EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    AssignToken,
                    descriptionLong: "Do you want to gain 1 Evade Token?",
                    imageHolder: HostShip,
                    showAlwaysUseOption: true
                );
            }
            else
            {
                HostShip.Tokens.AssignToken(typeof(EvadeToken), Triggers.FinishTrigger);
            }
        }

        private void AssignToken(object sender, System.EventArgs e)
        {
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName} gains Evade token");
            HostShip.Tokens.AssignToken(typeof(EvadeToken), SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}
