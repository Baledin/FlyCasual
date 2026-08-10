using Abilities.SecondEdition;
using Actions;
using ActionsList;
using Content;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.RZ1AWing
{
    public class ArvelCrynydBoE : RZ1AWing
    {
        public ArvelCrynydBoE() : base()
        {
            PilotInfo = new PilotCardInfo25(
                "Arvel Crynyd",
                "Battle Over Endor",
                Faction.Rebel,
                3,
                4,
                0,
                isLimited: true,
                abilityType: typeof(ArvelCrynydBattleOverEndorAbility),
                tags: new List<Tags>
                {
                        Tags.AWing
                },
                extraUpgradeIcons: new List<UpgradeType> {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Configuration
                },
                isStandardLayout: true,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            ImageUrl = "https://infinitearenas.com/xw2/images/quickbuilds/arvelcrynyd-battleoverendor.png";

            PilotNameCanonical = "arvelcrynyd-battleoverendor";

            ShipInfo.Shields++;

            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(FocusAction)));
            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(SlamAction)));

            MustHaveUpgrades.Add(typeof(VectoredCannonsRZ1));
            MustHaveUpgrades.Add(typeof(HeroicSacrifice));
            MustHaveUpgrades.Add(typeof(ItsATrap));
            MustHaveUpgrades.Add(typeof(ProtonRockets));
        }
    }

    public class ArvelCrynydBoEXWA : ArvelCrynydBoE
    {
        public ArvelCrynydBoEXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 9;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ArvelCrynydBattleOverEndorAbility : GenericAbility
    {
        //While defending, you may gain 1 strain token to change 1 focus result to a evade result.
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Arvel Crynyd",
                IsAvailable,
                AiPriority,
                DiceModificationType.Change,
                1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Focus },
                sideCanBeChangedTo: DieSide.Success,
                payAbilityCost: PayAbilityCost
            );
        }
        private void PayAbilityCost(Action<bool> callback)
        {
            HostShip.Tokens.AssignToken(typeof(Tokens.StrainToken), () => callback(true));
        }

        public bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Defence && Combat.CurrentDiceRoll.HasResult(DieSide.Focus);
        }

        private int AiPriority()
        {
            int result = 0;

            if (Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes
                && Combat.DiceRollDefence.Focuses > 0 && !HostShip.Tokens.HasGreenTokens)
            {
                result = 15;
            }

            return result;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}