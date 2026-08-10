using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.TIEDDefender
{
    public class DarthVader : TIEDDefender
    {
        public DarthVader() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Darth Vader",
                "Dark Lord of the Sith",
                Faction.Imperial,
                6,
                9,
                10,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.DarthVaderDefenderAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.ForcePower,
                    UpgradeType.Tech,
                    UpgradeType.Cannon,
                    UpgradeType.Missile,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.Tie,
                    Tags.DarkSide,
                    Tags.Sith
                },
                abilityText: "You may not spend force charges except when attacking. While you perform an attack, you may spend 1 force charge to turn a blank result into a hit.",
                force: 3,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            PilotNameCanonical = "darthvader-tieddefender";
        }
    }

    public class DarthVaderXWA : DarthVader
    {
        public DarthVaderXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 22;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 14;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.ForcePower,
                UpgradeType.ForcePower,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Cannon,
                UpgradeType.Missile,
                UpgradeType.Configuration
            };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DarthVaderDefenderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Blank },
                sideCanBeChangedTo: DieSide.Success,
                payAbilityCost: SpendForce
            );

            HostShip.OnCheckCanUseForceNow += AddRestriction;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack &&
                Combat.DiceRollAttack.Blanks > 0 &&
                HostShip.State.Force > 0;
        }

        private int GetAiPriority()
        {
            return 45;
        }

        private void SpendForce(Action<bool> callback)
        {
            HostShip.State.SpendForce(1, delegate { callback(true); });
        }

        private void AddRestriction(ref bool isAllowed)
        {
            if (Combat.Attacker == null || Combat.Attacker.ShipId != HostShip.ShipId) isAllowed = false;
        }
    }
}