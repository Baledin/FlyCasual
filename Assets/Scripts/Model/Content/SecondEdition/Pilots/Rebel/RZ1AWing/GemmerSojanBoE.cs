using Abilities.SecondEdition;
using Content;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.RZ1AWing
{
    public class GemmerSojanBoELSL : RZ1AWing
    {
        public GemmerSojanBoELSL() : base()
        {
            PilotInfo = new PilotCardInfo25(
                "Gemmer Sojan",
                "Battle Over Endor",
                Faction.Rebel,
                2,
                4,
                loadoutValue: 0,
                isLimited: true,
                abilityType: typeof(GemmerSojanBattleOverEndorAbility),
                tags: new List<Tags>
                {
                        Tags.AWing
                },
                extraUpgradeIcons: new List<UpgradeType> {
                        UpgradeType.Talent,
                        UpgradeType.Cannon,
                        UpgradeType.Modification,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                },
                isStandardLayout: true,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            PilotNameCanonical = "gemmersojan-battleoverendor";

            MustHaveUpgrades.Add(typeof(VectoredCannonsRZ1));
            MustHaveUpgrades.Add(typeof(ItsATrap));
            MustHaveUpgrades.Add(typeof(PrecisionTunedCannons));
            MustHaveUpgrades.Add(typeof(ChaffParticlesBoE));
            MustHaveUpgrades.Add(typeof(TargetAssistAlgorithm));
        }
    }

    public class GemmerSojanBoEXWA : GemmerSojanBoELSL
    {
        public GemmerSojanBoEXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 9;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GemmerSojanBattleOverEndorAbility : GenericAbility
    {
        //While defending, you may gain 1 strain token to change up to 2 of your blank results to focus results.
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Gemmer Sojan",
                IsAvailable,
                AiPriority,
                DiceModificationType.Change,
                2,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Blank },
                sideCanBeChangedTo: DieSide.Focus,
                payAbilityCost: PayAbilityCost
            );
        }
        private void PayAbilityCost(Action<bool> callback)
        {
            HostShip.Tokens.AssignToken(typeof(Tokens.StrainToken), () => callback(true));
        }

        public bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Defence && Combat.CurrentDiceRoll.HasResult(DieSide.Blank);
        }

        private int AiPriority()
        {
            int result = 0;

            if (Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes
                && Combat.DiceRollDefence.Blanks > 0
                && HostShip.Tokens.HasToken(typeof(FocusToken))
                && Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Focuses + Combat.DiceRollDefence.Successes)
            {
                result = 55;
            }

            return result;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}