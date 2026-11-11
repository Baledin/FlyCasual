using Abilities.SecondEdition;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.T65XWing
{
    public class YendorBoE : T65XWingBoE
    {
        public YendorBoE() : base()
        {
            PilotInfo = new PilotCardInfo25(
                "Yendor",
                "Battle Over Endor",
                Faction.Rebel,
                5,
                5,
                0,
                isLimited: true,
                abilityType: typeof(YendorAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
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
            MustHaveUpgrades.Add(typeof(PlasmaTorpedoes));
            MustHaveUpgrades.Add(typeof(StabilizingAstromech));

            PilotNameCanonical = "yendor-battleoverendor";
        }
    }

    public class YendorBoEXWA : YendorBoE
    {
        public YendorBoEXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 12;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class YendorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Yendor",
                IsAvailable,
                AiPriority,
                DiceModificationType.Reroll,
                1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Blank },
                payAbilityCost: PayAbilityCost
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                && Combat.DiceRollAttack.Blanks > 0;
        }

        private int AiPriority()
        {
            return 75;
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            HostShip.Tokens.AssignToken(typeof(Tokens.StrainToken), () => callback(true));
        }
    }
}