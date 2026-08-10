using Abilities.SecondEdition;
using Content;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.TIEFoFighter
{
    public class TamaraRyvora : TIEFoFighter
    {
        public TamaraRyvora() : base()
        {
            PilotInfo = new PilotCardInfo25(
                pilotName: "Tamara Ryvora",
                pilotTitle: "DT-533",
                faction: Faction.FirstOrder,
                initiative: 4,
                cost: 9,
                loadoutValue: 8,
                isLimited: true,
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Modification,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Missile
                },
                tags: new List<Tags>()
                {
                    Tags.Tie
                },
                abilityType: typeof(TamaraRyvoraAbility),
                legality: new List<Legality>() { Legality.XWA }
            );

            PilotNameCanonical = "tamararyvora-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    // While a ship you are locking performs an attack, you may choose 1 attack die.
    // If you do, the attacker rerolls that die.

    public class TamaraRyvoraAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal += RegisterDiceModification;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal -= RegisterDiceModification;
        }

        private void RegisterDiceModification()
        {
            if(HostShip.GetTargetLockLetterPairsOn(Combat.Attacker).Any())
            {
                GenericShip.OnAttackFinishGlobal += RemoveDiceModifications;

                if (Tools.IsFriendly(HostShip, Combat.Attacker))
                {
                    AddDiceModification(
                        HostShip.PilotInfo.PilotName,
                        IsAvailable,
                        GetAiPriority,
                        DiceModificationType.Reroll,
                        1,
                        timing: DiceModificationTimingType.Normal,
                        isGlobal: true
                    );
                }
                else
                {
                    AddDiceModification(
                        HostShip.PilotInfo.PilotName,
                        IsAvailable,
                        GetAiPriority,
                        DiceModificationType.Reroll,
                        1,
                        timing: DiceModificationTimingType.Opposite,
                        isGlobal: true
                    );
                }
            }
        }

        private void RemoveDiceModifications(GenericShip ship)
        {
            GenericShip.OnAttackFinishGlobal -= RemoveDiceModifications;

            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            if (Combat.AttackStep != CombatStep.Attack) return false;

            if (HostShip.GetTargetLockLetterPairsOn(Combat.Attacker).Any()) return true;

            return false;
        }

        private int GetAiPriority()
        {
            if (Combat.DiceRollAttack.Successes > 0)
                return 100;

            return 0;
        }
    }
}