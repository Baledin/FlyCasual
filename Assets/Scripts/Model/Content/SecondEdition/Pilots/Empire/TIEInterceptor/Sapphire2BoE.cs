using Abilities.SecondEdition;
using Content;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class Sapphire2BoE : TIEInterceptor
        {
            public Sapphire2BoE()
            {
                PilotInfo = new PilotCardInfo25(
                    "Sapphire 2",
                    "Battle Over Endor",
                    Faction.Imperial,
                    1,
                    4,
                    0,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Modification,
                        UpgradeType.Tech
                    },
                    isLimited: true,
                    isStandardLayout: true,
                    abilityType: typeof(Sapphire2Ability),
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }

                );
                PilotNameCanonical = "sapphire2-battleoverendor";

                AutoThrustersAbility oldAbility = (AutoThrustersAbility)ShipAbilities.First(n => n.GetType() == typeof(AutoThrustersAbility));
                ShipAbilities.Remove(oldAbility);
                ShipAbilities.Add(new SensitiveControlsRealAbility());

                MustHaveUpgrades.Add(typeof(NoEscape));
                MustHaveUpgrades.Add(typeof(TargetingMatrix));
                MustHaveUpgrades.Add(typeof(PrimedThrusters));
                MustHaveUpgrades.Add(typeof(Reckless));
            }
        }


        public class Sapphire2BoEXWA : Sapphire2BoE
        {
            public Sapphire2BoEXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 10;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you defend, if you are focused, roll 1 additional defense die.
    public class Sapphire2Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfDefenceDice += CheckDefenseBonus;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfDefenceDice -= CheckDefenseBonus;
        }

        private void CheckDefenseBonus(ref int count)
        {
            if (HostShip.Tokens.HasToken(typeof(FocusToken)))
            {
                Messages.ShowInfo($"{HostShip.PilotInfo.PilotName} rolls 1 additional defense die");
                count++;
            }
        }
    }
}