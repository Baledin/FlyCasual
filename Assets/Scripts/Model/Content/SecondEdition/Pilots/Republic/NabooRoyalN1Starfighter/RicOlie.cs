using Abilities.SecondEdition;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NabooRoyalN1Starfighter
    {
        public class RicOlie : NabooRoyalN1Starfighter
        {
            public RicOlie() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Ric Olie",
                    "Bravo Leader",
                    Faction.Republic,
                    5,
                    4,
                    12,
                    isLimited: true,
                    abilityText: "When you defend or perform a primary attack, if the maneuver you revealed is greater than the enemy ship’s maneuver, roll 1 additional die.",
                    abilityType: typeof(RicOlieAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Astromech,
                        UpgradeType.Sensor,
                        UpgradeType.Torpedo,
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class RicOlieXWA : RicOlie
        {
            public RicOlieXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 11;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 16;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                    UpgradeType.Sensor,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Torpedo
                };

            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RicOlieAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += CheckAttackAbility;
            HostShip.AfterGotNumberOfDefenceDice += CheckDefensebility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= CheckAttackAbility;
            HostShip.AfterGotNumberOfDefenceDice -= CheckDefensebility;
        }

        private void CheckAttackAbility(ref int count)
        {
            if (HostShip.RevealedManeuver == null || Combat.Defender.RevealedManeuver == null) return;

            if (HostShip.RevealedManeuver.Speed > Combat.Defender.RevealedManeuver.Speed)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": +1 attack die");
                count++;
            }
        }

        private void CheckDefensebility(ref int count)
        {
            if (HostShip.RevealedManeuver == null || Combat.Attacker.RevealedManeuver == null) return;

            if (HostShip.RevealedManeuver.Speed > Combat.Attacker.RevealedManeuver.Speed)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": +1 defense die");
                count++;
            }
        }
    }
}
