using Abilities.SecondEdition;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.T70XWing
{
    public class KazudaXiono : T70XWing
    {
        public KazudaXiono() : base()
        {
            PilotInfo = new PilotCardInfo25(
                pilotName: "Kazuda Xiono",
                pilotTitle: "Resistance Spy",
                faction: Faction.Resistance,
                initiative: 4,
                cost: 12,
                loadoutValue: 13,
                isLimited: true,
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>()
                {
                    Tags.XWing
                },
                abilityType: typeof(KazudaXionoLARAbility),
                legality: new List<Legality>() { Legality.XWA }
            );

            PilotNameCanonical = "kazudaxiono-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KazudaXionoLARAbility : GenericAbility
    {
        // While you defend or perform an attack, if the enemy ship has more damage cards than you have,
        // you may change a focus result to an evade or hit result.

        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Focus },
                sideCanBeChangedTo: DieSide.Success
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsAvailable()
        {
            GenericShip otherShip = null;

            if (Combat.Attacker == HostShip)
            {
                otherShip = Combat.Defender;

                if (Combat.DiceRollAttack.Focuses < 1) return false;
            }
            else if (Combat.Defender == HostShip)
            {
                otherShip = Combat.Attacker;

                if (Combat.DiceRollDefence.Focuses < 1) return false;
            }

            if (otherShip != null)
            {
                return (otherShip.State.HullMax - otherShip.State.HullCurrent) > (HostShip.State.HullMax - HostShip.State.HullCurrent);
            }

            return false;
        }

        public int GetAiPriority()
        {
            return 50;
        }
    }
}