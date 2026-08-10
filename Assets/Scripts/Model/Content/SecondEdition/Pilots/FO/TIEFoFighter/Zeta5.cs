using Abilities.SecondEdition;
using Content;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.TIEFoFighter
{
    public class Zeta5 : TIEFoFighter
    {
        public Zeta5() : base()
        {
            PilotInfo = new PilotCardInfo25(
                pilotName: "Zeta 5",
                pilotTitle: "Evacuation of D'Qar",
                faction: Faction.FirstOrder,
                initiative: 2,
                cost: 9,
                loadoutValue: 0,
                isLimited: true,
                limited: 1,
                abilityType: typeof(Zeta5Ability),
                extraUpgradeIcons: new()
                {
                    UpgradeType.Talent,
                    UpgradeType.Tech,
                    UpgradeType.Modification
                },
                tags: new()
                {
                    Tags.Tie
                },
                isStandardLayout: true,
                legality: new List<Legality>() { Legality.XWA }
            );

            PilotNameCanonical = "zeta5-evacuationofdqar";

            ShipAbilities.Add(new Merciless());

            MustHaveUpgrades.Add(typeof(Determination));
            MustHaveUpgrades.Add(typeof(PatternAnalyzer));
            MustHaveUpgrades.Add(typeof(TargetingMatrix));
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Zeta5Ability : GenericAbility
    {
        // While you perform an attack, if you are not shielded and the defender's initiative is higher than yours, add 1 focus result.

        public override void ActivateAbility()
        {
            AddDiceModification(
                name: HostShip.PilotInfo.PilotName,
                isAvailable: IsAvailable,
                aiPriority: AiPriority,
                modificationType: DiceModificationType.Add,
                count: 1,
                sideCanBeChangedTo: DieSide.Focus
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack && Combat.Attacker == HostShip && HostShip.State.ShieldsCurrent == 0 && Combat.Defender.State.Initiative > HostShip.State.Initiative;
        }

        private int AiPriority()
        {
            return 100;
        }
    }
}