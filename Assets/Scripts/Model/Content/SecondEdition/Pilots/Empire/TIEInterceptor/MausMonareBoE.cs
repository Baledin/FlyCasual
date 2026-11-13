using Abilities.SecondEdition;
using ActionsList;
using Content;
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
        public class MausMonareBoE : TIEInterceptor
        {
            public MausMonareBoE()
            {
                PilotInfo = new PilotCardInfo25(
                    "Maus Monare",
                    "Battle Over Endor",
                    Faction.Imperial,
                    3,
                    5,
                    0,
                    abilityType: typeof(MausMonareAbility),
                    isLimited: true,
                    isStandardLayout: true,
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }

                );
                PilotNameCanonical = "mausmonare-battleoverendor";

                ShipInfo.Shields++;
                AutoThrustersAbility oldAbility = (AutoThrustersAbility)ShipAbilities.First(n => n.GetType() == typeof(AutoThrustersAbility));
                ShipAbilities.Remove(oldAbility);
                ShipAbilities.Add(new SensitiveControlsRealAbility());

                MustHaveUpgrades.Add(typeof(NoEscape));
                MustHaveUpgrades.Add(typeof(Outmaneuver));
                MustHaveUpgrades.Add(typeof(FuelInjectionOverride));
            }
        }

        public class MausMonareBoEXWA : MausMonareBoE
        {
            public MausMonareBoEXWA() : base()
            {
                var pilot = (PilotCardInfo25)PilotInfo;
                pilot.Cost = 11;
                pilot.LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform an evade action, gain a calculate token.
    public class MausMonareAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckCalculateBonus;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckCalculateBonus;
        }

        private void CheckCalculateBonus(GenericAction action)
        {
            if (action is EvadeAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AssignBonusCalculateToken);
            }
        }
        private void AssignBonusCalculateToken(object sender, EventArgs e)
        {
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName} gains Calculate token");

            HostShip.Tokens.AssignToken(typeof(CalculateToken), Triggers.FinishTrigger);
        }

    }
}
